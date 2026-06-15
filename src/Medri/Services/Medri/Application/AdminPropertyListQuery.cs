using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminPropertyListQuery
    {
        private readonly MedriDbContext dbContext;

        public AdminPropertyListQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminPropertyListResultDto> ExecuteAsync(
            AdminPropertyListQueryFilter filter,
            CancellationToken cancellationToken = default)
        {
            filter ??= new AdminPropertyListQueryFilter();

            var baseQuery = CreateBaseQuery();
            var filteredQuery = ApplyFilters(baseQuery, filter)
                .OrderBy(row => row.SortOrder)
                .ThenBy(row => row.Reference);

            var advisors = await dbContext.AgencyUsers
                .AsNoTracking()
                .Where(advisor => !advisor.IsSystemSeed)
                .OrderBy(advisor => advisor.DisplayName)
                .ToArrayAsync(cancellationToken);
            var pageSize = NormalizePageSize(filter.PageSize);
            var totalItems = await filteredQuery.CountAsync(cancellationToken);
            var page = NormalizePage(filter.Page, totalItems, pageSize);
            var properties = await filteredQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(row => new AdminPropertyRowDto
                {
                    Id = row.Id,
                    Reference = row.Reference,
                    Title = row.Title,
                    DisplayLocation = row.DisplayLocation,
                    Contract = row.Contract,
                    PublicationStatus = row.PublicationStatus,
                    CompletionPercent = row.CompletionPercent,
                    MissingItems = row.MissingItems,
                    AssignedAgencyUserId = row.AssignedAgencyUserId,
                    AdvisorDisplayName = row.AdvisorDisplayName,
                    ImageUrl = row.ImageUrl,
                    SortOrder = row.SortOrder,
                    IsFeatured = row.PublicationStatus == PropertyPublicationStatuses.Published &&
                        row.FeaturedSortOrder.HasValue,
                    CanFeature = row.PublicationStatus == PropertyPublicationStatuses.Published &&
                        !row.FeaturedSortOrder.HasValue,
                    FeaturedSortOrder = row.FeaturedSortOrder
                })
                .ToArrayAsync(cancellationToken);

            return new AdminPropertyListResultDto
            {
                LeadCount = await dbContext.Leads
                    .AsNoTracking()
                    .CountAsync(
                        lead => lead.InternalReference != null && lead.WorkflowStatus == LeadWorkflowStatuses.New,
                        cancellationToken),
                ActiveRequestCount = await dbContext.SearchProfiles
                    .AsNoTracking()
                    .CountAsync(
                        profile => profile.PublicReference != null && profile.Status != RequestStatuses.Archived,
                        cancellationToken),
                ListingCount = await baseQuery.CountAsync(cancellationToken),
                IncompleteCount = await baseQuery.CountAsync(row => row.PublicationStatus == PropertyPublicationStatuses.Incomplete, cancellationToken),
                ReadyCount = await baseQuery.CountAsync(row => row.PublicationStatus == PropertyPublicationStatuses.Ready, cancellationToken),
                PublishedCount = await baseQuery.CountAsync(row => row.PublicationStatus == PropertyPublicationStatuses.Published, cancellationToken),
                NeedsUpdateCount = await baseQuery.CountAsync(row => row.PublicationStatus == PropertyPublicationStatuses.NeedsUpdate, cancellationToken),
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                Properties = properties,
                FeaturedSlots = await baseQuery
                    .Where(listing => listing.PublicationStatus == PropertyPublicationStatuses.Published &&
                                      listing.FeaturedSortOrder.HasValue)
                    .OrderBy(listing => listing.FeaturedSortOrder)
                    .ThenBy(listing => listing.SortOrder)
                    .Take(3)
                    .Select(listing => new AdminPropertyFeaturedSlotDto
                    {
                        SlotNumber = listing.FeaturedSortOrder.GetValueOrDefault(),
                        Reference = listing.Reference,
                        Title = listing.Title,
                        DisplayLocation = listing.DisplayLocation
                    })
                    .ToArrayAsync(cancellationToken),
                Advisors = advisors
                    .Select(advisor => new AdminPropertyAdvisorDto
                    {
                        Id = advisor.Id,
                        DisplayName = advisor.DisplayName
                    })
                    .ToArray()
            };
        }

        private IQueryable<AdminPropertyProjectedRow> CreateBaseQuery()
        {
            return from listing in dbContext.PropertyListings
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                   join advisor in dbContext.AgencyUsers.AsNoTracking()
                        on listing.AssignedAgencyUserId equals (Guid?)advisor.Id into advisorRows
                   from advisor in advisorRows.DefaultIfEmpty()
                   where listing.InternalReference != null &&
                         listing.PublicationStatus != PropertyPublicationStatuses.Archived
                   select new AdminPropertyProjectedRow
                   {
                       Id = listing.Id,
                       Reference = listing.InternalReference,
                       Title = listing.Title,
                       Location = listing.Location,
                       DisplayLocation = string.IsNullOrWhiteSpace(listing.DisplayLocation)
                           ? listing.Location
                           : listing.DisplayLocation,
                       Contract = listing.Contract,
                       PublicationStatus = listing.PublicationStatus,
                       CompletionPercent = listing.CompletionPercent,
                       MissingItems = listing.MissingItems,
                       AssignedAgencyUserId = listing.AssignedAgencyUserId,
                       AdvisorDisplayName = advisor == null ? null : advisor.DisplayName,
                       ImageUrl = listing.ImageUrl,
                       SortOrder = listing.SortOrder,
                       FeaturedSortOrder = listing.FeaturedSortOrder
                   };
        }

        private static IQueryable<AdminPropertyProjectedRow> ApplyFilters(
            IQueryable<AdminPropertyProjectedRow> rows,
            AdminPropertyListQueryFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim().ToLowerInvariant();
                rows = rows.Where(row =>
                    (row.Reference != null && row.Reference.ToLower().Contains(searchTerm)) ||
                    (row.Title != null && row.Title.ToLower().Contains(searchTerm)) ||
                    (row.DisplayLocation != null && row.DisplayLocation.ToLower().Contains(searchTerm)) ||
                    (row.Location != null && row.Location.ToLower().Contains(searchTerm)) ||
                    (row.AdvisorDisplayName != null && row.AdvisorDisplayName.ToLower().Contains(searchTerm)) ||
                    (row.MissingItems != null && row.MissingItems.ToLower().Contains(searchTerm)));
            }

            var statuses = CleanValues(filter.Statuses);
            if (statuses.Count > 0)
            {
                rows = rows.Where(row => statuses.Contains(row.PublicationStatus));
            }

            var contracts = CleanValues(filter.Contracts);
            if (contracts.Count > 0)
            {
                rows = rows.Where(row => contracts.Contains(row.Contract));
            }

            var advisorIds = filter.AssignedAgencyUserIds ?? Array.Empty<Guid>();
            if (advisorIds.Count > 0 && filter.OnlyUnassigned)
            {
                rows = rows.Where(row =>
                    !row.AssignedAgencyUserId.HasValue ||
                    advisorIds.Contains(row.AssignedAgencyUserId.Value));
            }
            else if (advisorIds.Count > 0)
            {
                rows = rows.Where(row =>
                    row.AssignedAgencyUserId.HasValue &&
                    advisorIds.Contains(row.AssignedAgencyUserId.Value));
            }
            else if (filter.OnlyUnassigned)
            {
                rows = rows.Where(row => row.AssignedAgencyUserId == null);
            }

            return rows;
        }

        private static IReadOnlyList<string> CleanValues(IReadOnlyList<string> values)
        {
            return (values ?? Array.Empty<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static int NormalizePage(int requestedPage, int totalItems, int pageSize)
        {
            return ApplicationText.NormalizePage(requestedPage, totalItems, pageSize);
        }

        private static int NormalizePageSize(int requestedPageSize)
        {
            return ApplicationText.NormalizePageSize(requestedPageSize, 15, 25, 50);
        }

        private sealed class AdminPropertyProjectedRow
        {
            public Guid Id { get; set; }

            public string Reference { get; set; }

            public string Title { get; set; }

            public string Location { get; set; }

            public string DisplayLocation { get; set; }

            public string Contract { get; set; }

            public string PublicationStatus { get; set; }

            public int CompletionPercent { get; set; }

            public string MissingItems { get; set; }

            public Guid? AssignedAgencyUserId { get; set; }

            public string AdvisorDisplayName { get; set; }

            public string ImageUrl { get; set; }

            public int SortOrder { get; set; }

            public int? FeaturedSortOrder { get; set; }
        }
    }
}

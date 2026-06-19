using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminPropertyDetailQuery
    {
        private readonly MedriDbContext dbContext;

        public AdminPropertyDetailQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminPropertyDetailResultDto> CreateNewAsync(
            CancellationToken cancellationToken = default)
        {
            return new AdminPropertyDetailResultDto
            {
                IsCreateMode = true,
                Reference = "Nuovo immobile",
                PublicationStatus = PropertyPublicationStatuses.Incomplete,
                CompletionPercent = 0,
                MissingItems = "Dati minimi, testi e media",
                Title = "Nuovo immobile",
                Slug = string.Empty,
                ListingCategory = "Abitazione",
                DisplayLocation = "Da completare",
                Contract = "Vendita",
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
                ListingCount = await AdminNavigationCounts.UnpublishedListingsAsync(dbContext, cancellationToken),
                Checklist = Checklist(new PropertyListing
                {
                    DisplayLocation = string.Empty,
                    Address = string.Empty,
                    MissingItems = "Dati minimi, testi e media"
                }, Array.Empty<PropertyMedia>()),
                Advisors = await AdvisorsAsync(cancellationToken)
            };
        }

        public async Task<AdminPropertyDetailResultDto> ExecuteAsync(
            string reference,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return null;
            }

            var listing = await dbContext.PropertyListings
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    item => item.InternalReference == reference,
                    cancellationToken);

            if (listing == null)
            {
                return null;
            }

            var advisors = await AdvisorsAsync(cancellationToken);
            var advisorDisplayName = advisors
                .Where(advisor => listing.AssignedAgencyUserId.HasValue &&
                                  advisor.Id == listing.AssignedAgencyUserId.Value)
                .Select(advisor => advisor.DisplayName)
                .FirstOrDefault();

            var mediaRows = await dbContext.PropertyMedia
                .AsNoTracking()
                .Where(item => item.PropertyListingId == listing.Id)
                .OrderBy(item => item.SortOrder)
                .ToArrayAsync(cancellationToken);
            var completion = AdminPropertyCompletionCalculator.Calculate(
                listing,
                mediaRows.Length);
            var media = mediaRows
                .Select(item => new AdminPropertyMediaDto
                {
                    Id = item.Id,
                    Url = item.Url,
                    AltText = item.AltText,
                    SortOrder = item.SortOrder
                })
                .ToArray();

            return new AdminPropertyDetailResultDto
            {
                Id = listing.Id,
                Reference = listing.InternalReference,
                PublicationStatus = listing.PublicationStatus,
                CompletionPercent = completion.CompletionPercent,
                MissingItems = completion.MissingItems,
                Title = listing.Title,
                Slug = listing.Slug,
                ImageUrl = listing.ImageUrl,
                ListingCategory = listing.ListingCategory,
                DisplayLocation = DisplayLocation(listing),
                Address = listing.Address,
                Latitude = listing.Latitude,
                Longitude = listing.Longitude,
                Contract = listing.Contract,
                Price = listing.Price,
                SurfaceSquareMeters = listing.SurfaceSquareMeters,
                Rooms = listing.Rooms,
                BedroomsLabel = listing.BedroomsLabel,
                Bathrooms = listing.Bathrooms,
                EnergyClass = listing.EnergyClass,
                RequiredWorksLabel = listing.RequiredWorksLabel,
                CondoFeesLabel = listing.CondoFeesLabel,
                FloorLabel = listing.FloorLabel,
                ElevatorLabel = listing.ElevatorLabel,
                OutdoorSpaceLabel = listing.OutdoorSpaceLabel,
                GarageLabel = listing.GarageLabel,
                Description = listing.SummaryParagraph1,
                SummaryTitle = listing.SummaryTitle,
                SummaryParagraph2 = listing.SummaryParagraph2,
                ReadinessNote = listing.ReadinessNote,
                CostsNote = listing.CostsNote,
                ContextNote = listing.ContextNote,
                DecisionMarginNote = listing.DecisionMarginNote,
                AvailabilityLabel = listing.AvailabilityLabel,
                HeatingLabel = listing.HeatingLabel,
                ConstructionYearLabel = listing.ConstructionYearLabel,
                BalconyLabel = listing.BalconyLabel,
                CellarLabel = listing.CellarLabel,
                NearbyServicesLabel = listing.NearbyServicesLabel,
                QuickNotes = QuickNotes(listing),
                HumanFitNote = listing.HumanFitNote,
                AdvisorDisplayName = advisorDisplayName,
                AssignedAgencyUserId = listing.AssignedAgencyUserId,
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
                ListingCount = await AdminNavigationCounts.UnpublishedListingsAsync(dbContext, cancellationToken),
                Media = media,
                Checklist = completion.Checklist
                    .Select(item => new AdminPropertyChecklistItemDto
                    {
                        Label = item.Label,
                        IsDone = item.IsDone
                    })
                    .ToArray(),
                Advisors = advisors
            };
        }

        private async Task<IReadOnlyList<AdminPropertyAdvisorDto>> AdvisorsAsync(
            CancellationToken cancellationToken)
        {
            return await StaffUserQueries.Assignable(dbContext)
                .OrderBy(advisor => advisor.DisplayName)
                .Select(advisor => new AdminPropertyAdvisorDto
                {
                    Id = advisor.Id,
                    DisplayName = advisor.DisplayName
                })
                .ToArrayAsync(cancellationToken);
        }

        private static string DisplayLocation(PropertyListing listing)
        {
            return string.IsNullOrWhiteSpace(listing.DisplayLocation)
                ? listing.Location
                : listing.DisplayLocation;
        }

        private static string[] QuickNotes(PropertyListing listing)
        {
            return new[]
                {
                    listing.AccessLabel,
                    listing.ContextNote,
                    listing.MainCompromise,
                    listing.NearbyServicesLabel,
                    listing.DecisionMarginNote,
                    listing.CostsNote
                }
                .Select(value => value ?? string.Empty)
                .Take(6)
                .ToArray();
        }

        private static AdminPropertyChecklistItemDto[] Checklist(
            PropertyListing listing,
            IReadOnlyList<PropertyMedia> media)
        {
            var completion = AdminPropertyCompletionCalculator.Calculate(
                listing,
                media.Count);
            return completion.Checklist
                .Select(item => new AdminPropertyChecklistItemDto
                {
                    Label = item.Label,
                    IsDone = item.IsDone
                })
                .ToArray();
        }
    }
}

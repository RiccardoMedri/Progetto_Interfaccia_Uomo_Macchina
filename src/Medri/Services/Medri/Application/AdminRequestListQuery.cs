using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminRequestListQuery
    {
        private readonly MedriDbContext dbContext;

        public AdminRequestListQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminRequestListResultDto> ExecuteAsync(
            AdminRequestListQueryFilter filter,
            CancellationToken cancellationToken = default)
        {
            filter ??= new AdminRequestListQueryFilter();

            var baseQuery = CreateBaseQuery();
            var filteredQuery = ApplyFilters(baseQuery, filter)
                .OrderByDescending(row => row.Reference);

            var advisors = await dbContext.AgencyUsers
                .AsNoTracking()
                .Where(advisor => !advisor.IsSystemSeed)
                .OrderBy(advisor => advisor.DisplayName)
                .ToArrayAsync(cancellationToken);
            var pageSize = NormalizePageSize(filter.PageSize);
            var totalItems = await filteredQuery.CountAsync(cancellationToken);
            var page = NormalizePage(filter.Page, totalItems, pageSize);
            var requests = (await filteredQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToArrayAsync(cancellationToken))
                .Select(CreateRow)
                .ToArray();

            return new AdminRequestListResultDto
            {
                LeadCount = await dbContext.Leads
                    .AsNoTracking()
                    .CountAsync(
                        lead => lead.InternalReference != null && lead.WorkflowStatus == LeadWorkflowStatuses.New,
                        cancellationToken),
                ActiveRequestCount = await baseQuery.CountAsync(cancellationToken),
                ListingCount = await AdminNavigationCounts.UnpublishedListingsAsync(dbContext, cancellationToken),
                NewCount = await baseQuery.CountAsync(row => row.Status == RequestStatuses.New, cancellationToken),
                AssignedCount = await baseQuery.CountAsync(row => row.AssignedAgencyUserId.HasValue, cancellationToken),
                UpdatingCount = await baseQuery.CountAsync(row => row.Status == RequestStatuses.Updating, cancellationToken),
                MatchingCount = await baseQuery.CountAsync(row => row.Status == RequestStatuses.InMatching, cancellationToken),
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                Requests = requests,
                Advisors = advisors
                    .Select(advisor => new AdminLeadAdvisorDto
                    {
                        Id = advisor.Id,
                        DisplayName = advisor.DisplayName
                    })
                    .ToArray()
            };
        }

        private IQueryable<AdminRequestProjectedRow> CreateBaseQuery()
        {
            return from profile in dbContext.SearchProfiles.AsNoTracking()
                   join lead in dbContext.Leads.AsNoTracking()
                        on profile.LeadId equals lead.Id
                   join preference in dbContext.LeadPreferences.AsNoTracking()
                        on lead.Id equals preference.LeadId into preferenceRows
                   from preference in preferenceRows.DefaultIfEmpty()
                   join advisor in dbContext.AgencyUsers.AsNoTracking()
                        on lead.AssignedAgencyUserId equals (Guid?)advisor.Id into advisorRows
                   from advisor in advisorRows.DefaultIfEmpty()
                   where profile.PublicReference != null &&
                         profile.Status != RequestStatuses.Archived
                   select new AdminRequestProjectedRow
                   {
                       Id = profile.Id,
                       Reference = profile.PublicReference,
                       CustomerName = lead.FullName,
                       Phone = lead.Phone,
                       Email = lead.Email,
                       RequestType = lead.RequestType,
                       Status = profile.Status,
                       AssignedAgencyUserId = lead.AssignedAgencyUserId,
                       Priority = preference == null ||
                                  preference.Timing == null ||
                                  preference.Timing == string.Empty
                            ? null
                            : preference.Timing,
                       CriteriaSummary = profile.CriteriaSummary,
                       PreferencePropertyType = preference == null ? null : preference.PropertyType,
                       PreferenceDesiredLocation = preference == null ? null : preference.DesiredLocation,
                       PreferenceAcceptableLocations = preference == null ? null : preference.AcceptableLocations,
                       PreferenceBudget = preference == null ? null : preference.SustainableBudgetLabel,
                       PreferenceMinimumRooms = preference == null ? null : preference.MinimumRooms,
                       PreferenceValuationGoal = preference == null ? null : preference.ValuationGoal,
                       PreferencePropertyToSellStatus = preference == null ? null : preference.PropertyToSellStatus,
                       PreferenceCompromises = preference == null ? null : preference.PreferencesAndCompromises,
                       PreferenceNegotiableItems = preference == null ? null : preference.PropertyCondition,
                       PreferenceQuestion = preference == null ? null : preference.ExpectedPriceOrMainQuestion,
                       AdvisorDisplayName = advisor == null ? null : advisor.DisplayName
                   };
        }

        private static AdminRequestRowDto CreateRow(AdminRequestProjectedRow row)
        {
            return new AdminRequestRowDto
            {
                Id = row.Id,
                Reference = row.Reference,
                CustomerName = row.CustomerName,
                ContactSummary = ContactSummary(row),
                RequestType = row.RequestType,
                Status = row.Status,
                CompletionPercent = AdminRequestCompletionCalculator.Calculate(
                    row.Phone,
                    row.Email,
                    row.AssignedAgencyUserId,
                    row.PreferenceBudget,
                    row.PreferenceQuestion,
                    row.PreferenceDesiredLocation,
                    row.PreferenceAcceptableLocations,
                    row.PreferenceMinimumRooms,
                    row.PreferencePropertyType,
                    row.PreferenceValuationGoal,
                    row.PreferencePropertyToSellStatus,
                    row.PreferenceCompromises,
                    row.PreferenceNegotiableItems),
                AssignedAgencyUserId = row.AssignedAgencyUserId,
                IsAssigned = !string.IsNullOrWhiteSpace(row.AdvisorDisplayName),
                Priority = row.Priority,
                Criteria = Criteria(row),
                AdvisorDisplayName = row.AdvisorDisplayName
            };
        }

        private static IQueryable<AdminRequestProjectedRow> ApplyFilters(
            IQueryable<AdminRequestProjectedRow> rows,
            AdminRequestListQueryFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim().ToLowerInvariant();
                rows = rows.Where(row =>
                    (row.Reference != null && row.Reference.ToLower().Contains(searchTerm)) ||
                    (row.CustomerName != null && row.CustomerName.ToLower().Contains(searchTerm)) ||
                    (row.Phone != null && row.Phone.ToLower().Contains(searchTerm)) ||
                    (row.Email != null && row.Email.ToLower().Contains(searchTerm)) ||
                    (row.AdvisorDisplayName != null && row.AdvisorDisplayName.ToLower().Contains(searchTerm)) ||
                    (row.Priority != null && row.Priority.ToLower().Contains(searchTerm)) ||
                    (row.CriteriaSummary != null && row.CriteriaSummary.ToLower().Contains(searchTerm)) ||
                    (row.PreferencePropertyType != null && row.PreferencePropertyType.ToLower().Contains(searchTerm)) ||
                    (row.PreferenceDesiredLocation != null && row.PreferenceDesiredLocation.ToLower().Contains(searchTerm)) ||
                    (row.PreferenceBudget != null && row.PreferenceBudget.ToLower().Contains(searchTerm)) ||
                    (row.PreferenceValuationGoal != null && row.PreferenceValuationGoal.ToLower().Contains(searchTerm)) ||
                    (row.PreferencePropertyToSellStatus != null && row.PreferencePropertyToSellStatus.ToLower().Contains(searchTerm)) ||
                    (row.PreferenceCompromises != null && row.PreferenceCompromises.ToLower().Contains(searchTerm)) ||
                    (row.PreferenceQuestion != null && row.PreferenceQuestion.ToLower().Contains(searchTerm)));
            }

            var requestTypes = CleanValues(filter.RequestTypes);
            if (requestTypes.Count > 0)
            {
                rows = rows.Where(row => requestTypes.Contains(row.RequestType));
            }

            var statuses = CleanValues(filter.Statuses);
            if (statuses.Count > 0)
            {
                var includeAssigned = statuses.Contains("Assigned");
                rows = rows.Where(row =>
                    (includeAssigned && row.AssignedAgencyUserId.HasValue) ||
                    statuses.Contains(row.Status));
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
                rows = rows.Where(row => !row.AssignedAgencyUserId.HasValue);
            }

            var priorities = CleanValues(filter.Priorities);
            if (priorities.Count > 0)
            {
                rows = rows.Where(row => priorities.Contains(row.Priority));
            }

            return rows;
        }

        private static string ContactSummary(AdminRequestProjectedRow row)
        {
            if (!string.IsNullOrWhiteSpace(row.Phone) &&
                !string.IsNullOrWhiteSpace(row.Email))
            {
                return $"{row.Phone} - {row.Email}";
            }

            if (!string.IsNullOrWhiteSpace(row.Phone))
            {
                return row.Phone;
            }

            if (!string.IsNullOrWhiteSpace(row.Email))
            {
                return row.Email;
            }

            return "Recapito da completare";
        }

        private static IReadOnlyList<string> Criteria(AdminRequestProjectedRow row)
        {
            var summaryCriteria = SplitCriteriaSummary(row.CriteriaSummary);
            if (summaryCriteria.Length > 0)
            {
                return summaryCriteria;
            }

            var criteria = new[]
                {
                    row.PreferencePropertyType,
                    row.PreferenceDesiredLocation,
                    row.PreferenceBudget,
                    row.PreferenceMinimumRooms.HasValue
                        ? $"{row.PreferenceMinimumRooms.Value} camere"
                        : null,
                    row.PreferenceValuationGoal,
                    row.PreferencePropertyToSellStatus,
                    row.PreferenceCompromises,
                    row.PreferenceQuestion
                }
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Take(3)
                .ToArray();

            if (criteria.Length > 0)
            {
                return criteria;
            }

            return Array.Empty<string>();
        }

        private static string[] SplitCriteriaSummary(string criteriaSummary)
        {
            return (criteriaSummary ?? string.Empty)
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Take(3)
                .ToArray();
        }

        private static IReadOnlyList<string> CleanValues(IReadOnlyList<string> values)
        {
            return (values ?? Array.Empty<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static int NormalizePage(int page, int totalItems, int pageSize)
        {
            return ApplicationText.NormalizePage(page, totalItems, pageSize);
        }

        private static int NormalizePageSize(int pageSize)
        {
            return ApplicationText.NormalizePageSize(pageSize, 15, 25, 50);
        }

        private sealed class AdminRequestProjectedRow
        {
            public Guid Id { get; set; }

            public string Reference { get; set; }

            public string CustomerName { get; set; }

            public string Phone { get; set; }

            public string Email { get; set; }

            public string RequestType { get; set; }

            public string Status { get; set; }

            public Guid? AssignedAgencyUserId { get; set; }

            public string Priority { get; set; }

            public string CriteriaSummary { get; set; }

            public string PreferencePropertyType { get; set; }

            public string PreferenceDesiredLocation { get; set; }

            public string PreferenceAcceptableLocations { get; set; }

            public string PreferenceBudget { get; set; }

            public int? PreferenceMinimumRooms { get; set; }

            public string PreferenceValuationGoal { get; set; }

            public string PreferencePropertyToSellStatus { get; set; }

            public string PreferenceCompromises { get; set; }

            public string PreferenceNegotiableItems { get; set; }

            public string PreferenceQuestion { get; set; }

            public string AdvisorDisplayName { get; set; }
        }
    }
}

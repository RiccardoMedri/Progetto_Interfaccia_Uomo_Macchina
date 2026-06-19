using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminLeadListQuery
    {
        private readonly MedriDbContext dbContext;

        public AdminLeadListQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminLeadListResultDto> ExecuteAsync(
            AdminLeadListQueryFilter filter,
            CancellationToken cancellationToken = default)
        {
            filter ??= new AdminLeadListQueryFilter();

            var adminLeads = dbContext.Leads
                .AsNoTracking()
                .Where(lead =>
                    lead.InternalReference != null &&
                    lead.WorkflowStatus != LeadWorkflowStatuses.Qualified);

            var filteredLeads = ApplyFilters(adminLeads, filter);
            var totalItems = await filteredLeads.CountAsync(cancellationToken);
            var pageSize = NormalizePageSize(filter.PageSize);
            var page = NormalizePage(filter.Page, totalItems, pageSize);

            var rows = await filteredLeads
                .GroupJoin(
                    StaffUserQueries.Assignable(dbContext),
                    lead => lead.AssignedAgencyUserId,
                    advisor => advisor.Id,
                    (lead, advisors) => new
                    {
                        lead,
                        Advisor = advisors.FirstOrDefault()
                    })
                .OrderByDescending(item => item.lead.InternalReference)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(item => new AdminLeadRowDto
                {
                    Id = item.lead.Id,
                    Reference = item.lead.InternalReference,
                    FullName = item.lead.FullName,
                    Email = item.lead.Email,
                    Phone = item.lead.Phone,
                    SourceChannel = item.lead.SourceChannel,
                    RequestType = item.lead.RequestType,
                    WorkflowStatus = item.lead.WorkflowStatus,
                    Priority = dbContext.LeadPreferences
                        .AsNoTracking()
                        .Where(preference => preference.LeadId == item.lead.Id)
                        .Select(preference => preference.Timing)
                        .FirstOrDefault(),
                    QualificationPercent = item.lead.QualificationPercent,
                    AdvisorDisplayName = item.Advisor == null ? null : item.Advisor.DisplayName
                })
                .ToArrayAsync(cancellationToken);

            return new AdminLeadListResultDto
            {
                NewCount = await adminLeads.CountAsync(
                    lead => lead.WorkflowStatus == LeadWorkflowStatuses.New,
                    cancellationToken),
                InContactCount = await adminLeads.CountAsync(
                    lead => lead.WorkflowStatus == LeadWorkflowStatuses.InContact,
                    cancellationToken),
                ArchivedCount = await adminLeads.CountAsync(
                    lead => lead.WorkflowStatus == LeadWorkflowStatuses.Archived,
                    cancellationToken),
                ActiveRequestCount = await dbContext.SearchProfiles
                    .AsNoTracking()
                    .CountAsync(
                        profile => profile.PublicReference != null && profile.Status != RequestStatuses.Archived,
                        cancellationToken),
                ListingCount = await AdminNavigationCounts.UnpublishedListingsAsync(dbContext, cancellationToken),
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                Leads = rows,
                Advisors = await StaffUserQueries.Assignable(dbContext)
                    .OrderBy(advisor => advisor.DisplayName)
                    .Select(advisor => new AdminLeadAdvisorDto
                    {
                        Id = advisor.Id,
                        DisplayName = advisor.DisplayName
                    })
                    .ToArrayAsync(cancellationToken)
            };
        }

        private IQueryable<Lead> ApplyFilters(
            IQueryable<Lead> query,
            AdminLeadListQueryFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();
                query = query.Where(lead =>
                    lead.FullName.Contains(searchTerm) ||
                    (lead.Phone != null && lead.Phone.Contains(searchTerm)) ||
                    (lead.Email != null && lead.Email.Contains(searchTerm)) ||
                    (lead.NextAction != null && lead.NextAction.Contains(searchTerm)) ||
                    (lead.Notes != null && lead.Notes.Contains(searchTerm)) ||
                    dbContext.LeadPreferences
                        .AsNoTracking()
                        .Any(preference =>
                            preference.LeadId == lead.Id &&
                            ((preference.DesiredLocation != null && preference.DesiredLocation.Contains(searchTerm)) ||
                             (preference.AcceptableLocations != null && preference.AcceptableLocations.Contains(searchTerm)) ||
                             (preference.PreferencesAndCompromises != null && preference.PreferencesAndCompromises.Contains(searchTerm)))));
            }

            var statuses = CleanStatuses(filter.Statuses);
            if (statuses.Count > 0)
            {
                query = query.Where(lead => statuses.Contains(lead.WorkflowStatus));
            }

            var requestTypes = CleanValues(filter.RequestTypes);
            if (requestTypes.Count > 0)
            {
                query = query.Where(lead => requestTypes.Contains(lead.RequestType));
            }

            var priorities = CleanValues(filter.Priorities);
            if (priorities.Count > 0)
            {
                query = query.Where(lead =>
                    dbContext.LeadPreferences
                        .AsNoTracking()
                        .Any(preference =>
                            preference.LeadId == lead.Id &&
                            ((priorities.Contains("Da capire") &&
                              (string.IsNullOrWhiteSpace(preference.Timing) ||
                               preference.Timing == "Da capire")) ||
                             priorities.Contains(preference.Timing))) ||
                    (priorities.Contains("Da capire") &&
                     !dbContext.LeadPreferences
                         .AsNoTracking()
                         .Any(preference => preference.LeadId == lead.Id)));
            }

            var advisorIds = filter.AssignedAgencyUserIds ?? Array.Empty<Guid>();
            if (advisorIds.Count > 0 && filter.OnlyUnassigned)
            {
                query = query.Where(lead =>
                    lead.AssignedAgencyUserId == null ||
                    (lead.AssignedAgencyUserId.HasValue &&
                     advisorIds.Contains(lead.AssignedAgencyUserId.Value)));
            }
            else if (advisorIds.Count > 0)
            {
                query = query.Where(lead =>
                    lead.AssignedAgencyUserId.HasValue &&
                    advisorIds.Contains(lead.AssignedAgencyUserId.Value));
            }
            else if (filter.OnlyUnassigned)
            {
                query = query.Where(lead => lead.AssignedAgencyUserId == null);
            }

            return query;
        }

        private static IReadOnlyList<string> CleanStatuses(IReadOnlyList<string> statuses)
        {
            return CleanValues(statuses);
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
    }
}

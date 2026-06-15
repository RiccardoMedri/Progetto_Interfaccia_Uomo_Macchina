using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminDashboardQuery
    {
        private const int DashboardRowLimit = 6;

        private readonly MedriDbContext dbContext;

        public AdminDashboardQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminDashboardResultDto> ExecuteAsync(
            CancellationToken cancellationToken = default)
        {
            var newLeads = dbContext.Leads
                .AsNoTracking()
                .Where(lead =>
                    lead.InternalReference != null &&
                    lead.WorkflowStatus == LeadWorkflowStatuses.New);

            var recentLeads = dbContext.Leads
                .AsNoTracking()
                .Where(lead =>
                    lead.InternalReference != null &&
                    (lead.WorkflowStatus == LeadWorkflowStatuses.New || lead.WorkflowStatus == LeadWorkflowStatuses.InContact));

            var activeRequests = dbContext.SearchProfiles
                .AsNoTracking()
                .Where(profile =>
                    profile.PublicReference != null &&
                    profile.Status != RequestStatuses.Archived);

            var incompleteListings = dbContext.PropertyListings
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(listing =>
                    listing.InternalReference != null &&
                    listing.PublicationStatus == PropertyPublicationStatuses.Incomplete);

            var operationalListings = dbContext.PropertyListings
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(listing =>
                    listing.InternalReference != null &&
                    (listing.PublicationStatus == PropertyPublicationStatuses.Incomplete ||
                     listing.PublicationStatus == PropertyPublicationStatuses.NeedsUpdate ||
                     listing.PublicationStatus == PropertyPublicationStatuses.Ready));

            return new AdminDashboardResultDto
            {
                LeadWorkCount = await newLeads.CountAsync(cancellationToken),
                ActiveRequestCount = await activeRequests.CountAsync(cancellationToken),
                IncompleteListingCount = await incompleteListings.CountAsync(cancellationToken),
                ListingCount = await dbContext.PropertyListings
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .CountAsync(
                        listing => listing.InternalReference != null,
                        cancellationToken),
                RecentLeads = await recentLeads
                    .OrderByDescending(lead => lead.UpdatedAtUtc)
                    .ThenByDescending(lead => lead.CreatedAtUtc)
                    .Take(DashboardRowLimit)
                    .Select(lead => new AdminDashboardLeadDto
                    {
                        Reference = lead.InternalReference,
                        ContactName = lead.FullName,
                        RequestType = lead.RequestType,
                        AdvisorDisplayName = dbContext.AgencyUsers
                            .Where(advisor => advisor.Id == lead.AssignedAgencyUserId)
                            .Select(advisor => advisor.DisplayName)
                            .FirstOrDefault()
                    })
                    .ToArrayAsync(cancellationToken),
                ActiveRequests = await activeRequests
                    .Join(
                        dbContext.Leads.AsNoTracking(),
                        profile => profile.LeadId,
                        lead => lead.Id,
                        (profile, lead) => new
                        {
                            profile.PublicReference,
                            profile.Status,
                            profile.UpdatedAtUtc,
                            lead.FullName,
                            lead.AssignedAgencyUserId
                    })
                    .OrderByDescending(item => item.UpdatedAtUtc)
                    .Take(DashboardRowLimit)
                    .Select(item => new AdminDashboardRequestDto
                    {
                        Reference = item.PublicReference,
                        CustomerName = item.FullName,
                        Status = item.Status,
                        AdvisorDisplayName = dbContext.AgencyUsers
                            .Where(advisor => advisor.Id == item.AssignedAgencyUserId)
                            .Select(advisor => advisor.DisplayName)
                            .FirstOrDefault()
                    })
                    .ToArrayAsync(cancellationToken),
                Listings = await operationalListings
                    .OrderByDescending(listing => listing.UpdatedAtUtc)
                    .ThenBy(listing => listing.SortOrder)
                    .ThenBy(listing => listing.InternalReference)
                    .Take(DashboardRowLimit)
                    .Select(listing => new AdminDashboardListingDto
                    {
                        Reference = listing.InternalReference,
                        Title = listing.Title,
                        Status = listing.PublicationStatus,
                        AdvisorDisplayName = dbContext.AgencyUsers
                            .Where(advisor => advisor.Id == listing.AssignedAgencyUserId)
                            .Select(advisor => advisor.DisplayName)
                            .FirstOrDefault()
                    })
                    .ToArrayAsync(cancellationToken)
            };
        }
    }
}

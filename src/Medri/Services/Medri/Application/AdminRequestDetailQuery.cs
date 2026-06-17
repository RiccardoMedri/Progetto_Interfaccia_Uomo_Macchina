using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminRequestDetailQuery
    {
        private readonly MedriDbContext dbContext;

        public AdminRequestDetailQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminRequestDetailResultDto> CreateNewAsync(
            CancellationToken cancellationToken = default)
        {
            return new AdminRequestDetailResultDto
            {
                IsCreateMode = true,
                Reference = "Nuova richiesta",
                Status = RequestStatuses.New,
                FullName = "Nuova richiesta",
                SourceChannel = "Telefono",
                RequestType = RequestTypes.Buy,
                LeadCount = await dbContext.Leads
                    .AsNoTracking()
                    .CountAsync(
                        item => item.InternalReference != null && item.WorkflowStatus == LeadWorkflowStatuses.New,
                        cancellationToken),
                ActiveRequestCount = await dbContext.SearchProfiles
                    .AsNoTracking()
                    .CountAsync(
                        item => item.PublicReference != null && item.Status != RequestStatuses.Archived,
                        cancellationToken),
                ListingCount = await AdminNavigationCounts.UnpublishedListingsAsync(dbContext, cancellationToken),
                Advisors = await dbContext.AgencyUsers
                    .AsNoTracking()
                    .Where(advisor => !advisor.IsSystemSeed)
                    .OrderBy(advisor => advisor.DisplayName)
                    .Select(advisor => new AdminLeadAdvisorDto
                    {
                        Id = advisor.Id,
                        DisplayName = advisor.DisplayName
                    })
                    .ToArrayAsync(cancellationToken)
            };
        }

        public async Task<AdminRequestDetailResultDto> ExecuteAsync(
            string reference,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return null;
            }

            var profile = await dbContext.SearchProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    item => item.PublicReference == reference,
                    cancellationToken);

            if (profile == null)
            {
                return null;
            }

            var lead = await dbContext.Leads
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    item => item.Id == profile.LeadId,
                    cancellationToken);

            if (lead == null)
            {
                return null;
            }

            var preference = await dbContext.LeadPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    item => item.LeadId == lead.Id,
                    cancellationToken);

            var result = new AdminRequestDetailResultDto
            {
                SearchProfileId = profile.Id,
                LeadId = lead.Id,
                Reference = profile.PublicReference,
                Status = profile.Status,
                CriteriaSummary = profile.CriteriaSummary,
                SourceQueryString = profile.SourceQueryString,
                UpdatedAtUtc = profile.UpdatedAtUtc,
                FullName = lead.FullName,
                Email = lead.Email,
                Phone = lead.Phone,
                SourceChannel = lead.SourceChannel,
                RequestType = lead.RequestType,
                LeadNotes = lead.Notes,
                AssignedAgencyUserId = lead.AssignedAgencyUserId,
                LinkedLeadReference = LinkedLeadReference(profile.SourceQueryString),
                MaximumPrice = preference?.MaximumPrice,
                MaximumBudgetLabel = preference?.SustainableBudgetLabel,
                PreferredBudgetLabel = preference?.ExpectedPriceOrMainQuestion,
                DesiredLocation = preference?.DesiredLocation,
                AcceptableLocations = preference?.AcceptableLocations,
                MinimumRooms = preference?.MinimumRooms,
                AccessibilityConstraint = preference?.Appurtenances,
                Priority = preference?.Timing,
                TimeFrame = preference?.DesiredMoveIn,
                FinancingStatus = preference?.FinancingStatus,
                PropertyToSellStatus = preference?.PropertyToSellStatus,
                SummaryNotes = preference?.SearchStage,
                NeedsAfterContact = preference?.HouseholdDescription,
                DesiredPreferenceTags = preference?.PreferencesAndCompromises,
                NegotiablePreferenceTags = preference?.PropertyCondition
            };

            result.LeadCount = await dbContext.Leads
                .AsNoTracking()
                .CountAsync(
                    item => item.InternalReference != null && item.WorkflowStatus == LeadWorkflowStatuses.New,
                    cancellationToken);
            result.ActiveRequestCount = await dbContext.SearchProfiles
                .AsNoTracking()
                .CountAsync(
                    item => item.PublicReference != null && item.Status != RequestStatuses.Archived,
                    cancellationToken);
            result.ListingCount = await AdminNavigationCounts.UnpublishedListingsAsync(dbContext, cancellationToken);
            result.Advisors = await dbContext.AgencyUsers
                .AsNoTracking()
                .Where(advisor => !advisor.IsSystemSeed)
                .OrderBy(advisor => advisor.DisplayName)
                .Select(advisor => new AdminLeadAdvisorDto
                {
                    Id = advisor.Id,
                    DisplayName = advisor.DisplayName
                })
                .ToArrayAsync(cancellationToken);
            result.Interactions = await dbContext.Interactions
                .AsNoTracking()
                .Where(interaction => interaction.LeadId == lead.Id)
                .OrderByDescending(interaction => interaction.OccurredAtUtc)
                .Select(interaction => new AdminLeadInteractionDto
                {
                    Channel = interaction.Channel,
                    Notes = interaction.Notes,
                    OccurredAtUtc = interaction.OccurredAtUtc
                })
                .ToArrayAsync(cancellationToken);

            return result;
        }

        private static string LinkedLeadReference(string sourceQueryString)
        {
            const string Prefix = "da lead ";

            if (string.IsNullOrWhiteSpace(sourceQueryString) ||
                !sourceQueryString.StartsWith(Prefix, System.StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return sourceQueryString.Substring(Prefix.Length).Trim();
        }
    }
}

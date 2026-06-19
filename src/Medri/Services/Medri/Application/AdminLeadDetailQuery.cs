using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminLeadDetailQuery
    {
        private readonly MedriDbContext dbContext;

        public AdminLeadDetailQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminLeadDetailResultDto> CreateNewAsync(
            CancellationToken cancellationToken = default)
        {
            var adminLeads = dbContext.Leads
                .AsNoTracking()
                .Where(item => item.InternalReference != null);

            return new AdminLeadDetailResultDto
            {
                IsCreateMode = true,
                Reference = "Nuovo lead",
                WorkflowStatus = LeadWorkflowStatuses.New,
                FullName = "Nuovo lead",
                SourceChannel = "Telefono",
                RequestType = RequestTypes.Buy,
                CreatedAtUtc = DateTime.UtcNow,
                NewCount = await adminLeads.CountAsync(
                    item => item.WorkflowStatus == LeadWorkflowStatuses.New,
                    cancellationToken),
                ActiveRequestCount = await dbContext.SearchProfiles
                    .AsNoTracking()
                    .CountAsync(
                        profile => profile.PublicReference != null && profile.Status != RequestStatuses.Archived,
                        cancellationToken),
                ListingCount = await AdminNavigationCounts.UnpublishedListingsAsync(dbContext, cancellationToken),
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

        public async Task<AdminLeadDetailResultDto> ExecuteAsync(
            string reference,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return null;
            }

            var leadEntity = await dbContext.Leads
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    item => item.InternalReference == reference,
                    cancellationToken);

            if (leadEntity == null)
            {
                return null;
            }

            var preference = await dbContext.LeadPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    item => item.LeadId == leadEntity.Id,
                    cancellationToken);
            var searchProfile = await dbContext.SearchProfiles
                .AsNoTracking()
                .Where(profile => profile.LeadId == leadEntity.Id)
                .OrderByDescending(profile => profile.UpdatedAtUtc)
                .FirstOrDefaultAsync(cancellationToken);

            var lead = new AdminLeadDetailResultDto
            {
                Id = leadEntity.Id,
                Reference = leadEntity.InternalReference,
                WorkflowStatus = leadEntity.WorkflowStatus,
                QualificationPercent = leadEntity.QualificationPercent,
                FullName = leadEntity.FullName,
                Email = leadEntity.Email,
                Phone = leadEntity.Phone,
                SourceChannel = leadEntity.SourceChannel,
                RequestType = leadEntity.RequestType,
                Notes = leadEntity.Notes,
                NextAction = leadEntity.NextAction,
                AssignedAgencyUserId = leadEntity.AssignedAgencyUserId,
                CreatedAtUtc = leadEntity.CreatedAtUtc,
                Urgency = preference?.Timing,
                DesiredLocation = preference?.DesiredLocation,
                ExpectedPriceOrMainQuestion = preference?.ExpectedPriceOrMainQuestion,
                LinkedPropertyReference = preference?.LinkedPropertyReference,
                ContactReason = preference?.ValuationGoal,
                NextContactQuestions = preference?.PreferencesAndCompromises,
                SearchProfileReference = searchProfile?.PublicReference
            };

            var adminLeads = dbContext.Leads
                .AsNoTracking()
                .Where(item => item.InternalReference != null);

            lead.NewCount = await adminLeads.CountAsync(
                item => item.WorkflowStatus == LeadWorkflowStatuses.New,
                cancellationToken);
            lead.ActiveRequestCount = await dbContext.SearchProfiles
                .AsNoTracking()
                .CountAsync(
                    profile => profile.PublicReference != null && profile.Status != RequestStatuses.Archived,
                    cancellationToken);
            lead.ListingCount = await AdminNavigationCounts.UnpublishedListingsAsync(dbContext, cancellationToken);
            lead.Advisors = await StaffUserQueries.Assignable(dbContext)
                .OrderBy(advisor => advisor.DisplayName)
                .Select(advisor => new AdminLeadAdvisorDto
                {
                    Id = advisor.Id,
                    DisplayName = advisor.DisplayName
                })
                .ToArrayAsync(cancellationToken);
            lead.Interactions = await dbContext.Interactions
                .AsNoTracking()
                .Where(interaction => interaction.LeadId == lead.Id)
                .OrderBy(interaction => interaction.OccurredAtUtc)
                .Select(interaction => new AdminLeadInteractionDto
                {
                    Channel = interaction.Channel,
                    Notes = interaction.Notes,
                    OccurredAtUtc = interaction.OccurredAtUtc
                })
                .ToArrayAsync(cancellationToken);

            return lead;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class UpdateAdminRequestDetailCommand
    {
        private readonly MedriDbContext dbContext;

        public UpdateAdminRequestDetailCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminRequestCommandResult> ExecuteAsync(
            string reference,
            AdminRequestDetailUpdateDto input,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return AdminRequestCommandResult.NotFound(reference);
            }

            input ??= new AdminRequestDetailUpdateDto();

            var profile = await dbContext.SearchProfiles
                .FirstOrDefaultAsync(
                    item => item.PublicReference == reference,
                    cancellationToken);

            if (profile == null)
            {
                return AdminRequestCommandResult.NotFound(reference);
            }

            var lead = await dbContext.Leads
                .FirstOrDefaultAsync(
                    item => item.Id == profile.LeadId,
                    cancellationToken);

            if (lead == null)
            {
                return AdminRequestCommandResult.NotFound(reference);
            }

            lead.FullName = ApplicationText.FullName(input.FirstName, input.LastName, lead.FullName);
            lead.Phone = ApplicationText.Clean(input.Phone);
            lead.Email = ApplicationText.Clean(input.Email);
            lead.SourceChannel = ApplicationText.Clean(input.SourceChannel) ?? lead.SourceChannel;
            lead.AssignedAgencyUserId = input.AssignedAgencyUserId;
            lead.RequestType = ApplicationText.Clean(input.RequestType) ?? lead.RequestType;
            lead.UpdatedAtUtc = DateTime.UtcNow;

            var status = ApplicationText.Clean(input.Status);
            if (IsRequestStatus(status))
            {
                profile.Status = status;
            }

            var preference = await dbContext.LeadPreferences
                .FirstOrDefaultAsync(
                    item => item.LeadId == lead.Id,
                    cancellationToken);

            if (preference == null)
            {
                preference = new LeadPreference
                {
                    Id = Guid.NewGuid(),
                    LeadId = lead.Id
                };
                dbContext.LeadPreferences.Add(preference);
            }

            preference.SustainableBudgetLabel = ApplicationText.Clean(input.MaximumBudgetLabel);
            preference.ExpectedPriceOrMainQuestion = ApplicationText.Clean(input.PreferredBudgetLabel);
            preference.DesiredLocation = ApplicationText.Clean(input.DesiredLocation);
            preference.AcceptableLocations = ApplicationText.Clean(input.AcceptableLocations);
            preference.MinimumRooms = input.MinimumRooms;
            preference.Appurtenances = ApplicationText.Clean(input.AccessibilityConstraint);
            preference.DesiredMoveIn = ApplicationText.Clean(input.TimeFrame);
            preference.FinancingStatus = ApplicationText.Clean(input.FinancingStatus);
            preference.PropertyToSellStatus = ApplicationText.Clean(input.PropertyToSellStatus);
            preference.SearchStage = ApplicationText.Clean(input.SummaryNotes);
            preference.HouseholdDescription = ApplicationText.Clean(input.NeedsAfterContact);
            preference.PreferencesAndCompromises = ApplicationText.NormalizeTags(input.DesiredPreferenceTagsText);
            preference.PropertyCondition = ApplicationText.NormalizeTags(input.NegotiablePreferenceTagsText);

            profile.UpdatedAtUtc = lead.UpdatedAtUtc;

            dbContext.Interactions.Add(new Interaction
            {
                Id = Guid.NewGuid(),
                LeadId = lead.Id,
                Channel = "Preferenze aggiornate",
                Notes = "Richiesta aggiornata dall'area Admin.",
                OccurredAtUtc = profile.UpdatedAtUtc
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminRequestCommandResult.Success(profile.PublicReference);
        }

        private static bool IsRequestStatus(string value)
        {
            return value == RequestStatuses.New ||
                value == RequestStatuses.Updating ||
                value == RequestStatuses.InMatching ||
                value == RequestStatuses.Archived;
        }
    }
}

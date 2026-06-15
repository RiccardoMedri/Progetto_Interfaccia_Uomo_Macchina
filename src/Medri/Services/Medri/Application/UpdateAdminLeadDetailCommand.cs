using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class UpdateAdminLeadDetailCommand
    {
        private readonly MedriDbContext dbContext;

        public UpdateAdminLeadDetailCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminLeadCommandResult> ExecuteAsync(
            string reference,
            AdminLeadDetailUpdateDto input,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return AdminLeadCommandResult.NotFound(reference);
            }

            input ??= new AdminLeadDetailUpdateDto();

            var lead = await dbContext.Leads
                .FirstOrDefaultAsync(
                    item => item.InternalReference == reference,
                    cancellationToken);

            if (lead == null)
            {
                return AdminLeadCommandResult.NotFound(reference);
            }

            var changedFields = new System.Collections.Generic.List<string>();

            SetIfChanged(
                changedFields,
                "Nome cliente",
                lead.FullName,
                ApplicationText.FullName(input.FirstName, input.LastName, lead.FullName),
                value => lead.FullName = value);
            SetIfChanged(
                changedFields,
                "Telefono",
                lead.Phone,
                ApplicationText.Clean(input.Phone),
                value => lead.Phone = value);
            SetIfChanged(
                changedFields,
                "Email",
                lead.Email,
                ApplicationText.Clean(input.Email),
                value => lead.Email = value);
            SetIfChanged(
                changedFields,
                "Fonte",
                lead.SourceChannel,
                ApplicationText.Clean(input.SourceChannel) ?? lead.SourceChannel,
                value => lead.SourceChannel = value);
            SetIfChanged(
                changedFields,
                "Referente",
                lead.AssignedAgencyUserId,
                input.AssignedAgencyUserId,
                value => lead.AssignedAgencyUserId = value);
            SetIfChanged(
                changedFields,
                "Esigenza",
                lead.RequestType,
                ApplicationText.Clean(input.RequestType),
                value => lead.RequestType = value);
            SetIfChanged(
                changedFields,
                "Note",
                lead.Notes,
                ApplicationText.Clean(input.Notes),
                value => lead.Notes = value);

            var preference = await dbContext.LeadPreferences
                .FirstOrDefaultAsync(
                    item => item.LeadId == lead.Id,
                    cancellationToken);

            if (preference == null && HasPreferenceData(input))
            {
                preference = new LeadPreference
                {
                    Id = Guid.NewGuid(),
                    LeadId = lead.Id
                };
                dbContext.LeadPreferences.Add(preference);
            }

            if (preference != null)
            {
                SetIfChanged(
                    changedFields,
                    "Urgenza",
                    preference.Timing,
                    ApplicationText.Clean(input.Urgency),
                    value => preference.Timing = value);
                SetIfChanged(
                    changedFields,
                    "Comune / zona",
                    preference.DesiredLocation,
                    ApplicationText.Clean(input.DesiredLocation),
                    value => preference.DesiredLocation = value);
                SetIfChanged(
                    changedFields,
                    "Budget / valore atteso",
                    preference.ExpectedPriceOrMainQuestion,
                    ApplicationText.Clean(input.ExpectedPriceOrMainQuestion),
                    value => preference.ExpectedPriceOrMainQuestion = value);
                SetIfChanged(
                    changedFields,
                    "Immobile collegato",
                    preference.LinkedPropertyReference,
                    ApplicationText.Clean(input.LinkedPropertyReference),
                    value => preference.LinkedPropertyReference = value);
                SetIfChanged(
                    changedFields,
                    "Motivo contatto",
                    preference.ValuationGoal,
                    ApplicationText.Clean(input.ContactReason),
                    value => preference.ValuationGoal = value);
                SetIfChanged(
                    changedFields,
                    "Da chiedere al prossimo contatto",
                    preference.PreferencesAndCompromises,
                    ApplicationText.Clean(input.NextContactQuestions),
                    value => preference.PreferencesAndCompromises = value);
            }

            var nextContactQuestions = ApplicationText.Clean(input.NextContactQuestions);
            if (!string.IsNullOrWhiteSpace(nextContactQuestions) &&
                !StringEquals(lead.NextAction, nextContactQuestions))
            {
                lead.NextAction = nextContactQuestions;
            }

            if (changedFields.Count == 0)
            {
                return AdminLeadCommandResult.Success(lead.InternalReference, changedFields);
            }

            var now = DateTime.UtcNow;
            if (lead.WorkflowStatus == LeadWorkflowStatuses.New)
            {
                lead.WorkflowStatus = LeadWorkflowStatuses.InContact;
                changedFields.Add("Stato");
            }

            if (lead.WorkflowStatus != LeadWorkflowStatuses.Qualified)
            {
                lead.QualificationPercent = LeadQualificationCalculator.Calculate(lead, preference);
            }

            lead.UpdatedAtUtc = now;

            dbContext.Interactions.Add(new Interaction
            {
                Id = Guid.NewGuid(),
                LeadId = lead.Id,
                Channel = "Aggiornamento scheda lead",
                Notes = $"Campi aggiornati: {string.Join(", ", changedFields)}.",
                OccurredAtUtc = now
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminLeadCommandResult.Success(lead.InternalReference, changedFields);
        }

        private static void SetIfChanged(
            System.Collections.Generic.ICollection<string> changedFields,
            string label,
            string current,
            string next,
            Action<string> setValue)
        {
            if (StringEquals(current, next))
            {
                return;
            }

            setValue(next);
            changedFields.Add(label);
        }

        private static void SetIfChanged(
            System.Collections.Generic.ICollection<string> changedFields,
            string label,
            Guid? current,
            Guid? next,
            Action<Guid?> setValue)
        {
            if (current == next)
            {
                return;
            }

            setValue(next);
            changedFields.Add(label);
        }

        private static bool StringEquals(string current, string next)
        {
            return string.Equals(
                ApplicationText.Clean(current),
                ApplicationText.Clean(next),
                StringComparison.Ordinal);
        }

        private static bool HasPreferenceData(AdminLeadDetailUpdateDto input)
        {
            return !string.IsNullOrWhiteSpace(input.Urgency) ||
                !string.IsNullOrWhiteSpace(input.DesiredLocation) ||
                !string.IsNullOrWhiteSpace(input.ExpectedPriceOrMainQuestion) ||
                !string.IsNullOrWhiteSpace(input.LinkedPropertyReference) ||
                !string.IsNullOrWhiteSpace(input.ContactReason) ||
                !string.IsNullOrWhiteSpace(input.NextContactQuestions);
        }

    }
}

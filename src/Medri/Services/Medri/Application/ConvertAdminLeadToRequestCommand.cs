using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class ConvertAdminLeadToRequestCommand
    {
        private readonly MedriDbContext dbContext;

        public ConvertAdminLeadToRequestCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminLeadCommandResult> ExecuteAsync(
            string reference,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return AdminLeadCommandResult.NotFound(reference);
            }

            var lead = await dbContext.Leads
                .FirstOrDefaultAsync(
                    item => item.InternalReference == reference,
                    cancellationToken);

            if (lead == null)
            {
                return AdminLeadCommandResult.NotFound(reference);
            }

            if (lead.WorkflowStatus == LeadWorkflowStatuses.Archived)
            {
                return AdminLeadCommandResult.NotFound(reference);
            }

            if (lead.WorkflowStatus == LeadWorkflowStatuses.Qualified)
            {
                return AdminLeadCommandResult.Success(lead.InternalReference);
            }

            var searchProfile = await dbContext.SearchProfiles
                .FirstOrDefaultAsync(
                    item => item.LeadId == lead.Id && item.PublicReference != null,
                    cancellationToken);

            var now = DateTime.UtcNow;

            if (searchProfile == null)
            {
                searchProfile = new SearchProfile
                {
                    Id = Guid.NewGuid(),
                    LeadId = lead.Id,
                    PublicReference = await AdminReferenceGenerator.NextRequestReferenceAsync(
                        dbContext,
                        cancellationToken,
                        defaultNumber: 2047),
                    Status = RequestStatuses.New,
                    CriteriaSummary = await CriteriaSummaryAsync(lead.Id, lead.RequestType, cancellationToken),
                    ContactEmail = lead.Email,
                    SourceQueryString = "",
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                };
                dbContext.SearchProfiles.Add(searchProfile);
            }
            else
            {
                searchProfile.Status = string.IsNullOrWhiteSpace(searchProfile.Status)
                    ? RequestStatuses.New
                    : searchProfile.Status;
                searchProfile.UpdatedAtUtc = now;
            }

            lead.WorkflowStatus = LeadWorkflowStatuses.Qualified;
            lead.QualificationPercent = 100;
            lead.UpdatedAtUtc = now;

            dbContext.Interactions.Add(new Interaction
            {
                Id = Guid.NewGuid(),
                LeadId = lead.Id,
                Channel = "Conversione",
                Notes = $"Lead convertito in richiesta strutturata {searchProfile.PublicReference}.",
                OccurredAtUtc = now
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminLeadCommandResult.Success(
                lead.InternalReference,
                new[] { "Stato", "Dati utili", "Richiesta" });
        }

        private async Task<string> CriteriaSummaryAsync(
            Guid leadId,
            string requestType,
            CancellationToken cancellationToken)
        {
            var preference = await dbContext.LeadPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    item => item.LeadId == leadId,
                    cancellationToken);

            var need = RequestTypeLabel(requestType);
            var place = string.IsNullOrWhiteSpace(preference?.DesiredLocation)
                ? "zona da completare"
                : preference.DesiredLocation;
            var reason = string.IsNullOrWhiteSpace(preference?.ValuationGoal)
                ? "motivo richiesta da completare"
                : preference.ValuationGoal;

            return $"{need} - {place} - {reason}";
        }

        private static string RequestTypeLabel(string requestType)
        {
            return requestType switch
            {
                RequestTypes.Valuation => "Valutazione immobile",
                RequestTypes.Rent => "Affitto casa",
                RequestTypes.Sell => "Vendita casa",
                RequestTypes.RentOut => "Affitto immobile",
                _ => "Acquisto casa"
            };
        }
    }
}

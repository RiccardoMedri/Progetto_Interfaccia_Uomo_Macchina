using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class RestoreAdminLeadCommand
    {
        private readonly MedriDbContext dbContext;

        public RestoreAdminLeadCommand(MedriDbContext dbContext)
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

            if (lead.WorkflowStatus != LeadWorkflowStatuses.Archived)
            {
                return AdminLeadCommandResult.Success(reference);
            }

            var now = DateTime.UtcNow;
            lead.WorkflowStatus = LeadWorkflowStatuses.InContact;
            lead.NextAction = string.IsNullOrWhiteSpace(lead.NextAction)
                ? "Riprendere contatto"
                : lead.NextAction;
            lead.UpdatedAtUtc = now;

            dbContext.Interactions.Add(new Interaction
            {
                Id = Guid.NewGuid(),
                LeadId = lead.Id,
                Channel = "Ripristino",
                Notes = "Lead ripristinato dall'archivio e riportato in lavorazione.",
                OccurredAtUtc = now
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminLeadCommandResult.Success(
                lead.InternalReference,
                new[] { "Stato" });
        }
    }
}

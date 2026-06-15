using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class ArchiveAdminLeadCommand
    {
        private readonly MedriDbContext dbContext;

        public ArchiveAdminLeadCommand(MedriDbContext dbContext)
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

            var now = DateTime.UtcNow;

            if (lead.WorkflowStatus == LeadWorkflowStatuses.Archived)
            {
                return AdminLeadCommandResult.Success(lead.InternalReference);
            }

            lead.WorkflowStatus = LeadWorkflowStatuses.Archived;
            lead.UpdatedAtUtc = now;

            dbContext.Interactions.Add(new Interaction
            {
                Id = Guid.NewGuid(),
                LeadId = lead.Id,
                Channel = "Archiviazione",
                Notes = "Lead archiviato dall'area Admin.",
                OccurredAtUtc = now
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminLeadCommandResult.Success(
                lead.InternalReference,
                new[] { "Stato" });
        }
    }
}

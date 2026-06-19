using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class BulkAssignAdminLeadsCommand
    {
        private readonly MedriDbContext dbContext;

        public BulkAssignAdminLeadsCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminLeadBulkCommandResult> ExecuteAsync(
            IEnumerable<string> references,
            Guid? assignedAgencyUserId,
            CancellationToken cancellationToken = default)
        {
            var selectedReferences = ApplicationText.CleanReferences(references);
            if (selectedReferences.Count == 0 || !assignedAgencyUserId.HasValue)
            {
                return AdminLeadBulkCommandResult.Empty();
            }

            var advisorExists = await StaffUserQueries.Assignable(dbContext)
                .AnyAsync(
                    advisor => advisor.Id == assignedAgencyUserId.Value,
                    cancellationToken);

            if (!advisorExists)
            {
                return AdminLeadBulkCommandResult.Empty();
            }

            var leads = await dbContext.Leads
                .Where(lead =>
                    lead.InternalReference != null &&
                    selectedReferences.Contains(lead.InternalReference) &&
                    lead.WorkflowStatus != LeadWorkflowStatuses.Archived &&
                    lead.WorkflowStatus != LeadWorkflowStatuses.Qualified &&
                    lead.AssignedAgencyUserId != assignedAgencyUserId)
                .ToArrayAsync(cancellationToken);

            if (leads.Length == 0)
            {
                return AdminLeadBulkCommandResult.Empty();
            }

            var leadIds = leads
                .Select(lead => lead.Id)
                .ToArray();
            var preferences = await dbContext.LeadPreferences
                .AsNoTracking()
                .Where(preference => leadIds.Contains(preference.LeadId))
                .ToDictionaryAsync(
                    preference => preference.LeadId,
                    cancellationToken);
            var now = DateTime.UtcNow;
            foreach (var lead in leads)
            {
                lead.AssignedAgencyUserId = assignedAgencyUserId;
                if (lead.WorkflowStatus != LeadWorkflowStatuses.Qualified)
                {
                    preferences.TryGetValue(lead.Id, out var preference);
                    lead.QualificationPercent = LeadQualificationCalculator.Calculate(lead, preference);
                }

                lead.UpdatedAtUtc = now;

                dbContext.Interactions.Add(new Interaction
                {
                    Id = Guid.NewGuid(),
                    LeadId = lead.Id,
                    Channel = "Assegnazione",
                    Notes = "Referente aggiornato da selezione multipla.",
                    OccurredAtUtc = now
                });
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminLeadBulkCommandResult.Success(leads.Length);
        }

    }

    public sealed class BulkConvertAdminLeadsCommand
    {
        private readonly ConvertAdminLeadToRequestCommand convertCommand;

        public BulkConvertAdminLeadsCommand(ConvertAdminLeadToRequestCommand convertCommand)
        {
            this.convertCommand = convertCommand;
        }

        public async Task<AdminLeadBulkCommandResult> ExecuteAsync(
            IEnumerable<string> references,
            CancellationToken cancellationToken = default)
        {
            var selectedReferences = ApplicationText.CleanReferences(references);
            var updatedCount = 0;

            foreach (var reference in selectedReferences)
            {
                var result = await convertCommand.ExecuteAsync(reference, cancellationToken);
                if (result.HasChanges)
                {
                    updatedCount++;
                }
            }

            return AdminLeadBulkCommandResult.Success(updatedCount);
        }

    }

    public sealed class BulkArchiveAdminLeadsCommand
    {
        private readonly ArchiveAdminLeadCommand archiveCommand;

        public BulkArchiveAdminLeadsCommand(ArchiveAdminLeadCommand archiveCommand)
        {
            this.archiveCommand = archiveCommand;
        }

        public async Task<AdminLeadBulkCommandResult> ExecuteAsync(
            IEnumerable<string> references,
            CancellationToken cancellationToken = default)
        {
            var selectedReferences = ApplicationText.CleanReferences(references);
            var updatedCount = 0;

            foreach (var reference in selectedReferences)
            {
                var result = await archiveCommand.ExecuteAsync(reference, cancellationToken);
                if (result.HasChanges)
                {
                    updatedCount++;
                }
            }

            return AdminLeadBulkCommandResult.Success(updatedCount);
        }

    }

    public sealed class AdminLeadBulkCommandResult
    {
        public int UpdatedCount { get; set; }

        public bool HasUpdates => UpdatedCount > 0;

        public static AdminLeadBulkCommandResult Success(int updatedCount)
        {
            return new AdminLeadBulkCommandResult
            {
                UpdatedCount = updatedCount
            };
        }

        public static AdminLeadBulkCommandResult Empty()
        {
            return new AdminLeadBulkCommandResult();
        }
    }
}

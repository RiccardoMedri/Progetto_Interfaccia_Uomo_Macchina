using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class BulkAssignAdminRequestsCommand
    {
        private readonly MedriDbContext dbContext;

        public BulkAssignAdminRequestsCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminRequestBulkCommandResult> ExecuteAsync(
            IEnumerable<string> references,
            Guid? assignedAgencyUserId,
            CancellationToken cancellationToken = default)
        {
            var selectedReferences = ApplicationText.CleanReferences(references);
            if (selectedReferences.Count == 0 || !assignedAgencyUserId.HasValue)
            {
                return AdminRequestBulkCommandResult.Empty();
            }

            var advisorExists = await StaffUserQueries.Assignable(dbContext)
                .AnyAsync(
                    advisor => advisor.Id == assignedAgencyUserId.Value,
                    cancellationToken);

            if (!advisorExists)
            {
                return AdminRequestBulkCommandResult.Empty();
            }

            var profiles = await dbContext.SearchProfiles
                .Where(profile =>
                    profile.PublicReference != null &&
                    selectedReferences.Contains(profile.PublicReference) &&
                    profile.Status != RequestStatuses.Archived)
                .ToArrayAsync(cancellationToken);

            var leadIds = profiles
                .Select(profile => profile.LeadId)
                .ToHashSet();
            var leads = await dbContext.Leads
                .Where(lead =>
                    leadIds.Contains(lead.Id) &&
                    lead.AssignedAgencyUserId != assignedAgencyUserId.Value)
                .ToArrayAsync(cancellationToken);

            if (leads.Length == 0)
            {
                return AdminRequestBulkCommandResult.Empty();
            }

            var changedLeadIds = leads
                .Select(lead => lead.Id)
                .ToArray();
            var now = DateTime.UtcNow;
            foreach (var lead in leads)
            {
                lead.AssignedAgencyUserId = assignedAgencyUserId;
                lead.UpdatedAtUtc = now;

                dbContext.Interactions.Add(new Interaction
                {
                    Id = Guid.NewGuid(),
                    LeadId = lead.Id,
                    Channel = "Assegnazione richiesta",
                    Notes = "Referente richiesta aggiornato da selezione multipla.",
                    OccurredAtUtc = now
                });
            }

            foreach (var profile in profiles.Where(profile => changedLeadIds.Contains(profile.LeadId)))
            {
                profile.UpdatedAtUtc = now;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminRequestBulkCommandResult.Success(leads.Length);
        }

    }

    public sealed class AdminRequestBulkCommandResult
    {
        public int UpdatedCount { get; set; }

        public bool HasUpdates => UpdatedCount > 0;

        public static AdminRequestBulkCommandResult Success(int updatedCount)
        {
            return new AdminRequestBulkCommandResult
            {
                UpdatedCount = updatedCount
            };
        }

        public static AdminRequestBulkCommandResult Empty()
        {
            return new AdminRequestBulkCommandResult();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class ArchiveAdminRequestCommand
    {
        private readonly MedriDbContext dbContext;

        public ArchiveAdminRequestCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminRequestCommandResult> ExecuteAsync(
            string reference,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return AdminRequestCommandResult.NotFound(reference);
            }

            var profile = await dbContext.SearchProfiles
                .FirstOrDefaultAsync(
                    item => item.PublicReference == reference,
                    cancellationToken);

            if (profile == null)
            {
                return AdminRequestCommandResult.NotFound(reference);
            }

            profile.Status = RequestStatuses.Archived;
            profile.UpdatedAtUtc = DateTime.UtcNow;

            dbContext.Interactions.Add(new Interaction
            {
                Id = Guid.NewGuid(),
                LeadId = profile.LeadId,
                Channel = "Richiesta archiviata",
                Notes = "Profilo richiesta rimosso dalla lista attiva Admin.",
                OccurredAtUtc = profile.UpdatedAtUtc
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminRequestCommandResult.Success(profile.PublicReference);
        }
    }
}

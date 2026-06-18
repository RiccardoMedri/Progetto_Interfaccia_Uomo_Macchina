using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class ClaimClientRequestsCommand
    {
        private readonly MedriDbContext dbContext;

        public ClaimClientRequestsCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task ExecuteAsync(
            Guid userId,
            IEnumerable<Guid> requestIds,
            CancellationToken cancellationToken = default)
        {
            var normalizedRequestIds = requestIds?
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray() ?? Array.Empty<Guid>();

            if (normalizedRequestIds.Length == 0)
            {
                return;
            }

            var hasChanges = false;
            foreach (var requestId in normalizedRequestIds)
            {
                var isSubmitted = await dbContext.Appointments
                    .AsNoTracking()
                    .AnyAsync(
                        appointment =>
                            appointment.LeadId == requestId &&
                            appointment.Status != AppointmentStatuses.Draft,
                        cancellationToken);
                if (!isSubmitted)
                {
                    continue;
                }

                var lead = await dbContext.Leads
                    .Where(item =>
                        item.Id == requestId &&
                        (!item.ClientUserId.HasValue || item.ClientUserId == userId))
                    .FirstOrDefaultAsync(cancellationToken);
                if (lead == null || lead.ClientUserId.HasValue)
                {
                    continue;
                }

                lead.ClientUserId = userId;
                hasChanges = true;
            }

            if (hasChanges)
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

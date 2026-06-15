using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class ClientRequestCountQuery
    {
        private readonly MedriDbContext dbContext;

        public ClientRequestCountQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<int> ExecuteAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return dbContext.Leads
                .AsNoTracking()
                .Where(lead => lead.ClientUserId == userId)
                .Join(
                    dbContext.Appointments
                        .AsNoTracking()
                        .Where(appointment => appointment.Status != AppointmentStatuses.Draft),
                    lead => lead.Id,
                    appointment => appointment.LeadId,
                    (lead, _) => lead.Id)
                .Distinct()
                .CountAsync(cancellationToken);
        }
    }
}

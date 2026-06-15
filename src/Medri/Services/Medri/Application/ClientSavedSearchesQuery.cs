using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class ClientSavedSearchDto
    {
        public Guid Id { get; set; }

        public string Label { get; set; }

        public string QueryString { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }

    public sealed class ClientSavedSearchesQuery
    {
        private readonly MedriDbContext dbContext;

        public ClientSavedSearchesQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IReadOnlyList<ClientSavedSearchDto>> ExecuteAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await dbContext.ClientSavedSearches
                .AsNoTracking()
                .Where(saved => saved.UserId == userId)
                .OrderByDescending(saved => saved.CreatedAtUtc)
                .Select(saved => new ClientSavedSearchDto
                {
                    Id = saved.Id,
                    Label = saved.Label,
                    QueryString = saved.QueryString,
                    CreatedAtUtc = saved.CreatedAtUtc
                })
                .ToListAsync(cancellationToken);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Medri.Services.Medri;

namespace Medri.Services.Medri.Application
{
    public sealed class ClientRequestsQuery
    {
        private const int DefaultPage = 1;
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 25;

        private readonly MedriDbContext dbContext;

        public ClientRequestsQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ClientRequestsResultDto> ExecuteAsync(
            Guid userId,
            int page = DefaultPage,
            int pageSize = DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            var normalizedPageSize = NormalizePageSize(pageSize);
            var normalizedPage = Math.Max(DefaultPage, page);

            var clientLeads = dbContext.Leads
                .AsNoTracking()
                .Where(lead => lead.ClientUserId == userId);

            var requestLeadQuery = dbContext.Appointments
                .AsNoTracking()
                .Where(appointment => appointment.Status != AppointmentStatuses.Draft)
                .Join(
                    clientLeads,
                    appointment => appointment.LeadId,
                    lead => lead.Id,
                    (appointment, lead) => new { appointment, lead })
                .GroupBy(row => new
                {
                    row.lead.Id,
                    row.lead.RequestType,
                    row.lead.Notes,
                    row.lead.CreatedAtUtc
                })
                .Select(group => new ClientRequestLeadRow
                {
                    LeadId = group.Key.Id,
                    RequestType = group.Key.RequestType,
                    Notes = group.Key.Notes,
                    CreatedAtUtc = group.Key.CreatedAtUtc
                })
                .OrderByDescending(row => row.CreatedAtUtc);

            var totalItems = await requestLeadQuery.CountAsync(cancellationToken);
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)normalizedPageSize));
            normalizedPage = Math.Min(normalizedPage, totalPages);

            var requestRows = await requestLeadQuery
                .Skip((normalizedPage - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .ToListAsync(cancellationToken);

            var leadIds = requestRows
                .Select(row => row.LeadId)
                .Distinct()
                .ToList();

            var appointmentRows = leadIds.Count == 0
                ? Array.Empty<Appointment>()
                : await dbContext.Appointments
                    .AsNoTracking()
                    .Where(appointment =>
                        appointment.Status != AppointmentStatuses.Draft &&
                        leadIds.Contains(appointment.LeadId))
                    .OrderByDescending(appointment => appointment.CreatedAtUtc)
                    .ToArrayAsync(cancellationToken);

            var appointments = appointmentRows
                .GroupBy(appointment => appointment.LeadId)
                .ToDictionary(group => group.Key, group => group.First());

            var preferenceRows = leadIds.Count == 0
                ? Array.Empty<LeadPreference>()
                : await dbContext.LeadPreferences
                    .AsNoTracking()
                    .Where(preference => leadIds.Contains(preference.LeadId))
                    .OrderBy(preference => preference.Id)
                    .ToArrayAsync(cancellationToken);

            var preferences = preferenceRows
                .GroupBy(preference => preference.LeadId)
                .ToDictionary(group => group.Key, group => group.First());

            var propertyIds = requestRows
                .Where(row => appointments.ContainsKey(row.LeadId))
                .Select(row => appointments[row.LeadId].PropertyListingId)
                .Where(propertyListingId => propertyListingId.HasValue)
                .Select(propertyListingId => propertyListingId.Value)
                .Distinct()
                .ToList();

            var properties = propertyIds.Count == 0
                ? new Dictionary<Guid, PropertyListing>()
                : await dbContext.PropertyListings
                    .AsNoTracking()
                    .Where(property => propertyIds.Contains(property.Id))
                    .ToDictionaryAsync(property => property.Id, cancellationToken);

            var requests = new List<ClientRequestDto>();
            foreach (var row in requestRows)
            {
                if (!appointments.TryGetValue(row.LeadId, out var appointment))
                {
                    continue;
                }

                preferences.TryGetValue(row.LeadId, out var preference);
                var property =
                    appointment.PropertyListingId is Guid propertyId &&
                    properties.TryGetValue(propertyId, out var propertyListing)
                        ? propertyListing
                        : null;

                requests.Add(new ClientRequestDto
                {
                    Id = row.LeadId,
                    RequestType = row.RequestType,
                    Status = appointment.Status,
                    DesiredLocation = preference?.DesiredLocation,
                    MinimumRooms = preference?.MinimumRooms,
                    SustainableBudgetLabel = preference?.SustainableBudgetLabel,
                    PropertyType = preference?.PropertyType,
                    PropertyTitle = property?.Title,
                    PropertySlug = property?.Slug,
                    Notes = row.Notes,
                    CreatedAtUtc = row.CreatedAtUtc
                });
            }

            return new ClientRequestsResultDto
            {
                FavoriteCount = await dbContext.FavoriteProperties
                    .AsNoTracking()
                    .CountAsync(favorite => favorite.UserId == userId, cancellationToken),
                TotalItems = totalItems,
                Page = normalizedPage,
                PageSize = normalizedPageSize,
                Requests = requests
            };
        }

        private static int NormalizePageSize(int pageSize)
        {
            if (pageSize <= 0)
            {
                return DefaultPageSize;
            }

            return Math.Min(pageSize, MaxPageSize);
        }

        private sealed class ClientRequestLeadRow
        {
            public Guid LeadId { get; set; }

            public string RequestType { get; set; }

            public string Notes { get; set; }

            public DateTime CreatedAtUtc { get; set; }
        }
    }
}

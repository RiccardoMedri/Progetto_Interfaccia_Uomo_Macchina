using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class ClientNotificationPreferencesQuery
    {
        private readonly MedriDbContext dbContext;

        public ClientNotificationPreferencesQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ClientNotificationPreferencesResultDto> ExecuteAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var savedPreferenceRows = await dbContext.ClientNotificationPreferences
                .AsNoTracking()
                .Where(preference => preference.UserId == userId)
                .Select(preference => new ClientNotificationPreferenceDto
                {
                    Category = preference.Category,
                    IsActive = preference.IsActive,
                    IsDaily = preference.IsDaily,
                    IsWeekly = preference.IsWeekly
                })
                .ToArrayAsync(cancellationToken);

            var savedPreferences = savedPreferenceRows
                .GroupBy(preference => preference.Category, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal);

            var preferences = ClientNotificationCategories.All
                .Select(category =>
                    savedPreferences.TryGetValue(category, out var preference)
                        ? preference
                        : CreateDefaultPreference(category))
                .ToArray();

            return new ClientNotificationPreferencesResultDto
            {
                FavoriteCount = await dbContext.FavoriteProperties
                    .AsNoTracking()
                    .CountAsync(favorite => favorite.UserId == userId, cancellationToken),
                RequestCount = await dbContext.Leads
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
                    .CountAsync(cancellationToken),
                Preferences = preferences
            };
        }

        private static ClientNotificationPreferenceDto CreateDefaultPreference(string category)
        {
            return new ClientNotificationPreferenceDto
            {
                Category = category,
                IsActive = category != ClientNotificationCategories.SavedSearches,
                IsDaily = true,
                IsWeekly = false
            };
        }
    }
}

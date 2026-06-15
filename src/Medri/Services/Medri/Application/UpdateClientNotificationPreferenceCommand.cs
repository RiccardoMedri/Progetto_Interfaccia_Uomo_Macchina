using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class UpdateClientNotificationPreferenceCommand
    {
        private readonly MedriDbContext dbContext;

        public UpdateClientNotificationPreferenceCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<UpdateClientNotificationPreferenceResult> ExecuteAsync(
            Guid userId,
            UpdateClientNotificationPreferenceDto input,
            CancellationToken cancellationToken = default)
        {
            if (input == null ||
                !ClientNotificationCategories.All.Contains(input.Category, StringComparer.Ordinal))
            {
                return new UpdateClientNotificationPreferenceResult
                {
                    Succeeded = false
                };
            }

            var preference = await dbContext.ClientNotificationPreferences
                .FirstOrDefaultAsync(
                    item => item.UserId == userId && item.Category == input.Category,
                    cancellationToken);

            if (preference == null)
            {
                preference = new ClientNotificationPreference
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Category = input.Category,
                    IsActive = input.Category != ClientNotificationCategories.SavedSearches,
                    IsDaily = true,
                    IsWeekly = false
                };
                dbContext.ClientNotificationPreferences.Add(preference);
            }

            preference.IsActive = input.IsActive;
            if (input.IsDaily.HasValue)
            {
                preference.IsDaily = input.IsDaily.Value;
            }

            if (input.IsWeekly.HasValue)
            {
                preference.IsWeekly = input.IsWeekly.Value;
            }
            preference.UpdatedAtUtc = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);
            return new UpdateClientNotificationPreferenceResult
            {
                Succeeded = true
            };
        }
    }
}

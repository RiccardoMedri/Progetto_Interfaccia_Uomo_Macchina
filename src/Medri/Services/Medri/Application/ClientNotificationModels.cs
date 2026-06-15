using System;
using System.Collections.Generic;

namespace Medri.Services.Medri.Application
{
    public static class ClientNotificationCategories
    {
        public const string Requests = "Requests";
        public const string Favorites = "Favorites";
        public const string SavedSearches = "SavedSearches";

        public static readonly IReadOnlyList<string> All = new[]
        {
            Requests,
            Favorites,
            SavedSearches
        };
    }

    public sealed class ClientNotificationPreferencesResultDto
    {
        public int FavoriteCount { get; set; }

        public int RequestCount { get; set; }

        public IReadOnlyList<ClientNotificationPreferenceDto> Preferences { get; set; } =
            Array.Empty<ClientNotificationPreferenceDto>();
    }

    public sealed class ClientNotificationPreferenceDto
    {
        public string Category { get; set; }

        public bool IsActive { get; set; }

        public bool IsDaily { get; set; }

        public bool IsWeekly { get; set; }
    }

    public sealed class UpdateClientNotificationPreferenceDto
    {
        public string Category { get; set; }

        public bool IsActive { get; set; }

        public bool? IsDaily { get; set; }

        public bool? IsWeekly { get; set; }
    }

    public sealed class UpdateClientNotificationPreferenceResult
    {
        public bool Succeeded { get; set; }
    }
}

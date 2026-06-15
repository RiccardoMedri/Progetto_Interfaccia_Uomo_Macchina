using System.Collections.Generic;
using System.Linq;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Client;

namespace Medri.Web.Areas.Client.Notifications
{
    internal static class ClientNotificationsViewModelMapper
    {
        private static readonly IReadOnlyList<PreferencePresentation> Presentations =
            new[]
            {
                new PreferencePresentation(
                    ClientNotificationCategories.Requests,
                    "Richieste",
                    "Passaggi chiave delle pratiche aperte",
                    "Stato della richiesta, documenti richiesti, visita fissata e feedback del consulente."),
                new PreferencePresentation(
                    ClientNotificationCategories.Favorites,
                    "Preferiti",
                    "Variazioni sugli annunci salvati",
                    "Prezzo aggiornato, immobile opzionato, visita aperta o annuncio non piu disponibile."),
                new PreferencePresentation(
                    ClientNotificationCategories.SavedSearches,
                    "Ricerche salvate",
                    "Nuovi match coerenti con filtri e quartieri",
                    "Nuovi immobili compatibili con zona, budget e taglio, senza riaprire ogni volta la ricerca.")
            };

        public static ClientNotificationsViewModel Create(
            ClientNotificationPreferencesResultDto result)
        {
            var preferences = result.Preferences.ToDictionary(preference => preference.Category);
            return new ClientNotificationsViewModel
            {
                Navigation = ClientAreaNavigationViewModel.Create(
                    ClientAreaTabs.Notifications,
                    result.FavoriteCount,
                    result.RequestCount),
                Preferences = Presentations
                    .Select(presentation =>
                    {
                        var preference = preferences.TryGetValue(presentation.Category, out var savedPreference)
                            ? savedPreference
                            : new ClientNotificationPreferenceDto
                            {
                                Category = presentation.Category,
                                IsActive = presentation.Category != ClientNotificationCategories.SavedSearches,
                                IsDaily = true
                            };
                        return new ClientNotificationPreferenceViewModel
                        {
                            Category = presentation.Category,
                            Label = presentation.Label,
                            Title = presentation.Title,
                            Description = presentation.Description,
                            IsActive = preference.IsActive,
                            IsDaily = preference.IsDaily,
                            IsWeekly = preference.IsWeekly
                        };
                    })
                    .ToArray()
            };
        }

        public static UpdateClientNotificationPreferenceDto CreateCommand(
            ClientNotificationPreferenceInputModel input)
        {
            return new UpdateClientNotificationPreferenceDto
            {
                Category = input.Category,
                IsActive = input.IsActive,
                IsDaily = input.IsDaily,
                IsWeekly = input.IsWeekly
            };
        }

        private sealed class PreferencePresentation
        {
            public PreferencePresentation(
                string category,
                string label,
                string title,
                string description)
            {
                Category = category;
                Label = label;
                Title = title;
                Description = description;
            }

            public string Category { get; }

            public string Label { get; }

            public string Title { get; }

            public string Description { get; }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Medri.Web.Areas.Client
{
    public static class ClientAreaTabs
    {
        public const string Saved = "Saved";
        public const string Requests = "Requests";
        public const string Searches = "Searches";
        public const string Notifications = "Notifications";
    }

    public sealed class ClientAreaNavigationViewModel
    {
        public string CurrentLabel { get; set; }

        public IReadOnlyList<ClientAreaTabViewModel> Tabs { get; set; } =
            Array.Empty<ClientAreaTabViewModel>();

        public static ClientAreaNavigationViewModel Create(
            string activeTab,
            int favoriteCount,
            int requestCount)
        {
            return new ClientAreaNavigationViewModel
            {
                CurrentLabel = Label(activeTab),
                Tabs = new[]
                {
                    new ClientAreaTabViewModel
                    {
                        Label = "Annunci Preferiti",
                        Area = "Client",
                        Controller = "Saved",
                        Action = "Index",
                        Count = favoriteCount,
                        IsActive = activeTab == ClientAreaTabs.Saved
                    },
                    new ClientAreaTabViewModel
                    {
                        Label = "Richieste",
                        Area = "Client",
                        Controller = "Requests",
                        Action = "Index",
                        Count = requestCount,
                        IsActive = activeTab == ClientAreaTabs.Requests
                    },
                    new ClientAreaTabViewModel
                    {
                        Label = "Ricerche",
                        Area = "Client",
                        Controller = "Searches",
                        Action = "Index",
                        IsActive = activeTab == ClientAreaTabs.Searches
                    },
                    new ClientAreaTabViewModel
                    {
                        Label = "Notifiche",
                        Area = "Client",
                        Controller = "Notifications",
                        Action = "Index",
                        IsActive = activeTab == ClientAreaTabs.Notifications
                    }
                }
            };
        }

        private static string Label(string activeTab)
        {
            return activeTab == ClientAreaTabs.Requests
                ? "Richieste"
                : activeTab == ClientAreaTabs.Searches
                    ? "Ricerche"
                    : activeTab == ClientAreaTabs.Notifications
                        ? "Notifiche"
                        : "Annunci Preferiti";
        }
    }

    public sealed class ClientAreaTabViewModel
    {
        public string Label { get; set; }

        public string Area { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public int? Count { get; set; }

        public bool IsActive { get; set; }

        public string LinkCssClass => IsActive ? "nav-link active" : "nav-link";

        public string AriaCurrent => IsActive ? "page" : null;
    }
}

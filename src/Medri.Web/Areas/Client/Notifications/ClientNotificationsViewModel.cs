using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Medri.Web.Areas.Client;

namespace Medri.Web.Areas.Client.Notifications
{
    public sealed class ClientNotificationsViewModel
    {
        public ClientAreaNavigationViewModel Navigation { get; set; }

        public IReadOnlyList<ClientNotificationPreferenceViewModel> Preferences { get; set; } =
            Array.Empty<ClientNotificationPreferenceViewModel>();

        public bool HasPreferences => Preferences.Count > 0;
    }

    public sealed class ClientNotificationPreferenceViewModel
    {
        public string Category { get; set; }

        public string Label { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public bool IsDaily { get; set; }

        public bool IsWeekly { get; set; }
    }

    public sealed class ClientNotificationPreferenceInputModel
    {
        [Required]
        public string Category { get; set; }

        public bool IsActive { get; set; }

        public bool? IsDaily { get; set; }

        public bool? IsWeekly { get; set; }
    }
}

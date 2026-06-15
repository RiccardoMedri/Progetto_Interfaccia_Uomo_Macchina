using System;
using System.Collections.Generic;
using Medri.Web.Areas.Admin;

namespace Medri.Web.Areas.Admin.Dashboard
{
    public sealed class AdminDashboardViewModel : IAdminPageViewModel
    {
        public AdminNavigationViewModel Navigation { get; set; }

        public int LeadWorkCount { get; set; }

        public int ActiveRequestCount { get; set; }

        public int IncompleteListingCount { get; set; }

        public IReadOnlyList<AdminDashboardLeadViewModel> RecentLeads { get; set; } =
            Array.Empty<AdminDashboardLeadViewModel>();

        public IReadOnlyList<AdminDashboardRequestViewModel> ActiveRequests { get; set; } =
            Array.Empty<AdminDashboardRequestViewModel>();

        public IReadOnlyList<AdminDashboardListingViewModel> Listings { get; set; } =
            Array.Empty<AdminDashboardListingViewModel>();
    }

    public sealed class AdminDashboardLeadViewModel
    {
        public string Reference { get; set; }

        public string ContactName { get; set; }

        public string IntentLabel { get; set; }

        public string AdvisorLabel { get; set; }
    }

    public sealed class AdminDashboardRequestViewModel
    {
        public string Reference { get; set; }

        public string CustomerName { get; set; }

        public string StatusLabel { get; set; }

        public string StatusClass { get; set; }

        public string AdvisorLabel { get; set; }
    }

    public sealed class AdminDashboardListingViewModel
    {
        public string Reference { get; set; }

        public string Title { get; set; }

        public string StatusLabel { get; set; }

        public string StatusClass { get; set; }

        public string AdvisorLabel { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminDashboardResultDto
    {
        public int LeadWorkCount { get; set; }

        public int ActiveRequestCount { get; set; }

        public int IncompleteListingCount { get; set; }

        public int ListingCount { get; set; }

        public IReadOnlyList<AdminDashboardLeadDto> RecentLeads { get; set; } =
            Array.Empty<AdminDashboardLeadDto>();

        public IReadOnlyList<AdminDashboardRequestDto> ActiveRequests { get; set; } =
            Array.Empty<AdminDashboardRequestDto>();

        public IReadOnlyList<AdminDashboardListingDto> Listings { get; set; } =
            Array.Empty<AdminDashboardListingDto>();
    }

    public sealed class AdminDashboardLeadDto
    {
        public string Reference { get; set; }

        public string ContactName { get; set; }

        public string RequestType { get; set; }

        public string AdvisorDisplayName { get; set; }
    }

    public sealed class AdminDashboardRequestDto
    {
        public string Reference { get; set; }

        public string CustomerName { get; set; }

        public string Status { get; set; }

        public string AdvisorDisplayName { get; set; }
    }

    public sealed class AdminDashboardListingDto
    {
        public string Reference { get; set; }

        public string Title { get; set; }

        public string Status { get; set; }

        public string AdvisorDisplayName { get; set; }
    }
}

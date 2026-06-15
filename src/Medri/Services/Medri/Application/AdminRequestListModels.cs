using System;
using System.Collections.Generic;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminRequestListQueryFilter
    {
        public string SearchTerm { get; set; }

        public IReadOnlyList<string> RequestTypes { get; set; } =
            Array.Empty<string>();

        public IReadOnlyList<string> Statuses { get; set; } =
            Array.Empty<string>();

        public IReadOnlyList<Guid> AssignedAgencyUserIds { get; set; } =
            Array.Empty<Guid>();

        public bool OnlyUnassigned { get; set; }

        public IReadOnlyList<string> Priorities { get; set; } =
            Array.Empty<string>();

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 15;
    }

    public sealed class AdminRequestListResultDto
    {
        public int LeadCount { get; set; }

        public int ActiveRequestCount { get; set; }

        public int ListingCount { get; set; }

        public int NewCount { get; set; }

        public int AssignedCount { get; set; }

        public int UpdatingCount { get; set; }

        public int MatchingCount { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public IReadOnlyList<AdminRequestRowDto> Requests { get; set; } =
            Array.Empty<AdminRequestRowDto>();

        public IReadOnlyList<AdminLeadAdvisorDto> Advisors { get; set; } =
            Array.Empty<AdminLeadAdvisorDto>();
    }

    public sealed class AdminRequestRowDto
    {
        public Guid Id { get; set; }

        public string Reference { get; set; }

        public string CustomerName { get; set; }

        public string ContactSummary { get; set; }

        public string RequestType { get; set; }

        public string Status { get; set; }

        public int CompletionPercent { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        public bool IsAssigned { get; set; }

        public string Priority { get; set; }

        public IReadOnlyList<string> Criteria { get; set; } =
            Array.Empty<string>();

        public string AdvisorDisplayName { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminLeadListQueryFilter
    {
        public string SearchTerm { get; set; }

        public IReadOnlyList<string> Statuses { get; set; } =
            Array.Empty<string>();

        public IReadOnlyList<string> RequestTypes { get; set; } =
            Array.Empty<string>();

        public IReadOnlyList<string> Priorities { get; set; } =
            Array.Empty<string>();

        public IReadOnlyList<Guid> AssignedAgencyUserIds { get; set; } =
            Array.Empty<Guid>();

        public bool OnlyUnassigned { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 15;
    }

    public sealed class AdminLeadListResultDto
    {
        public int NewCount { get; set; }

        public int InContactCount { get; set; }

        public int ArchivedCount { get; set; }

        public int ActiveRequestCount { get; set; }

        public int ListingCount { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public IReadOnlyList<AdminLeadRowDto> Leads { get; set; } =
            Array.Empty<AdminLeadRowDto>();

        public IReadOnlyList<AdminLeadAdvisorDto> Advisors { get; set; } =
            Array.Empty<AdminLeadAdvisorDto>();
    }

    public sealed class AdminLeadRowDto
    {
        public Guid Id { get; set; }

        public string Reference { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string SourceChannel { get; set; }

        public string RequestType { get; set; }

        public string WorkflowStatus { get; set; }

        public string Priority { get; set; }

        public int QualificationPercent { get; set; }

        public string AdvisorDisplayName { get; set; }
    }

    public sealed class AdminLeadAdvisorDto
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }
    }
}

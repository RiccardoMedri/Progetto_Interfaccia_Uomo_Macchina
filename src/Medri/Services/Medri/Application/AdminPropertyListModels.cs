using System;
using System.Collections.Generic;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminPropertyListQueryFilter
    {
        public string SearchTerm { get; set; }

        public IReadOnlyList<string> Statuses { get; set; } =
            Array.Empty<string>();

        public IReadOnlyList<string> Contracts { get; set; } =
            Array.Empty<string>();

        public IReadOnlyList<Guid> AssignedAgencyUserIds { get; set; } =
            Array.Empty<Guid>();

        public bool OnlyUnassigned { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 15;
    }

    public sealed class AdminPropertyListResultDto
    {
        public int LeadCount { get; set; }

        public int ActiveRequestCount { get; set; }

        public int ListingCount { get; set; }

        public int IncompleteCount { get; set; }

        public int ReadyCount { get; set; }

        public int PublishedCount { get; set; }

        public int NeedsUpdateCount { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public IReadOnlyList<AdminPropertyRowDto> Properties { get; set; } =
            Array.Empty<AdminPropertyRowDto>();

        public IReadOnlyList<AdminPropertyFeaturedSlotDto> FeaturedSlots { get; set; } =
            Array.Empty<AdminPropertyFeaturedSlotDto>();

        public IReadOnlyList<AdminPropertyAdvisorDto> Advisors { get; set; } =
            Array.Empty<AdminPropertyAdvisorDto>();
    }

    public sealed class AdminPropertyRowDto
    {
        public Guid Id { get; set; }

        public string Reference { get; set; }

        public string Title { get; set; }

        public string DisplayLocation { get; set; }

        public string Contract { get; set; }

        public string PublicationStatus { get; set; }

        public int CompletionPercent { get; set; }

        public string MissingItems { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        public string AdvisorDisplayName { get; set; }

        public string ImageUrl { get; set; }

        public int SortOrder { get; set; }

        public bool IsFeatured { get; set; }

        public bool CanFeature { get; set; }

        public int? FeaturedSortOrder { get; set; }
    }

    public sealed class AdminPropertyFeaturedSlotDto
    {
        public int SlotNumber { get; set; }

        public string Reference { get; set; }

        public string Title { get; set; }

        public string DisplayLocation { get; set; }
    }

    public sealed class AdminPropertyBulkCommandResult
    {
        public int UpdatedCount { get; set; }

        public bool HasUpdates => UpdatedCount > 0;
    }

    public sealed class AdminPropertyAdvisorDto
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }
    }
}

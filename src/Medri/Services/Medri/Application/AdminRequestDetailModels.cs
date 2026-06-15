using System;
using System.Collections.Generic;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminRequestDetailResultDto
    {
        public int LeadCount { get; set; }

        public int ActiveRequestCount { get; set; }

        public int ListingCount { get; set; }

        public Guid SearchProfileId { get; set; }

        public Guid LeadId { get; set; }

        public string Reference { get; set; }

        public bool IsCreateMode { get; set; }

        public string Status { get; set; }

        public string CriteriaSummary { get; set; }

        public string SourceQueryString { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string SourceChannel { get; set; }

        public string RequestType { get; set; }

        public string LeadNotes { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        public string LinkedLeadReference { get; set; }

        public decimal? MaximumPrice { get; set; }

        public string MaximumBudgetLabel { get; set; }

        public string PreferredBudgetLabel { get; set; }

        public string DesiredLocation { get; set; }

        public string AcceptableLocations { get; set; }

        public int? MinimumRooms { get; set; }

        public string AccessibilityConstraint { get; set; }

        public string Priority { get; set; }

        public string TimeFrame { get; set; }

        public string FinancingStatus { get; set; }

        public string PropertyToSellStatus { get; set; }

        public string SummaryNotes { get; set; }

        public string NeedsAfterContact { get; set; }

        public string DesiredPreferenceTags { get; set; }

        public string NegotiablePreferenceTags { get; set; }

        public IReadOnlyList<AdminLeadAdvisorDto> Advisors { get; set; } =
            Array.Empty<AdminLeadAdvisorDto>();

        public IReadOnlyList<AdminLeadInteractionDto> Interactions { get; set; } =
            Array.Empty<AdminLeadInteractionDto>();
    }

    public sealed class AdminRequestDetailUpdateDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string SourceChannel { get; set; }

        public string Status { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        public string RequestType { get; set; }

        public string MaximumBudgetLabel { get; set; }

        public string PreferredBudgetLabel { get; set; }

        public string DesiredLocation { get; set; }

        public string AcceptableLocations { get; set; }

        public int? MinimumRooms { get; set; }

        public string AccessibilityConstraint { get; set; }

        public string TimeFrame { get; set; }

        public string FinancingStatus { get; set; }

        public string PropertyToSellStatus { get; set; }

        public string SummaryNotes { get; set; }

        public string NeedsAfterContact { get; set; }

        public string DesiredPreferenceTagsText { get; set; }

        public string NegotiablePreferenceTagsText { get; set; }
    }

    public sealed class AdminRequestCommandResult
    {
        public bool Succeeded { get; set; }

        public string Reference { get; set; }

        public string ErrorCode { get; set; }

        public static AdminRequestCommandResult Success(string reference)
        {
            return new AdminRequestCommandResult
            {
                Succeeded = true,
                Reference = reference
            };
        }

        public static AdminRequestCommandResult NotFound(string reference)
        {
            return new AdminRequestCommandResult
            {
                Succeeded = false,
                Reference = reference,
                ErrorCode = "NotFound"
            };
        }
    }
}

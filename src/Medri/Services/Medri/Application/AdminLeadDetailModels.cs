using System;
using System.Collections.Generic;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminLeadDetailResultDto
    {
        public int NewCount { get; set; }

        public int ActiveRequestCount { get; set; }

        public int ListingCount { get; set; }

        public Guid Id { get; set; }

        public string Reference { get; set; }

        public bool IsCreateMode { get; set; }

        public string WorkflowStatus { get; set; }

        public int QualificationPercent { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string SourceChannel { get; set; }

        public string RequestType { get; set; }

        public string Notes { get; set; }

        public string NextAction { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public string Urgency { get; set; }

        public string DesiredLocation { get; set; }

        public string ExpectedPriceOrMainQuestion { get; set; }

        public string LinkedPropertyReference { get; set; }

        public string ContactReason { get; set; }

        public string NextContactQuestions { get; set; }

        public string SearchProfileReference { get; set; }

        public IReadOnlyList<AdminLeadAdvisorDto> Advisors { get; set; } =
            Array.Empty<AdminLeadAdvisorDto>();

        public IReadOnlyList<AdminLeadInteractionDto> Interactions { get; set; } =
            Array.Empty<AdminLeadInteractionDto>();
    }

    public sealed class AdminLeadInteractionDto
    {
        public string Channel { get; set; }

        public string Notes { get; set; }

        public DateTime OccurredAtUtc { get; set; }
    }

    public sealed class AdminLeadDetailUpdateDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string SourceChannel { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        public string RequestType { get; set; }

        public string Urgency { get; set; }

        public string DesiredLocation { get; set; }

        public string ExpectedPriceOrMainQuestion { get; set; }

        public string LinkedPropertyReference { get; set; }

        public string ContactReason { get; set; }

        public string Notes { get; set; }

        public string NextContactQuestions { get; set; }
    }

    public sealed class AdminLeadCommandResult
    {
        public bool Succeeded { get; set; }

        public string Reference { get; set; }

        public string ErrorCode { get; set; }

        public IReadOnlyList<string> ChangedFields { get; set; } =
            Array.Empty<string>();

        public bool HasChanges => ChangedFields.Count > 0;

        public static AdminLeadCommandResult Success(string reference)
        {
            return Success(reference, Array.Empty<string>());
        }

        public static AdminLeadCommandResult Success(
            string reference,
            IReadOnlyList<string> changedFields)
        {
            return new AdminLeadCommandResult
            {
                Succeeded = true,
                Reference = reference,
                ChangedFields = changedFields ?? Array.Empty<string>()
            };
        }

        public static AdminLeadCommandResult NotFound(string reference)
        {
            return new AdminLeadCommandResult
            {
                Succeeded = false,
                Reference = reference,
                ErrorCode = "NotFound"
            };
        }
    }
}

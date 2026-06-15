using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Medri.Services.Medri.Application;

namespace Medri.Web.Areas.Admin.Leads
{
    public class AdminLeadDetailInputModel
    {
        [Required, StringLength(80)]
        public string FirstName { get; set; }

        [Required, StringLength(80)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [EmailAddress, StringLength(254)]
        public string Email { get; set; }

        [Required, StringLength(80)]
        public string SourceChannel { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        [Required, StringLength(80)]
        public string RequestType { get; set; }

        [StringLength(80)]
        public string Urgency { get; set; }

        [StringLength(500)]
        public string DesiredLocation { get; set; }

        [StringLength(500)]
        public string ExpectedPriceOrMainQuestion { get; set; }

        [StringLength(160)]
        public string LinkedPropertyReference { get; set; }

        [StringLength(1000)]
        public string ContactReason { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        [StringLength(2000)]
        public string NextContactQuestions { get; set; }

        public AdminLeadDetailUpdateDto ToUpdateDto()
        {
            return new AdminLeadDetailUpdateDto
            {
                FirstName = FirstName,
                LastName = LastName,
                Phone = Phone,
                Email = Email,
                SourceChannel = SourceChannel,
                AssignedAgencyUserId = AssignedAgencyUserId,
                RequestType = RequestType,
                Urgency = Urgency,
                DesiredLocation = DesiredLocation,
                ExpectedPriceOrMainQuestion = ExpectedPriceOrMainQuestion,
                LinkedPropertyReference = LinkedPropertyReference,
                ContactReason = ContactReason,
                Notes = Notes,
                NextContactQuestions = NextContactQuestions
            };
        }
    }

    public sealed class AdminLeadDetailViewModel : AdminLeadDetailInputModel, IAdminPageViewModel
    {
        public AdminNavigationViewModel Navigation { get; set; }

        public string Reference { get; set; }

        public bool IsCreateMode { get; set; }

        public string DisplayName { get; set; }

        public string WorkflowStatus { get; set; }

        public string StatusLabel { get; set; }

        public string StatusClass { get; set; }

        public string SearchProfileReference { get; set; }

        public string PhoneHref { get; set; }

        public string WhatsAppHref { get; set; }

        public string MailtoHref { get; set; }

        public IReadOnlyList<AdminLeadSummaryCardViewModel> SummaryCards { get; set; } =
            Array.Empty<AdminLeadSummaryCardViewModel>();

        public IReadOnlyList<AdminLeadOptionViewModel> SourceOptions { get; set; } =
            Array.Empty<AdminLeadOptionViewModel>();

        public IReadOnlyList<AdminLeadOptionViewModel> AdvisorOptions { get; set; } =
            Array.Empty<AdminLeadOptionViewModel>();

        public IReadOnlyList<AdminLeadOptionViewModel> RequestTypeOptions { get; set; } =
            Array.Empty<AdminLeadOptionViewModel>();

        public IReadOnlyList<AdminLeadOptionViewModel> UrgencyOptions { get; set; } =
            Array.Empty<AdminLeadOptionViewModel>();

        public IReadOnlyList<AdminLeadInteractionViewModel> Interactions { get; set; } =
            Array.Empty<AdminLeadInteractionViewModel>();

        public IReadOnlyList<AdminLeadChecklistItemViewModel> ConversionChecklist { get; set; } =
            Array.Empty<AdminLeadChecklistItemViewModel>();

        public bool CanConvert => !IsCreateMode && WorkflowStatus != "Qualified" && WorkflowStatus != "Archived";

        public bool CanArchive => !IsCreateMode && WorkflowStatus != "Archived";

        public bool CanRestore => !IsCreateMode && WorkflowStatus == "Archived";
    }

    public sealed class AdminLeadSummaryCardViewModel
    {
        public string Label { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }
    }

    public sealed class AdminLeadInteractionViewModel
    {
        public string TimeLabel { get; set; }

        public string Title { get; set; }

        public string Notes { get; set; }
    }

    public sealed class AdminLeadChecklistItemViewModel
    {
        public string Label { get; set; }

        public bool IsDone { get; set; }
    }
}

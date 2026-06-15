using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Admin;
using Medri.Web.Areas.Admin.Leads;

namespace Medri.Web.Areas.Admin.Requests
{
    public class AdminRequestDetailInputModel
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

        [Required, StringLength(80)]
        public string Status { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        [Required, StringLength(80)]
        public string RequestType { get; set; }

        [StringLength(80)]
        public string MaximumBudgetLabel { get; set; }

        [StringLength(80)]
        public string PreferredBudgetLabel { get; set; }

        [StringLength(500)]
        public string DesiredLocation { get; set; }

        [StringLength(500)]
        public string AcceptableLocations { get; set; }

        [Range(0, 100)]
        public int? MinimumRooms { get; set; }

        [StringLength(500)]
        public string AccessibilityConstraint { get; set; }

        [StringLength(80)]
        public string TimeFrame { get; set; }

        [StringLength(120)]
        public string FinancingStatus { get; set; }

        [StringLength(120)]
        public string PropertyToSellStatus { get; set; }

        [StringLength(2000)]
        public string SummaryNotes { get; set; }

        [StringLength(2000)]
        public string NeedsAfterContact { get; set; }

        [StringLength(2000)]
        public string DesiredPreferenceTagsText { get; set; }

        [StringLength(2000)]
        public string NegotiablePreferenceTagsText { get; set; }

        public AdminRequestDetailUpdateDto ToUpdateDto()
        {
            return new AdminRequestDetailUpdateDto
            {
                FirstName = FirstName,
                LastName = LastName,
                Phone = Phone,
                Email = Email,
                SourceChannel = SourceChannel,
                Status = Status,
                AssignedAgencyUserId = AssignedAgencyUserId,
                RequestType = RequestType,
                MaximumBudgetLabel = MaximumBudgetLabel,
                PreferredBudgetLabel = PreferredBudgetLabel,
                DesiredLocation = DesiredLocation,
                AcceptableLocations = AcceptableLocations,
                MinimumRooms = MinimumRooms,
                AccessibilityConstraint = AccessibilityConstraint,
                TimeFrame = TimeFrame,
                FinancingStatus = FinancingStatus,
                PropertyToSellStatus = PropertyToSellStatus,
                SummaryNotes = SummaryNotes,
                NeedsAfterContact = NeedsAfterContact,
                DesiredPreferenceTagsText = DesiredPreferenceTagsText,
                NegotiablePreferenceTagsText = NegotiablePreferenceTagsText
            };
        }
    }

    public sealed class AdminRequestDetailViewModel : AdminRequestDetailInputModel, IAdminPageViewModel
    {
        public AdminNavigationViewModel Navigation { get; set; }

        public string Reference { get; set; }

        public bool IsCreateMode { get; set; }

        public string DisplayName { get; set; }

        public string StatusLabel { get; set; }

        public string StatusClass { get; set; }

        public string PriorityLabel { get; set; }

        public string LinkedLeadReference { get; set; }

        public string PhoneHref { get; set; }

        public string WhatsAppHref { get; set; }

        public bool CanArchive { get; set; }

        public IReadOnlyList<AdminRequestSummaryCardViewModel> SummaryCards { get; set; } =
            Array.Empty<AdminRequestSummaryCardViewModel>();

        public IReadOnlyList<AdminLeadOptionViewModel> SourceOptions { get; set; } =
            Array.Empty<AdminLeadOptionViewModel>();

        public IReadOnlyList<AdminLeadOptionViewModel> AdvisorOptions { get; set; } =
            Array.Empty<AdminLeadOptionViewModel>();

        public IReadOnlyList<AdminLeadOptionViewModel> StatusOptions { get; set; } =
            Array.Empty<AdminLeadOptionViewModel>();

        public IReadOnlyList<AdminLeadOptionViewModel> RequestTypeOptions { get; set; } =
            Array.Empty<AdminLeadOptionViewModel>();

        public IReadOnlyList<AdminLeadOptionViewModel> TimeFrameOptions { get; set; } =
            Array.Empty<AdminLeadOptionViewModel>();

        public IReadOnlyList<AdminLeadOptionViewModel> FinancingOptions { get; set; } =
            Array.Empty<AdminLeadOptionViewModel>();

        public IReadOnlyList<AdminLeadOptionViewModel> PropertyToSellOptions { get; set; } =
            Array.Empty<AdminLeadOptionViewModel>();

        public IReadOnlyList<AdminRequestInfoCardViewModel> ConstraintCards { get; set; } =
            Array.Empty<AdminRequestInfoCardViewModel>();

        public IReadOnlyList<string> DesiredPreferenceChips { get; set; } =
            Array.Empty<string>();

        public IReadOnlyList<string> NegotiablePreferenceChips { get; set; } =
            Array.Empty<string>();

        public IReadOnlyList<AdminLeadInteractionViewModel> Interactions { get; set; } =
            Array.Empty<AdminLeadInteractionViewModel>();

        public IReadOnlyList<AdminLeadChecklistItemViewModel> QualificationChecklist { get; set; } =
            Array.Empty<AdminLeadChecklistItemViewModel>();
    }

    public sealed class AdminRequestSummaryCardViewModel
    {
        public string Label { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }
    }

    public sealed class AdminRequestInfoCardViewModel
    {
        public string Label { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }
    }
}

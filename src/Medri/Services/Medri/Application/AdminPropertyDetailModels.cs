using System;
using System.Collections.Generic;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminPropertyDetailResultDto
    {
        public Guid Id { get; set; }

        public string Reference { get; set; }

        public bool IsCreateMode { get; set; }

        public string PublicationStatus { get; set; }

        public int CompletionPercent { get; set; }

        public string MissingItems { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string ImageUrl { get; set; }

        public string ListingCategory { get; set; }

        public string DisplayLocation { get; set; }

        public string Address { get; set; }

        public string Contract { get; set; }

        public decimal Price { get; set; }

        public int SurfaceSquareMeters { get; set; }

        public int Rooms { get; set; }

        public string BedroomsLabel { get; set; }

        public int Bathrooms { get; set; }

        public string EnergyClass { get; set; }

        public string RequiredWorksLabel { get; set; }

        public string CondoFeesLabel { get; set; }

        public string FloorLabel { get; set; }

        public string ElevatorLabel { get; set; }

        public string OutdoorSpaceLabel { get; set; }

        public string GarageLabel { get; set; }

        public string Description { get; set; }

        public string SummaryTitle { get; set; }

        public string SummaryParagraph2 { get; set; }

        public string ReadinessNote { get; set; }

        public string CostsNote { get; set; }

        public string ContextNote { get; set; }

        public string DecisionMarginNote { get; set; }

        public string AvailabilityLabel { get; set; }

        public string HeatingLabel { get; set; }

        public string ConstructionYearLabel { get; set; }

        public string BalconyLabel { get; set; }

        public string CellarLabel { get; set; }

        public string NearbyServicesLabel { get; set; }

        public IReadOnlyList<string> QuickNotes { get; set; } =
            Array.Empty<string>();

        public string HumanFitNote { get; set; }

        public string AdvisorDisplayName { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        public int LeadCount { get; set; }

        public int ActiveRequestCount { get; set; }

        public int ListingCount { get; set; }

        public IReadOnlyList<AdminPropertyMediaDto> Media { get; set; } =
            Array.Empty<AdminPropertyMediaDto>();

        public IReadOnlyList<AdminPropertyChecklistItemDto> Checklist { get; set; } =
            Array.Empty<AdminPropertyChecklistItemDto>();

        public IReadOnlyList<AdminPropertyAdvisorDto> Advisors { get; set; } =
            Array.Empty<AdminPropertyAdvisorDto>();
    }

    public sealed class AdminPropertyMediaDto
    {
        public Guid Id { get; set; }

        public string Url { get; set; }

        public string AltText { get; set; }

        public int SortOrder { get; set; }
    }

    public sealed class AdminPropertyChecklistItemDto
    {
        public string Label { get; set; }

        public bool IsDone { get; set; }
    }

    public sealed class AdminPropertyDetailUpdateDto
    {
        public string Title { get; set; }

        public string ListingCategory { get; set; }

        public string Contract { get; set; }

        public decimal Price { get; set; }

        public string DisplayLocation { get; set; }

        public string Address { get; set; }

        public int SurfaceSquareMeters { get; set; }

        public int Rooms { get; set; }

        public string BedroomsLabel { get; set; }

        public int Bathrooms { get; set; }

        public string EnergyClass { get; set; }

        public string RequiredWorksLabel { get; set; }

        public string CondoFeesLabel { get; set; }

        public string FloorLabel { get; set; }

        public string ElevatorLabel { get; set; }

        public string OutdoorSpaceLabel { get; set; }

        public string GarageLabel { get; set; }

        public string Description { get; set; }

        public string SummaryTitle { get; set; }

        public string SummaryParagraph2 { get; set; }

        public string ReadinessNote { get; set; }

        public string CostsNote { get; set; }

        public string ContextNote { get; set; }

        public string DecisionMarginNote { get; set; }

        public string AvailabilityLabel { get; set; }

        public string HeatingLabel { get; set; }

        public string ConstructionYearLabel { get; set; }

        public string BalconyLabel { get; set; }

        public string CellarLabel { get; set; }

        public string NearbyServicesLabel { get; set; }

        public IReadOnlyList<string> QuickNotes { get; set; } =
            Array.Empty<string>();

        public string HumanFitNote { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        public IReadOnlyList<AdminPropertyMediaUpdateDto> Media { get; set; } =
            Array.Empty<AdminPropertyMediaUpdateDto>();
    }

    public sealed class AdminPropertyMediaUpdateDto
    {
        public Guid Id { get; set; }

        public string AltText { get; set; }

        public int SortOrder { get; set; }

        public bool Remove { get; set; }
    }

    public sealed class AdminPropertyCommandResult
    {
        public bool Succeeded { get; set; }

        public string Reference { get; set; }

        public bool IsBlocked { get; set; }

        public string Message { get; set; }

        public static AdminPropertyCommandResult NotFound()
        {
            return new AdminPropertyCommandResult();
        }

        public static AdminPropertyCommandResult Blocked(
            string reference,
            string message)
        {
            return new AdminPropertyCommandResult
            {
                Reference = reference,
                IsBlocked = true,
                Message = message
            };
        }

        public static AdminPropertyCommandResult Success(string reference)
        {
            return new AdminPropertyCommandResult
            {
                Succeeded = true,
                Reference = reference
            };
        }
    }
}

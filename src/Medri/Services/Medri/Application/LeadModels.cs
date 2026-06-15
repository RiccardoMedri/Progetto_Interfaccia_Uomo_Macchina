using System;
using System.Collections.Generic;

namespace Medri.Services.Medri.Application
{
    public class SaveSearchRequestDto
    {
        public IReadOnlyList<string> Contracts { get; set; } = new List<string>();

        public IReadOnlyList<string> PropertyTypes { get; set; } = new List<string>();

        public IReadOnlyList<string> Rooms { get; set; } = new List<string>();

        public IReadOnlyList<string> Zones { get; set; } = new List<string>();

        public IReadOnlyList<string> PriceRanges { get; set; } = new List<string>();

        public IReadOnlyList<string> Features { get; set; } = new List<string>();

        public IReadOnlyList<string> Bathrooms { get; set; } = new List<string>();

        public IReadOnlyList<string> SurfaceRanges { get; set; } = new List<string>();

        public IReadOnlyList<string> EnergyClasses { get; set; } = new List<string>();

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string View { get; set; }

        public string Sort { get; set; }
    }

    public record LeadRequestDto
    {
        public Guid? ClientUserId { get; set; }

        public string RequestType { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string PreferredContactMode { get; set; }

        public string PreferredTimeSlot { get; set; }

        public string PreferredDay { get; set; }

        public string PreferredTime { get; set; }

        public string SustainableBudget { get; set; }

        public string DesiredLocation { get; set; }

        public string AcceptableLocations { get; set; }

        public string PropertyType { get; set; }

        public string SearchStage { get; set; }

        public string FinancingStatus { get; set; }

        public string PropertyToSellStatus { get; set; }

        public string Timing { get; set; }

        public string PreferencesAndCompromises { get; set; }

        public string HouseholdDescription { get; set; }

        public string WorkStudySituation { get; set; }

        public string AvailableGuarantees { get; set; }

        public string DesiredMoveIn { get; set; }

        public string PropertyCondition { get; set; }

        public string Availability { get; set; }

        public string ExpectedPriceOrMainQuestion { get; set; }

        public string DesiredContractType { get; set; }

        public string IndicativeSurface { get; set; }

        public string Appurtenances { get; set; }

        public string ValuationGoal { get; set; }
    }

    public class LeadConfirmationDto
    {
        public Guid Id { get; set; }

        public string RequestType { get; set; }

        public string PreferredContactMode { get; set; }

        public string PreferredTimeSlot { get; set; }

        public string PreferredDay { get; set; }

        public string PreferredTime { get; set; }

        public IReadOnlyList<PropertySummaryDto> FeaturedProperties { get; set; } =
            new List<PropertySummaryDto>();
    }

    public class PropertyContactRequestDto
    {
        public Guid? ClientUserId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string PreferredContactMode { get; set; }

        public string PreferredTimeSlot { get; set; }

        public string PreferredDay { get; set; }

        public string PreferredTime { get; set; }

        public string Message { get; set; }
    }

    public class VisitPropertySummaryDto
    {
        public string Slug { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        public string Contract { get; set; }

        public string DisplayLocation { get; set; }

        public string Location { get; set; }

        public int SurfaceSquareMeters { get; set; }

        public int Rooms { get; set; }

        public string Status { get; set; }
    }

    public class VisitReviewDto
    {
        public Guid AppointmentId { get; set; }

        public VisitPropertySummaryDto Property { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }

        public string PreferredContactMode { get; set; }

        public string PreferredTimeSlot { get; set; }

        public string PreferredDay { get; set; }

        public string PreferredTime { get; set; }
    }

    public class VisitConfirmationDto
    {
        public Guid AppointmentId { get; set; }

        public VisitPropertySummaryDto Property { get; set; }

        public IReadOnlyList<PropertySummaryDto> SimilarProperties { get; set; } =
            new List<PropertySummaryDto>();
    }

}

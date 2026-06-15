using System;
using System.Collections.Generic;

namespace Medri.Services.Medri.Application
{
    public class SearchListingsCriteriaDto
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

        public string Sort { get; set; }

        public bool IncludeListings { get; set; } = true;

        public bool IncludeMapListings { get; set; } = true;

        public Guid? FocusListingId { get; set; }
    }

    public class SearchListingsResultDto
    {
        public IReadOnlyList<PropertySearchCardDto> Listings { get; set; } =
            new List<PropertySearchCardDto>();

        public IReadOnlyList<PropertySearchCardDto> MapListings { get; set; } =
            new List<PropertySearchCardDto>();
    }

    public class PropertySearchCardDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Status { get; set; }

        public decimal Price { get; set; }

        public string Contract { get; set; }

        public string DisplayLocation { get; set; }

        public string Location { get; set; }

        public int Rooms { get; set; }

        public int Bathrooms { get; set; }

        public int SurfaceSquareMeters { get; set; }

        public string ImageUrl { get; set; }

        public int SortOrder { get; set; }

        public int? FeaturedSortOrder { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool IsSaved { get; set; }
    }

    public class PropertySummaryDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Status { get; set; }

        public decimal Price { get; set; }

        public string Contract { get; set; }

        public string DisplayLocation { get; set; }

        public string Location { get; set; }

        public string ImageUrl { get; set; }
    }

    public class PropertyDetailDto : PropertySummaryDto
    {
        public bool IsSaved { get; set; }

        public string Address { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        public string AdvisorDisplayName { get; set; }

        public string AdvisorRole { get; set; }

        public string PropertyType { get; set; }

        public string Zone { get; set; }

        public int Rooms { get; set; }

        public int Bathrooms { get; set; }

        public int SurfaceSquareMeters { get; set; }

        public string BedroomsLabel { get; set; }

        public string FloorLabel { get; set; }

        public string GarageLabel { get; set; }

        public string OutdoorSpaceLabel { get; set; }

        public string EnergyClass { get; set; }

        public string AvailabilityLabel { get; set; }

        public string HeatingLabel { get; set; }

        public string RequiredWorksLabel { get; set; }

        public string ConstructionYearLabel { get; set; }

        public string CondoFeesLabel { get; set; }

        public string BalconyLabel { get; set; }

        public string CellarLabel { get; set; }

        public string NearbyServicesLabel { get; set; }

        public string SummaryTitle { get; set; }

        public string SummaryParagraph1 { get; set; }

        public string SummaryParagraph2 { get; set; }

        public string ReadinessNote { get; set; }

        public string CostsNote { get; set; }

        public string ContextNote { get; set; }

        public string DecisionMarginNote { get; set; }

        public string HumanFitNote { get; set; }

        public IReadOnlyList<PropertyMediaDto> Media { get; set; } =
            new List<PropertyMediaDto>();

        public IReadOnlyList<PropertySummaryDto> SimilarProperties { get; set; } =
            new List<PropertySummaryDto>();
    }

    public class PropertyMediaDto
    {
        public string Url { get; set; }

        public string AltText { get; set; }

        public int SortOrder { get; set; }
    }

    public class FavoritePropertyDto : PropertySummaryDto
    {
        public bool IsSaved { get; set; }

        public int Rooms { get; set; }

        public int Bathrooms { get; set; }

        public int SurfaceSquareMeters { get; set; }

        public string BedroomsLabel { get; set; }

        public string GarageLabel { get; set; }

        public string OutdoorSpaceLabel { get; set; }

        public string EnergyClass { get; set; }
    }

    public class ComparisonPropertyDto : PropertySummaryDto
    {
        public string ZoneComparisonLabel { get; set; }

        public string SurfaceRoomsComparisonLabel { get; set; }

        public string StatusWorksComparisonLabel { get; set; }

        public string OutdoorSpaceLabel { get; set; }

        public string EnergyClass { get; set; }

        public string MainCompromise { get; set; }

        public string BedroomsLabel { get; set; }

        public int Bathrooms { get; set; }

        public string AccessLabel { get; set; }

        public string GarageLabel { get; set; }

        public string HeatingLabel { get; set; }

        public string ConstructionYearLabel { get; set; }

        public string ManagementCostsLabel { get; set; }

        public string EstimatedWorksLabel { get; set; }

        public string EnergyCostsLabel { get; set; }

        public string PersonalizationLabel { get; set; }

        public string AvailabilityLabel { get; set; }

        public string NearbyServicesLabel { get; set; }

        public string TransportLabel { get; set; }

        public string PrivacyLabel { get; set; }

        public string NoiseLabel { get; set; }

        public string IdealTargetLabel { get; set; }
    }
}

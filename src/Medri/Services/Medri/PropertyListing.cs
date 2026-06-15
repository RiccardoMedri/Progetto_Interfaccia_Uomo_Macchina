using System;
using System.ComponentModel.DataAnnotations;

namespace Medri.Services.Medri
{
    public class PropertyListing
    {
        [Key]
        public Guid Id { get; set; }

        [Required, StringLength(160)]
        public string Title { get; set; }

        [Required, StringLength(160)]
        public string Slug { get; set; }

        [StringLength(30)]
        public string InternalReference { get; set; }

        [StringLength(80)]
        public string PublicationStatus { get; set; }

        [Range(0, 100)]
        public int CompletionPercent { get; set; }

        [StringLength(300)]
        public string MissingItems { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        public int? FeaturedSortOrder { get; set; }

        [Required, StringLength(160)]
        public string Location { get; set; }

        [Required, StringLength(160)]
        public string DisplayLocation { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal Price { get; set; }

        [Range(0, 100)]
        public int Rooms { get; set; }

        [Range(0, 100)]
        public int Bathrooms { get; set; }

        [Range(0, 100000)]
        public int SurfaceSquareMeters { get; set; }

        [Required, StringLength(80)]
        public string Status { get; set; }

        [Required, StringLength(30)]
        public string Contract { get; set; }

        [Required, StringLength(80)]
        public string PropertyType { get; set; }

        [StringLength(80)]
        public string ListingCategory { get; set; }

        [Required, StringLength(80)]
        public string Zone { get; set; }

        [Required, StringLength(500)]
        public string FeatureKeys { get; set; }

        [Required, StringLength(300)]
        public string ImageUrl { get; set; }

        [Range(-90d, 90d)]
        public double Latitude { get; set; }

        [Range(-180d, 180d)]
        public double Longitude { get; set; }

        public int SortOrder { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public string Address { get; set; }

        public string BedroomsLabel { get; set; }

        public string FloorLabel { get; set; }

        public string ElevatorLabel { get; set; }

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

        public string ZoneComparisonLabel { get; set; }

        public string SurfaceRoomsComparisonLabel { get; set; }

        public string StatusWorksComparisonLabel { get; set; }

        public string MainCompromise { get; set; }

        public string AccessLabel { get; set; }

        public string ManagementCostsLabel { get; set; }

        public string EstimatedWorksLabel { get; set; }

        public string EnergyCostsLabel { get; set; }

        public string PersonalizationLabel { get; set; }

        public string TransportLabel { get; set; }

        public string PrivacyLabel { get; set; }

        public string NoiseLabel { get; set; }

        public string IdealTargetLabel { get; set; }
    }
}

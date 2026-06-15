using System.Collections.Generic;

namespace Medri.Web.Features.Property
{
    public class PropertyDetailViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Tag { get; set; }

        public bool IsSaved { get; set; }

        public string PriceLabel { get; set; }

        public string Address { get; set; }

        public IReadOnlyList<PropertyDetailMediaViewModel> Media { get; set; } =
            new List<PropertyDetailMediaViewModel>();

        public bool HasMedia => Media.Count > 0;

        public string RoomsLabel { get; set; }

        public string BedroomsLabel { get; set; }

        public string BathroomsLabel { get; set; }

        public string SurfaceLabel { get; set; }

        public string StatusLabel { get; set; }

        public string OutdoorSpaceLabel { get; set; }

        public string EnergyClass { get; set; }

        public string AvailabilityLabel { get; set; }

        public string FloorLabel { get; set; }

        public string GarageLabel { get; set; }

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

        public string SearchReturnUrl { get; set; } = "/immobili";

        public string MapUrl { get; set; }

        public string RequestInfoUrl { get; set; }

        public string AdvisorDisplayName { get; set; }

        public string AdvisorRole { get; set; }

        public bool HasAdvisor => !string.IsNullOrWhiteSpace(AdvisorDisplayName);

        public string AgencyPhoneLabel { get; set; }

        public string AgencyPhoneHref { get; set; }

        public bool HasAgencyPhone =>
            !string.IsNullOrWhiteSpace(AgencyPhoneLabel) &&
            !string.IsNullOrWhiteSpace(AgencyPhoneHref);

        public IReadOnlyList<PropertyDetailTechnicalSectionViewModel> TechnicalSections { get; set; } =
            new List<PropertyDetailTechnicalSectionViewModel>();

        public IReadOnlyList<PropertySummaryCardViewModel> SimilarProperties { get; set; } =
            new List<PropertySummaryCardViewModel>();
    }

    public class PropertyDetailMediaViewModel
    {
        public string Url { get; set; }

        public string AltText { get; set; }

        public string Label { get; set; }
    }

    public class PropertyDetailTechnicalSectionViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public bool IsExpanded { get; set; }

        public IReadOnlyList<PropertyDetailFactViewModel> Items { get; set; } =
            new List<PropertyDetailFactViewModel>();
    }

    public class PropertyDetailFactViewModel
    {
        public string Label { get; set; }

        public string Value { get; set; }
    }
}

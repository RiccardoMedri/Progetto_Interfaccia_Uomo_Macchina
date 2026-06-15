using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace Medri.Web.Features.Search
{
    public class SearchIndexViewModel
    {
        public SearchFiltersViewModel Filters { get; set; } = new SearchFiltersViewModel();

        public SearchFilterOptionsViewModel AvailableFilters { get; set; } = new SearchFilterOptionsViewModel();

        public IReadOnlyList<SearchResultCardViewModel> Results { get; set; } = new List<SearchResultCardViewModel>();

        public IReadOnlyList<SearchMapMarkerViewModel> Markers { get; set; } = new List<SearchMapMarkerViewModel>();

        public string GoogleMapsBrowserApiKey { get; set; }

        public string GoogleMapsMapId { get; set; }

        public string FavoriteReturnUrl { get; set; }

        public bool IsSavedSearch { get; set; }

        public string BodyClass => $"medri-search-body search-mode-{Filters.NormalizedView}";

        public string ResultsStackClasses => Filters.IsMapView
            ? "col-12 search-results-stack-map"
            : Filters.IsListView
                ? "col-12"
                : "col-12 col-lg-5";

        public string ResultsPanelClasses => Filters.IsMapView ? "d-none" : string.Empty;

        public string MapPanelClasses => Filters.IsListView
            ? "d-none"
            : Filters.IsMapView
                ? "col-12"
                : "col-12 col-lg-7";

        public string ResultsGridClasses => Filters.IsListView
            ? "row row-cols-1 row-cols-md-2 row-cols-lg-3 row-cols-xl-5 g-3"
            : "row row-cols-1 row-cols-md-2 g-3";

        public IReadOnlyList<SearchResultCardViewModel> VisibleResults => Results;

        public bool IsGoogleMapsConfigured =>
            !Filters.IsListView &&
            !string.IsNullOrWhiteSpace(GoogleMapsBrowserApiKey) &&
            !string.IsNullOrWhiteSpace(GoogleMapsMapId);

        public string BreadcrumbSuffix => Filters.NormalizedView switch
        {
            "list" => "Lista",
            "map" => "Mappa",
            _ => null
        };

        public string SelectedComparisonCountLabel => "0 immobili selezionati";

        public SearchMapMarkerViewModel SelectedMarker =>
            Markers.FirstOrDefault(marker => marker.Id == Filters.SelectedId) ??
            Markers.FirstOrDefault();

        public SaveSearchFormViewModel SaveSearchForm => SaveSearchFormViewModel.Create(
            "saveSearchDesktop",
            Filters,
            FavoriteReturnUrl);

        public SearchMultiSelectViewModel ToolbarContractsFilter =>
            ToolbarFilter("search-contracts", "contracts", "Contratto", AvailableFilters.Contracts, Filters.Contracts);

        public SearchMultiSelectViewModel ToolbarPropertyTypesFilter =>
            ToolbarFilter("search-property-types", "propertyTypes", "Tipologia", AvailableFilters.PropertyTypes, Filters.PropertyTypes);

        public SearchMultiSelectViewModel ToolbarRoomsFilter =>
            ToolbarFilter("search-rooms", "rooms", "Locali", AvailableFilters.Rooms, Filters.Rooms);

        public string FavoriteAction(bool isSaved)
        {
            return isSaved ? "/preferiti/rimuovi" : "/preferiti/aggiungi";
        }

        public string MapConfigJson()
        {
            return JsonSerializer.Serialize(
                new
                {
                    apiKey = GoogleMapsBrowserApiKey,
                    mapId = GoogleMapsMapId,
                    isConfigured = IsGoogleMapsConfigured,
                    selectedId = Filters.SelectedId,
                    markers = Markers
                },
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        private static SearchMultiSelectViewModel ToolbarFilter(
            string id,
            string name,
            string placeholder,
            IReadOnlyList<SearchFilterOptionViewModel> options,
            IReadOnlyCollection<string> selectedValues)
        {
            return new SearchMultiSelectViewModel
            {
                Id = id,
                Name = name,
                Placeholder = placeholder,
                Options = options,
                SelectedValues = selectedValues
            };
        }
    }

    public class SaveSearchFormViewModel
    {
        public string FormId { get; set; }

        public IReadOnlyList<HiddenInputViewModel> HiddenInputs { get; set; } =
            new List<HiddenInputViewModel>();

        public static SaveSearchFormViewModel Create(
            string formId,
            SearchFiltersViewModel filters,
            string returnUrl)
        {
            var inputs = new List<HiddenInputViewModel>();
            AddValues(inputs, "contracts", filters.Contracts);
            AddValues(inputs, "propertyTypes", filters.PropertyTypes);
            AddValues(inputs, "rooms", filters.Rooms);
            AddValues(inputs, "zones", filters.Zones);
            AddValues(inputs, "priceRanges", filters.PriceRanges);
            AddValues(inputs, "features", filters.Features);
            AddValues(inputs, "bathrooms", filters.Bathrooms);
            AddValues(inputs, "surfaceRanges", filters.SurfaceRanges);
            AddValues(inputs, "energyClasses", filters.EnergyClasses);
            AddValue(inputs, "minPrice", filters.MinPrice?.ToString("0", CultureInfo.InvariantCulture));
            AddValue(inputs, "maxPrice", filters.MaxPrice?.ToString("0", CultureInfo.InvariantCulture));
            AddValue(inputs, "view", filters.NormalizedView);

            if (!filters.IsMapView)
            {
                AddValue(inputs, "sort", filters.NormalizedSort);
            }

            AddValue(inputs, "returnUrl", returnUrl);

            return new SaveSearchFormViewModel
            {
                FormId = formId,
                HiddenInputs = inputs
            };
        }

        private static void AddValues(
            ICollection<HiddenInputViewModel> inputs,
            string name,
            IEnumerable<string> values)
        {
            foreach (var value in values ?? Enumerable.Empty<string>())
            {
                AddValue(inputs, name, value);
            }
        }

        private static void AddValue(
            ICollection<HiddenInputViewModel> inputs,
            string name,
            string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                inputs.Add(new HiddenInputViewModel { Name = name, Value = value });
            }
        }
    }

    public class HiddenInputViewModel
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}

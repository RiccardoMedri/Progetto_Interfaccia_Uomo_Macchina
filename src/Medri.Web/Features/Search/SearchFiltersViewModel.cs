using System;
using System.Collections.Generic;
using System.Linq;

namespace Medri.Web.Features.Search
{
    public class SearchFiltersViewModel
    {
        public string View { get; set; }

        public string Sort { get; set; }

        public string SelectedId { get; set; }

        public string FocusId { get; set; }

        public List<string> Contracts { get; set; } = new List<string>();

        public List<string> PropertyTypes { get; set; } = new List<string>();

        public List<string> Rooms { get; set; } = new List<string>();

        public List<string> Zones { get; set; } = new List<string>();

        public List<string> PriceRanges { get; set; } = new List<string>();

        public List<string> Features { get; set; } = new List<string>();

        public List<string> Bathrooms { get; set; } = new List<string>();

        public List<string> SurfaceRanges { get; set; } = new List<string>();

        public List<string> EnergyClasses { get; set; } = new List<string>();

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string NormalizedView
        {
            get
            {
                var value = View?.Trim().ToLowerInvariant();
                return value == "list" || value == "map" ? value : "split";
            }
        }

        public bool IsListView => NormalizedView == "list";

        public bool IsMapView => NormalizedView == "map";

        public string NormalizedSort
        {
            get
            {
                var value = Sort?.Trim().ToLowerInvariant();
                return value == "price-asc" ||
                    value == "price-desc" ||
                    value == "size-asc" ||
                    value == "size-desc"
                        ? value
                        : null;
            }
        }

        public string ToUrl(string view = null, string sort = null, string selectedId = null)
        {
            var query = new List<string>();
            var nextView = string.IsNullOrWhiteSpace(view) ? NormalizedView : view;
            var nextSort = nextView == "map" ? null : sort ?? NormalizedSort;
            var nextSelectedId = selectedId ?? SelectedId;
            var nextFocusId = FocusId;

            if (nextView != "split")
            {
                query.Add("view=" + Uri.EscapeDataString(nextView));
            }

            if (!string.IsNullOrWhiteSpace(nextSort))
            {
                query.Add("sort=" + Uri.EscapeDataString(nextSort));
            }

            if (nextView == "map" && !string.IsNullOrWhiteSpace(nextSelectedId))
            {
                query.Add("selectedId=" + Uri.EscapeDataString(nextSelectedId));
            }

            if (nextView == "map" && !string.IsNullOrWhiteSpace(nextFocusId))
            {
                query.Add("focusId=" + Uri.EscapeDataString(nextFocusId));
            }

            AddQueryValues(query, "contracts", Contracts);
            AddQueryValues(query, "propertyTypes", PropertyTypes);
            AddQueryValues(query, "rooms", Rooms);
            AddQueryValues(query, "zones", Zones);
            AddQueryValues(query, "priceRanges", PriceRanges);
            AddQueryValues(query, "features", Features);
            AddQueryValues(query, "bathrooms", Bathrooms);
            AddQueryValues(query, "surfaceRanges", SurfaceRanges);
            AddQueryValues(query, "energyClasses", EnergyClasses);
            AddQueryValue(query, "minPrice", MinPrice);
            AddQueryValue(query, "maxPrice", MaxPrice);

            return query.Count == 0 ? "/immobili" : "/immobili?" + string.Join("&", query);
        }

        private static void AddQueryValue(ICollection<string> query, string name, decimal? value)
        {
            if (value.HasValue)
            {
                query.Add(Uri.EscapeDataString(name) + "=" + Uri.EscapeDataString(value.Value.ToString("0", System.Globalization.CultureInfo.InvariantCulture)));
            }
        }

        private static void AddQueryValues(
            ICollection<string> query,
            string name,
            IEnumerable<string> values)
        {
            foreach (var value in (values ?? Array.Empty<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                query.Add(Uri.EscapeDataString(name) + "=" + Uri.EscapeDataString(value));
            }
        }
    }
}

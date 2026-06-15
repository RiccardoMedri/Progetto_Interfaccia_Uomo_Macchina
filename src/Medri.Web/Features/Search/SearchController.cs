using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;

namespace Medri.Web.Features.Search
{
    public partial class SearchController : Controller
    {
        private readonly SearchListingsQuery searchListingsQuery;
        private readonly GoogleMapsOptions googleMapsOptions;

        public SearchController(
            SearchListingsQuery searchListingsQuery,
            IOptions<GoogleMapsOptions> googleMapsOptions)
        {
            this.searchListingsQuery = searchListingsQuery;
            this.googleMapsOptions = googleMapsOptions.Value;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index([FromQuery] SearchFiltersViewModel filters)
        {
            filters ??= new SearchFiltersViewModel();
            var focusListingId = ParseGuid(filters.FocusId);
            if (focusListingId.HasValue && string.IsNullOrWhiteSpace(filters.SelectedId))
            {
                filters.FocusId = focusListingId.Value.ToString();
                filters.SelectedId = filters.FocusId;
            }

            var result = await searchListingsQuery.ExecuteAsync(
                new SearchListingsCriteriaDto
                {
                    Contracts = filters.Contracts,
                    PropertyTypes = filters.PropertyTypes,
                    Rooms = filters.Rooms,
                    Zones = filters.Zones,
                    PriceRanges = filters.PriceRanges,
                    Features = filters.Features,
                    Bathrooms = filters.Bathrooms,
                    SurfaceRanges = filters.SurfaceRanges,
                    EnergyClasses = filters.EnergyClasses,
                    MinPrice = filters.MinPrice,
                    MaxPrice = filters.MaxPrice,
                    Sort = filters.IsMapView ? null : filters.NormalizedSort,
                    IncludeListings = !filters.IsMapView,
                    IncludeMapListings = !filters.IsListView,
                    FocusListingId = focusListingId
                },
                AuthenticatedUserId.Get(User),
                HttpContext.RequestAborted);
            var model = SearchIndexViewModelMapper.Create(filters, result, googleMapsOptions);
            model.FavoriteReturnUrl = HttpContext.Request.Path.Value + HttpContext.Request.QueryString.Value;
            model.IsSavedSearch = HttpContext.Request.Query["savedSearch"] == "1";
            return View(model);
        }

        private static Guid? ParseGuid(string value)
        {
            return Guid.TryParse(value, out var parsed) ? parsed : null;
        }
    }
}

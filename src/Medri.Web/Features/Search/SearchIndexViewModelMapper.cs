using System.Globalization;
using System.Linq;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;
using Medri.Web.Features.Property;

namespace Medri.Web.Features.Search
{
    internal static class SearchIndexViewModelMapper
    {
        public static SearchIndexViewModel Create(
            SearchFiltersViewModel filters,
            SearchListingsResultDto result,
            GoogleMapsOptions googleMapsOptions)
        {
            var cards = result.Listings.Select(listing => new SearchResultCardViewModel
            {
                Id = listing.Id.ToString(),
                Title = listing.Title,
                Slug = listing.Slug,
                Tag = listing.Status,
                PriceLabel = PropertyFormatting.FormatPrice(listing.Price, listing.Contract),
                DisplayLocation = PropertyFormatting.DisplayOrFallback(listing.DisplayLocation, listing.Location),
                FactsLabel = $"{listing.Rooms} locali - {listing.Bathrooms} bagni - {listing.SurfaceSquareMeters} mq",
                MobileFactsLabel = $"{System.Math.Max(listing.Rooms - 1, 1)} camere - {listing.Bathrooms} bagni - {listing.SurfaceSquareMeters} mq",
                ImageUrl = listing.ImageUrl,
                IsSaved = listing.IsSaved
            }).ToList();

            var markers = result.MapListings.Select((listing, index) => new SearchMapMarkerViewModel
            {
                Id = listing.Id.ToString(),
                Label = (index + 1).ToString(CultureInfo.InvariantCulture),
                Title = listing.Title,
                Tag = listing.Status,
                PriceLabel = PropertyFormatting.FormatPrice(listing.Price, listing.Contract),
                DisplayLocation = PropertyFormatting.DisplayOrFallback(listing.DisplayLocation, listing.Location),
                FactsLabel = $"{listing.Rooms} locali - {listing.Bathrooms} bagni - {listing.SurfaceSquareMeters} mq",
                ImageUrl = listing.ImageUrl,
                Latitude = listing.Latitude,
                Longitude = listing.Longitude,
                DetailUrl = "/immobili/" + listing.Slug
            }).ToList();

            return new SearchIndexViewModel
            {
                Filters = filters,
                AvailableFilters = new SearchFilterOptionsViewModel(),
                Results = cards,
                Markers = markers,
                GoogleMapsBrowserApiKey = googleMapsOptions.BrowserApiKey,
                GoogleMapsMapId = googleMapsOptions.MapId
            };
        }
    }
}

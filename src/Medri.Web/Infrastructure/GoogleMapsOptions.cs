namespace Medri.Web.Infrastructure
{
    public class GoogleMapsOptions
    {
        public string PlacesApiKey { get; set; }

        // Dedicated key for the server-side Geocoding API (address -> coordinates).
        // Falls back to PlacesApiKey if left empty.
        public string GeocodingApiKey { get; set; }

        public string AgencyPlaceId { get; set; }

        public string BrowserApiKey { get; set; }

        public string MapId { get; set; }
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GoogleMapsOptions = Medri.Web.Infrastructure.GoogleMapsOptions;

namespace Medri.Web.Areas.Admin.Properties
{
    public interface IAdminPropertyGeocoder
    {
        Task<AdminPropertyGeocodeResult> GeocodeAsync(
            string address,
            string displayLocation,
            CancellationToken cancellationToken);
    }

    public class GoogleAdminPropertyGeocoder : IAdminPropertyGeocoder
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient httpClient;
        private readonly GoogleMapsOptions options;
        private readonly ILogger<GoogleAdminPropertyGeocoder> logger;

        public GoogleAdminPropertyGeocoder(
            HttpClient httpClient,
            IOptions<GoogleMapsOptions> options,
            ILogger<GoogleAdminPropertyGeocoder> logger)
        {
            this.httpClient = httpClient;
            this.options = options.Value;
            this.logger = logger;
        }

        public async Task<AdminPropertyGeocodeResult> GeocodeAsync(
            string address,
            string displayLocation,
            CancellationToken cancellationToken)
        {
            var queries = BuildQueries(address, displayLocation);
            if (queries.Count == 0)
            {
                return AdminPropertyGeocodeResult.Failure(AdminPropertyLocationMessages.PreciseAddressRequired);
            }

            var apiKey = string.IsNullOrWhiteSpace(options.GeocodingApiKey)
                ? options.PlacesApiKey
                : options.GeocodingApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                logger.LogWarning("Google geocoding skipped because GoogleMaps:PlacesApiKey is not configured.");
                return AdminPropertyGeocodeResult.Failure(AdminPropertyLocationMessages.MapUnavailable);
            }

            foreach (var query in queries)
            {
                var result = await GeocodeQueryAsync(query, apiKey.Trim(), cancellationToken);
                if (result.Succeeded)
                {
                    return AdminPropertyGeocodeResult.Success(result.Latitude.GetValueOrDefault(), result.Longitude.GetValueOrDefault());
                }

                if (result.IsUnavailable)
                {
                    return AdminPropertyGeocodeResult.Failure(AdminPropertyLocationMessages.MapUnavailable);
                }
            }

            return AdminPropertyGeocodeResult.Failure(AdminPropertyLocationMessages.AddressNotFound);
        }

        private async Task<GeocodeQueryResult> GeocodeQueryAsync(
            string query,
            string apiKey,
            CancellationToken cancellationToken)
        {
            try
            {
                using var response = await httpClient.GetAsync(CreateRequestUrl(query, apiKey), cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Google geocoding request failed with status code {StatusCode}.", response.StatusCode);
                    return GeocodeQueryResult.Unavailable();
                }

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var payload = await JsonSerializer.DeserializeAsync<GoogleGeocodeResponse>(
                    stream,
                    JsonOptions,
                    cancellationToken);

                if (!string.Equals(payload?.Status, "OK", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.Equals(payload?.Status, "ZERO_RESULTS", StringComparison.OrdinalIgnoreCase))
                    {
                        return GeocodeQueryResult.NotFound();
                    }

                    logger.LogWarning("Google geocoding returned status {Status}.", payload?.Status);
                    return GeocodeQueryResult.Unavailable();
                }

                var location = payload.Results?
                    .Select(result => result?.Geometry?.Location)
                    .FirstOrDefault(IsUsableCoordinates);

                return location == null
                    ? GeocodeQueryResult.NotFound()
                    : GeocodeQueryResult.Success(location.Lat.GetValueOrDefault(), location.Lng.GetValueOrDefault());
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning("Google geocoding request timed out.");
                return GeocodeQueryResult.Unavailable();
            }
            catch (HttpRequestException exception)
            {
                logger.LogWarning(exception, "Google geocoding request failed.");
                return GeocodeQueryResult.Unavailable();
            }
            catch (JsonException exception)
            {
                logger.LogWarning(exception, "Google geocoding response could not be parsed.");
                return GeocodeQueryResult.Unavailable();
            }
        }

        private static string CreateRequestUrl(string query, string apiKey)
        {
            return "json?address=" +
                Uri.EscapeDataString(query) +
                "&components=country:IT&region=it&language=it&key=" +
                Uri.EscapeDataString(apiKey);
        }

        private static List<string> BuildQueries(string address, string displayLocation)
        {
            var normalizedAddress = NormalizeLocationPart(address);
            if (string.IsNullOrWhiteSpace(normalizedAddress))
            {
                return new List<string>();
            }

            var normalizedDisplayLocation = NormalizeLocationPart(displayLocation);
            var city = CityFromDisplayLocation(normalizedDisplayLocation);
            var province = ProvinceHint(city);
            var addressHasCity = HasLocationPart(normalizedAddress, city);
            var addressHasProvince = HasLocationPart(normalizedAddress, province);

            return new[]
                {
                    CompactQueryParts(normalizedAddress, addressHasCity ? string.Empty : city, addressHasProvince ? string.Empty : province, "Italia"),
                    CompactQueryParts(normalizedAddress, addressHasCity ? string.Empty : city, "Italia"),
                    CompactQueryParts(normalizedAddress, normalizedDisplayLocation, "Italia"),
                    CompactQueryParts(normalizedAddress, "Italia")
                }
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string NormalizeLocationPart(string value)
        {
            return string.Join(
                " ",
                (value ?? string.Empty)
                    .Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private static string CityFromDisplayLocation(string value)
        {
            var normalized = NormalizeLocationPart(value);
            var separatorIndex = normalized.IndexOf(" - ", StringComparison.Ordinal);
            return separatorIndex < 0
                ? normalized
                : normalized.Substring(0, separatorIndex).Trim();
        }

        private static bool HasLocationPart(string value, string part)
        {
            return !string.IsNullOrWhiteSpace(part) &&
                value?.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string ProvinceHint(string city)
        {
            return string.Equals(city, "Cesena", StringComparison.OrdinalIgnoreCase)
                ? "FC"
                : string.Empty;
        }

        private static string CompactQueryParts(params string[] parts)
        {
            return string.Join(
                ", ",
                parts
                    .Select(NormalizeLocationPart)
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Distinct(StringComparer.OrdinalIgnoreCase));
        }

        private static bool IsUsableCoordinates(GoogleGeocodeLocation location)
        {
            var latitude = location?.Lat;
            var longitude = location?.Lng;
            return latitude.HasValue &&
                longitude.HasValue &&
                latitude.Value >= -90 &&
                latitude.Value <= 90 &&
                longitude.Value >= -180 &&
                longitude.Value <= 180 &&
                (latitude.Value != 0 || longitude.Value != 0);
        }

        private class GeocodeQueryResult
        {
            public bool Succeeded { get; private init; }

            public bool IsUnavailable { get; private init; }

            public double? Latitude { get; private init; }

            public double? Longitude { get; private init; }

            public static GeocodeQueryResult Success(double latitude, double longitude)
            {
                return new GeocodeQueryResult
                {
                    Succeeded = true,
                    Latitude = latitude,
                    Longitude = longitude
                };
            }

            public static GeocodeQueryResult NotFound()
            {
                return new GeocodeQueryResult();
            }

            public static GeocodeQueryResult Unavailable()
            {
                return new GeocodeQueryResult
                {
                    IsUnavailable = true
                };
            }
        }

        private class GoogleGeocodeResponse
        {
            public string Status { get; set; }

            public List<GoogleGeocodeResult> Results { get; set; }
        }

        private class GoogleGeocodeResult
        {
            public GoogleGeocodeGeometry Geometry { get; set; }
        }

        private class GoogleGeocodeGeometry
        {
            public GoogleGeocodeLocation Location { get; set; }
        }

        private class GoogleGeocodeLocation
        {
            public double? Lat { get; set; }

            public double? Lng { get; set; }
        }
    }

    public class AdminPropertyGeocodeInputModel
    {
        public string Address { get; set; }

        public string DisplayLocation { get; set; }
    }

    public class AdminPropertyGeocodeResult
    {
        private AdminPropertyGeocodeResult()
        {
        }

        public bool Succeeded { get; private init; }

        public double? Latitude { get; private init; }

        public double? Longitude { get; private init; }

        public string Message { get; private init; }

        public static AdminPropertyGeocodeResult Success(double latitude, double longitude)
        {
            return new AdminPropertyGeocodeResult
            {
                Succeeded = true,
                Latitude = latitude,
                Longitude = longitude,
                Message = string.Empty
            };
        }

        public static AdminPropertyGeocodeResult Failure(string message)
        {
            return new AdminPropertyGeocodeResult
            {
                Message = message
            };
        }
    }

    public static class AdminPropertyLocationMessages
    {
        public const string PreciseAddressRequired = "Inserisci un indirizzo preciso.";

        public const string AddressNotFound = "Indirizzo non trovato. Inserisci via, numero civico, comune e provincia.";

        public const string MapUnavailable = "Mappa non disponibile.";
    }
}

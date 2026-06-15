using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GoogleMapsOptions = Medri.Web.Infrastructure.GoogleMapsOptions;

namespace Medri.Web.Features.Home
{
    public class GooglePlacesReviewsService : IGoogleReviewsService
    {
        private const string FieldMask = "reviews.text,reviews.rating";

        private readonly HttpClient httpClient;
        private readonly GoogleMapsOptions options;
        private readonly ILogger<GooglePlacesReviewsService> logger;

        public GooglePlacesReviewsService(
            HttpClient httpClient,
            IOptions<GoogleMapsOptions> options,
            ILogger<GooglePlacesReviewsService> logger)
        {
            this.httpClient = httpClient;
            this.options = options.Value;
            this.logger = logger;
        }

        public async Task<GoogleReviewsSummaryViewModel> GetHomeReviewsAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(options.PlacesApiKey) || string.IsNullOrWhiteSpace(options.AgencyPlaceId))
            {
                return GoogleReviewsSummaryViewModel.Unconfigured();
            }

            var placeResourceName = NormalizePlaceResourceName(options.AgencyPlaceId);
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{placeResourceName}?languageCode=it&regionCode=IT");
            request.Headers.TryAddWithoutValidation("X-Goog-Api-Key", options.PlacesApiKey);
            request.Headers.TryAddWithoutValidation("X-Goog-FieldMask", FieldMask);

            try
            {
                using var response = await httpClient.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Google Places Place Details request failed with status code {StatusCode}.", response.StatusCode);
                    return GoogleReviewsSummaryViewModel.Unavailable();
                }

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var place = await JsonSerializer.DeserializeAsync<GooglePlaceDetailsResponse>(
                    stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                    cancellationToken);

                if (place == null)
                {
                    logger.LogWarning("Google Places Place Details returned an empty response.");
                    return GoogleReviewsSummaryViewModel.Unavailable();
                }

                return MapPlace(place);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning("Google Places Place Details request timed out.");
                return GoogleReviewsSummaryViewModel.Unavailable();
            }
            catch (HttpRequestException exception)
            {
                logger.LogWarning(exception, "Google Places Place Details request failed.");
                return GoogleReviewsSummaryViewModel.Unavailable();
            }
            catch (JsonException exception)
            {
                logger.LogWarning(exception, "Google Places Place Details response could not be parsed.");
                return GoogleReviewsSummaryViewModel.Unavailable();
            }
        }

        private static GoogleReviewsSummaryViewModel MapPlace(GooglePlaceDetailsResponse place)
        {
            var reviews = place.Reviews?
                .Where(review => review != null && review.Rating.GetValueOrDefault() >= 3)
                .Take(5)
                .Select(MapReview)
                .ToList() ?? new List<GoogleReviewViewModel>();

            return new GoogleReviewsSummaryViewModel
            {
                Reviews = reviews
            };
        }

        private static GoogleReviewViewModel MapReview(GooglePlaceReviewResponse review)
        {
            var reviewText = review.Text?.Text;
            return new GoogleReviewViewModel
            {
                Rating = review.Rating.HasValue
                    ? Math.Clamp((int)Math.Round(review.Rating.Value, MidpointRounding.AwayFromZero), 0, 5)
                    : 0,
                RatingLabel = review.Rating.HasValue
                    ? review.Rating.Value.ToString("0.#", CultureInfo.InvariantCulture) + "/5"
                    : "-",
                Text = GoogleReviewViewModel.Truncate(reviewText, 165)
            };
        }

        private static string NormalizePlaceResourceName(string agencyPlaceId)
        {
            var trimmed = agencyPlaceId.Trim();
            return trimmed.StartsWith("places/", StringComparison.OrdinalIgnoreCase)
                ? trimmed
                : $"places/{trimmed}";
        }

        private class GooglePlaceDetailsResponse
        {
            public List<GooglePlaceReviewResponse> Reviews { get; set; }
        }

        private class GooglePlaceReviewResponse
        {
            public LocalizedText Text { get; set; }

            public double? Rating { get; set; }
        }

        private class LocalizedText
        {
            public string Text { get; set; }
        }
    }
}

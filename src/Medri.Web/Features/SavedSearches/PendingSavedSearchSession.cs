using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using Medri.Services.Medri.Application;

namespace Medri.Web.Features.SavedSearches
{
    internal static class PendingSavedSearchSession
    {
        private const string SessionKey = "Medri.PendingSavedSearch";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static void Store(ISession session, SaveSearchInputModel input, string returnUrl)
        {
            var pending = new PendingSavedSearch
            {
                Input = input,
                ReturnUrl = returnUrl
            };
            session.SetString(SessionKey, JsonSerializer.Serialize(pending, JsonOptions));
        }

        public static bool TryRead(ISession session, out SaveSearchRequestDto request, out string returnUrl)
        {
            request = null;
            returnUrl = null;
            var value = session.GetString(SessionKey);
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            try
            {
                var pending = JsonSerializer.Deserialize<PendingSavedSearch>(value, JsonOptions);
                if (pending?.Input == null)
                {
                    return false;
                }

                request = ToRequest(pending.Input);
                returnUrl = pending.ReturnUrl;
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }

        public static SaveSearchRequestDto ToRequest(SaveSearchInputModel input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return new SaveSearchRequestDto
            {
                Contracts = input.Contracts,
                PropertyTypes = input.PropertyTypes,
                Rooms = input.Rooms,
                Zones = input.Zones,
                PriceRanges = input.PriceRanges,
                Features = input.Features,
                Bathrooms = input.Bathrooms,
                SurfaceRanges = input.SurfaceRanges,
                EnergyClasses = input.EnergyClasses,
                MinPrice = input.MinPrice,
                MaxPrice = input.MaxPrice,
                View = input.View,
                Sort = input.Sort
            };
        }

        public static void Clear(ISession session) => session.Remove(SessionKey);

        private sealed class PendingSavedSearch
        {
            public SaveSearchInputModel Input { get; set; }

            public string ReturnUrl { get; set; }
        }
    }
}

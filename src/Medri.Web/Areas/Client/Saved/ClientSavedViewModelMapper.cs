using System.Linq;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Client;
using Medri.Web.Features.Property;

namespace Medri.Web.Areas.Client.Saved
{
    internal static class ClientSavedViewModelMapper
    {
        public static ClientSavedViewModel Create(
            System.Collections.Generic.IReadOnlyList<FavoritePropertyDto> properties,
            int requestCount)
        {
            return new ClientSavedViewModel
            {
                Navigation = ClientAreaNavigationViewModel.Create(
                    ClientAreaTabs.Saved,
                    properties.Count,
                    requestCount),
                Properties = properties.Select(property => new ClientSavedPropertyViewModel
                {
                    Id = property.Id,
                    Title = property.Title,
                    Contract = property.Contract,
                    PriceLabel = PropertyFormatting.FormatPrice(property.Price, property.Contract),
                    DisplayLocation = PropertyFormatting.DisplayOrFallback(property.DisplayLocation, property.Location),
                    ImageUrl = property.ImageUrl,
                    Slug = property.Slug,
                    BedroomsLabel = string.IsNullOrWhiteSpace(property.BedroomsLabel)
                        ? $"{property.Rooms} locali"
                        : $"{property.BedroomsLabel} camere",
                    BathroomsLabel = property.Bathrooms == 1 ? "1 bagno" : $"{property.Bathrooms} bagni",
                    SurfaceLabel = $"{property.SurfaceSquareMeters} mq",
                    FourthFactLabel = GetFourthFact(property),
                    SavedNote = GetSavedNote(property)
                }).ToArray()
            };
        }

        private static string GetFourthFact(FavoritePropertyDto property)
        {
            if (!string.IsNullOrWhiteSpace(property.GarageLabel) &&
                property.GarageLabel != "No")
            {
                return property.GarageLabel;
            }

            if (!string.IsNullOrWhiteSpace(property.OutdoorSpaceLabel) &&
                property.OutdoorSpaceLabel != "No")
            {
                return property.OutdoorSpaceLabel;
            }

            return string.IsNullOrWhiteSpace(property.EnergyClass)
                ? $"{property.Rooms} locali"
                : $"Classe {property.EnergyClass}";
        }

        private static string GetSavedNote(FavoritePropertyDto property)
        {
            var location = PropertyFormatting.DisplayOrFallback(property.DisplayLocation, property.Location);
            return $"Scheda salvata: {location}, {property.SurfaceSquareMeters} mq, {GetFourthFact(property)}.";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;

namespace Medri.Web.Features.Property
{
    internal static class PropertyViewModelMapper
    {
        public static PropertyDetailViewModel Create(
            PropertyDetailDto property,
            AgencyContactOptions agencyContactOptions,
            bool isAdminPreview = false)
        {
            var model = new PropertyDetailViewModel
            {
                Id = property.Id.ToString(),
                Title = property.Title,
                Tag = property.Status,
                IsSaved = property.IsSaved,
                IsAdminPreview = isAdminPreview,
                PriceLabel = PropertyFormatting.FormatPrice(property.Price, property.Contract),
                Address = PropertyFormatting.DisplayOrFallback(property.Address, property.DisplayLocation),
                Media = CreateMedia(property),
                RoomsLabel = property.Rooms + " locali",
                BedroomsLabel = property.BedroomsLabel,
                BathroomsLabel = property.Bathrooms + " bagni",
                SurfaceLabel = property.SurfaceSquareMeters + " mq",
                StatusLabel = PropertyFormatting.DisplayOrFallback(property.RequiredWorksLabel, "Da verificare"),
                OutdoorSpaceLabel = property.OutdoorSpaceLabel,
                EnergyClass = property.EnergyClass,
                AvailabilityLabel = property.AvailabilityLabel,
                FloorLabel = property.FloorLabel,
                GarageLabel = property.GarageLabel,
                HeatingLabel = property.HeatingLabel,
                RequiredWorksLabel = property.RequiredWorksLabel,
                ConstructionYearLabel = property.ConstructionYearLabel,
                CondoFeesLabel = property.CondoFeesLabel,
                BalconyLabel = property.BalconyLabel,
                CellarLabel = property.CellarLabel,
                NearbyServicesLabel = property.NearbyServicesLabel,
                SummaryTitle = property.SummaryTitle,
                SummaryParagraph1 = property.SummaryParagraph1,
                SummaryParagraph2 = property.SummaryParagraph2,
                ReadinessNote = property.ReadinessNote,
                CostsNote = property.CostsNote,
                ContextNote = property.ContextNote,
                DecisionMarginNote = property.DecisionMarginNote,
                HumanFitNote = property.HumanFitNote,
                MapUrl = MapUrl(property, isAdminPreview),
                RequestInfoUrl = "/immobili/" + property.Slug + "/richiedi",
                AdvisorDisplayName = property.AdvisorDisplayName,
                AdvisorRole = PropertyFormatting.DisplayOrFallback(property.AdvisorRole, "Referente per visite e prime informazioni"),
                AgencyPhoneLabel = agencyContactOptions?.PhoneLabel,
                AgencyPhoneHref = PhoneHref(agencyContactOptions),
                SimilarProperties = property.SimilarProperties.Select(CreateSummary).ToList()
            };

            model.TechnicalSections = CreateTechnicalSections(model);
            return model;
        }

        private static string MapUrl(PropertyDetailDto property, bool isAdminPreview)
        {
            if (isAdminPreview && !string.IsNullOrWhiteSpace(property.InternalReference))
            {
                return "/admin/immobili/" + Uri.EscapeDataString(property.InternalReference) + "/anteprima-mappa";
            }

            return "/immobili?view=map&focusId=" + Uri.EscapeDataString(property.Id.ToString());
        }

        public static PropertySummaryCardViewModel CreateSummary(PropertySummaryDto property)
        {
            return new PropertySummaryCardViewModel
            {
                Id = property.Id.ToString(),
                Title = property.Title,
                Slug = property.Slug,
                Tag = property.Status,
                PriceLabel = PropertyFormatting.FormatPrice(property.Price, property.Contract),
                DisplayLocation = PropertyFormatting.DisplayOrFallback(property.DisplayLocation, property.Location),
                ImageUrl = property.ImageUrl
            };
        }

        private static IReadOnlyList<PropertyDetailMediaViewModel> CreateMedia(PropertyDetailDto property)
        {
            var media = (property.Media ?? new List<PropertyMediaDto>())
                .Where(item => !string.IsNullOrWhiteSpace(item.Url))
                .OrderBy(item => item.SortOrder)
                .Select((item, index) => new PropertyDetailMediaViewModel
                {
                    Url = item.Url,
                    AltText = PropertyFormatting.DisplayOrFallback(item.AltText, property.Title),
                    Label = index == 0 ? "Foto principale" : PropertyFormatting.DisplayOrFallback(item.AltText, "Foto immobile")
                })
                .ToList();

            if (!media.Any() && !string.IsNullOrWhiteSpace(property.ImageUrl))
            {
                media.Add(new PropertyDetailMediaViewModel
                {
                    Url = property.ImageUrl,
                    AltText = property.Title,
                    Label = "Foto principale"
                });
            }

            return media;
        }

        private static IReadOnlyList<PropertyDetailTechnicalSectionViewModel> CreateTechnicalSections(
            PropertyDetailViewModel property)
        {
            return new[]
                {
                    Section(
                        "technicalMain",
                        "Caratteristiche principali",
                        true,
                        ("Superficie", property.SurfaceLabel),
                        ("Locali", property.RoomsLabel),
                        ("Bagni", property.BathroomsLabel),
                        ("Camere", property.BedroomsLabel),
                        ("Piano", property.FloorLabel),
                        ("Garage", property.GarageLabel)),
                    Section(
                        "technicalSystems",
                        "Stato, impianti e consumi",
                        false,
                        ("Stato immobile", property.StatusLabel),
                        ("Classe energetica", property.EnergyClass),
                        ("Riscaldamento", property.HeatingLabel),
                        ("Anno costruzione", property.ConstructionYearLabel),
                        ("Spese condominiali", property.CondoFeesLabel)),
                    Section(
                        "technicalOutdoor",
                        "Esterni, pertinenze e servizi",
                        false,
                        ("Giardino", property.OutdoorSpaceLabel),
                        ("Balcone", property.BalconyLabel),
                        ("Cantina", property.CellarLabel),
                        ("Servizi vicini", property.NearbyServicesLabel),
                        ("Disponibilita", property.AvailabilityLabel))
                }
                .Where(section => section.Items.Count > 0)
                .ToList();
        }

        private static PropertyDetailTechnicalSectionViewModel Section(
            string id,
            string title,
            bool isExpanded,
            params (string Label, string Value)[] items)
        {
            return new PropertyDetailTechnicalSectionViewModel
            {
                Id = id,
                Title = title,
                IsExpanded = isExpanded,
                Items = items
                    .Where(item => !string.IsNullOrWhiteSpace(item.Value))
                    .Select(item => new PropertyDetailFactViewModel
                    {
                        Label = item.Label,
                        Value = item.Value
                    })
                    .ToList()
            };
        }

        private static string PhoneHref(AgencyContactOptions agencyContactOptions)
        {
            var configuredHref = agencyContactOptions?.PhoneHref?.Trim();
            if (!string.IsNullOrWhiteSpace(configuredHref))
            {
                return configuredHref;
            }

            var digits = new string((agencyContactOptions?.PhoneLabel ?? string.Empty)
                .Where(char.IsDigit)
                .ToArray());
            return string.IsNullOrWhiteSpace(digits) ? null : "tel:" + digits;
        }
    }
}

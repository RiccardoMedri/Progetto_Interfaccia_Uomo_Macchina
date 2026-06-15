using System;
using System.Collections.Generic;
using System.Linq;
using Medri.Services.Medri.Application;
using Medri.Web.Features.Property;

namespace Medri.Web.Features.Comparison
{
    internal static class ComparisonViewModelMapper
    {
        public static ComparisonIndexViewModel Create(IReadOnlyList<ComparisonPropertyDto> listings)
        {
            return new ComparisonIndexViewModel
            {
                Properties = listings.Select(property => new ComparisonPropertyViewModel
                {
                    Id = property.Id.ToString(),
                    Title = property.Title,
                    ShortTitle = ToShortTitle(property.Title),
                    Slug = property.Slug,
                    DisplayLocation = PropertyFormatting.DisplayOrFallback(property.DisplayLocation, property.Location),
                    PriceLabel = PropertyFormatting.FormatPrice(property.Price, property.Contract),
                    ImageUrl = property.ImageUrl
                }).ToList(),
                Groups = BuildGroups(listings),
                SelectedIdsQueryValue = string.Join(",", listings.Select(property => property.Id.ToString()))
            };
        }

        private static IReadOnlyList<ComparisonGroupViewModel> BuildGroups(
            IReadOnlyList<ComparisonPropertyDto> listings)
        {
            return new List<ComparisonGroupViewModel>
            {
                new ComparisonGroupViewModel
                {
                    Id = "comparePriority",
                    HeadingId = "comparePriorityHeading",
                    Title = "Informazioni prioritarie",
                    IsOpen = true,
                    Rows = new[]
                    {
                        Row("Prezzo", listings, p => PropertyFormatting.FormatPrice(p.Price, p.Contract)),
                        Row("Zona", listings, p => p.ZoneComparisonLabel),
                        Row("Superficie e locali", listings, p => p.SurfaceRoomsComparisonLabel),
                        Row("Stato e lavori", listings, p => p.StatusWorksComparisonLabel),
                        Row("Spazi esterni", listings, p => p.OutdoorSpaceLabel),
                        Row("Classe energetica", listings, p => p.EnergyClass),
                        Row("Compromesso principale", listings, p => p.MainCompromise)
                    }
                },
                new ComparisonGroupViewModel
                {
                    Id = "compareTechnical",
                    HeadingId = "compareTechnicalHeading",
                    Title = "Dati tecnici e caratteristiche",
                    Rows = new[]
                    {
                        Row("Camere", listings, p => p.BedroomsLabel + " camere"),
                        Row("Bagni", listings, p => p.Bathrooms + " bagni"),
                        Row("Piano e accesso", listings, p => p.AccessLabel),
                        Row("Autorimessa", listings, p => p.GarageLabel),
                        Row("Riscaldamento", listings, p => p.HeatingLabel),
                        Row("Anno immobile", listings, p => p.ConstructionYearLabel)
                    }
                },
                new ComparisonGroupViewModel
                {
                    Id = "compareCosts",
                    HeadingId = "compareCostsHeading",
                    Title = "Costi, gestione e rischio",
                    Rows = new[]
                    {
                        Row("Spese condominiali", listings, p => p.ManagementCostsLabel),
                        Row("Lavori stimati", listings, p => p.EstimatedWorksLabel),
                        Row("Costi energetici", listings, p => p.EnergyCostsLabel),
                        Row("Personalizzazione", listings, p => p.PersonalizationLabel),
                        Row("Disponibilita", listings, p => p.AvailabilityLabel)
                    }
                },
                new ComparisonGroupViewModel
                {
                    Id = "compareContext",
                    HeadingId = "compareContextHeading",
                    Title = "Contesto, servizi e vita quotidiana",
                    Rows = new[]
                    {
                        Row("Servizi vicini", listings, p => p.NearbyServicesLabel),
                        Row("Trasporti", listings, p => p.TransportLabel),
                        Row("Riservatezza", listings, p => p.PrivacyLabel),
                        Row("Rumorosita", listings, p => p.NoiseLabel),
                        Row("Target ideale", listings, p => p.IdealTargetLabel)
                    }
                }
            };
        }

        private static ComparisonRowViewModel Row(
            string criterion,
            IEnumerable<ComparisonPropertyDto> listings,
            Func<ComparisonPropertyDto, string> value)
        {
            return new ComparisonRowViewModel
            {
                Criterion = criterion,
                Values = listings.Select(property => new ComparisonValueViewModel
                {
                    PropertyLabel = ToShortTitle(property.Title),
                    Value = value(property)
                }).ToList()
            };
        }

        private static string ToShortTitle(string title)
        {
            const int maxLength = 18;

            if (string.IsNullOrWhiteSpace(title) || title.Length <= maxLength)
            {
                return title;
            }

            var cutIndex = title.LastIndexOf(' ', maxLength);
            if (cutIndex < 8)
            {
                cutIndex = maxLength;
            }

            return title.Substring(0, cutIndex).TrimEnd() + "...";
        }
    }
}

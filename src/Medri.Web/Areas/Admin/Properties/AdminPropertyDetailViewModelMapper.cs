using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Admin;

namespace Medri.Web.Areas.Admin.Properties
{
    internal static class AdminPropertyDetailViewModelMapper
    {
        public static AdminPropertyDetailViewModel CreateEdit(
            AdminPropertyDetailResultDto result,
            AdminPropertyDetailInputModel input,
            ClaimsPrincipal user)
        {
            var values = input ?? CreateInput(result);

            return new AdminPropertyDetailViewModel
            {
                Navigation = CreateNavigation(result, user),
                Reference = result.Reference,
                IsCreateMode = result.IsCreateMode,
                StatusLabel = StatusLabel(result.PublicationStatus),
                StatusClass = StatusClass(result.PublicationStatus),
                CompletionPercent = result.CompletionPercent,
                MissingItems = MissingItems(result.MissingItems),
                AdvisorLabel = AdvisorLabel(result.AdvisorDisplayName),
                AssignedAgencyUserId = values.AssignedAgencyUserId,
                ListingCategory = values.ListingCategory,
                Title = string.IsNullOrWhiteSpace(values.Title) ? result.Title : values.Title,
                Contract = values.Contract,
                PriceText = values.PriceText,
                DisplayLocation = values.DisplayLocation,
                Address = values.Address,
                SurfaceText = values.SurfaceText,
                RoomsText = values.RoomsText,
                BedroomsLabel = values.BedroomsLabel,
                BathroomsText = values.BathroomsText,
                EnergyClass = values.EnergyClass,
                RequiredWorksLabel = values.RequiredWorksLabel,
                CondoFeesLabel = values.CondoFeesLabel,
                FloorLabel = values.FloorLabel,
                ElevatorLabel = values.ElevatorLabel,
                OutdoorSpaceLabel = values.OutdoorSpaceLabel,
                GarageLabel = values.GarageLabel,
                Description = values.Description,
                SummaryTitle = values.SummaryTitle,
                SummaryParagraph2 = values.SummaryParagraph2,
                ReadinessNote = values.ReadinessNote,
                CostsNote = values.CostsNote,
                ContextNote = values.ContextNote,
                DecisionMarginNote = values.DecisionMarginNote,
                AvailabilityLabel = values.AvailabilityLabel,
                HeatingLabel = values.HeatingLabel,
                ConstructionYearLabel = values.ConstructionYearLabel,
                BalconyLabel = values.BalconyLabel,
                CellarLabel = values.CellarLabel,
                NearbyServicesLabel = values.NearbyServicesLabel,
                HumanFitNote = values.HumanFitNote,
                QuickNotes = CreateQuickNotes(values),
                MediaSlots = CreateMediaSlots(result.Media, values.Media, result),
                Checklist = result.Checklist
                    .Select(item => new AdminPropertyChecklistItemViewModel
                    {
                        Label = item.Label,
                        IsDone = item.IsDone
                    })
                    .ToArray(),
                ListingCategoryOptions = CreateListingCategoryOptions(values.ListingCategory),
                ContractOptions = CreateContractOptions(values.Contract),
                AdvisorOptions = CreateAdvisorOptions(result.Advisors, values.AssignedAgencyUserId)
            };
        }

        public static AdminPropertyPreviewViewModel CreatePreview(
            AdminPropertyDetailResultDto result,
            string mode,
            ClaimsPrincipal user)
        {
            var viewMode = mode == "mobile" ? "mobile" : "desktop";

            return new AdminPropertyPreviewViewModel
            {
                Navigation = CreateNavigation(result, user),
                Reference = result.Reference,
                StatusLabel = StatusLabel(result.PublicationStatus),
                CompletionPercent = result.CompletionPercent,
                AdvisorLabel = AdvisorLabel(result.AdvisorDisplayName),
                Title = result.Title,
                PublicPreviewUrl = string.IsNullOrWhiteSpace(result.Slug)
                    ? null
                    : "/immobili/" + Uri.EscapeDataString(result.Slug),
                DisplayLocation = result.DisplayLocation,
                Contract = result.Contract,
                PriceLabel = PriceLabel(result.Price, result.Contract),
                SurfaceLabel = result.SurfaceSquareMeters > 0
                    ? $"{result.SurfaceSquareMeters.ToString(CultureInfo.InvariantCulture)} mq"
                    : "Da completare",
                RoomsLabel = result.Rooms > 0
                    ? result.Rooms.ToString(CultureInfo.InvariantCulture)
                    : "Da completare",
                BathroomsLabel = result.Bathrooms > 0
                    ? result.Bathrooms.ToString(CultureInfo.InvariantCulture)
                    : "Da completare",
                EnergyClass = string.IsNullOrWhiteSpace(result.EnergyClass)
                    ? "Da completare"
                    : result.EnergyClass,
                RequiredWorksLabel = ShortState(result.RequiredWorksLabel),
                Description = result.Description,
                HumanFitNote = result.HumanFitNote,
                ViewMode = viewMode,
                GalleryLabels = result.Media
                    .OrderBy(media => media.SortOrder)
                    .Take(5)
                    .Select(media => media.AltText)
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .ToArray(),
                QuickNotes = new[]
                    {
                        result.ReadinessNote,
                        result.CostsNote,
                        result.ContextNote,
                        result.DecisionMarginNote
                    }
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .ToArray(),
                TechnicalFacts = TechnicalFacts(result)
            };
        }

        private static AdminNavigationViewModel CreateNavigation(
            AdminPropertyDetailResultDto result,
            ClaimsPrincipal user)
        {
            return AdminNavigationViewModelMapper.Create(
                user,
                result.LeadCount,
                result.ActiveRequestCount,
                result.ListingCount,
                AdminSections.Properties);
        }

        private static AdminPropertyDetailInputModel CreateInput(AdminPropertyDetailResultDto result)
        {
            if (result.IsCreateMode)
            {
                return new AdminPropertyDetailInputModel
                {
                    ListingCategory = "Abitazione",
                    Contract = "Vendita",
                    AssignedAgencyUserId = result.AssignedAgencyUserId,
                    Media = new List<AdminPropertyMediaInputModel>()
                };
            }

            return new AdminPropertyDetailInputModel
            {
                ListingCategory = string.IsNullOrWhiteSpace(result.ListingCategory) ? "Abitazione" : result.ListingCategory,
                Title = result.Title,
                Contract = result.Contract,
                PriceText = PriceLabel(result.Price, result.Contract),
                DisplayLocation = result.DisplayLocation,
                Address = result.Address,
                SurfaceText = result.SurfaceSquareMeters > 0
                    ? $"{result.SurfaceSquareMeters.ToString(CultureInfo.InvariantCulture)} mq"
                    : string.Empty,
                RoomsText = result.Rooms > 0
                    ? result.Rooms.ToString(CultureInfo.InvariantCulture)
                    : string.Empty,
                BedroomsLabel = result.BedroomsLabel,
                BathroomsText = result.Bathrooms > 0
                    ? result.Bathrooms.ToString(CultureInfo.InvariantCulture)
                    : string.Empty,
                EnergyClass = result.EnergyClass,
                RequiredWorksLabel = result.RequiredWorksLabel,
                CondoFeesLabel = result.CondoFeesLabel,
                FloorLabel = result.FloorLabel,
                ElevatorLabel = result.ElevatorLabel,
                OutdoorSpaceLabel = result.OutdoorSpaceLabel,
                GarageLabel = result.GarageLabel,
                Description = result.Description,
                SummaryTitle = result.SummaryTitle,
                SummaryParagraph2 = result.SummaryParagraph2,
                ReadinessNote = result.ReadinessNote,
                CostsNote = result.CostsNote,
                ContextNote = result.ContextNote,
                DecisionMarginNote = result.DecisionMarginNote,
                AvailabilityLabel = result.AvailabilityLabel,
                HeatingLabel = result.HeatingLabel,
                ConstructionYearLabel = result.ConstructionYearLabel,
                BalconyLabel = result.BalconyLabel,
                CellarLabel = result.CellarLabel,
                NearbyServicesLabel = result.NearbyServicesLabel,
                HumanFitNote = result.HumanFitNote,
                AssignedAgencyUserId = result.AssignedAgencyUserId,
                Media = result.Media
                    .OrderBy(media => media.SortOrder)
                    .Select(media => new AdminPropertyMediaInputModel
                    {
                        Id = media.Id,
                        AltText = media.AltText,
                        SortOrder = media.SortOrder
                    })
                    .ToList()
            };
        }

        private static IReadOnlyList<AdminPropertyQuickNoteViewModel> CreateQuickNotes(
            AdminPropertyDetailInputModel values)
        {
            return new[]
            {
                CreateQuickNote(1, nameof(values.ReadinessNote), values.ReadinessNote, "Pronto da abitare / lavori"),
                CreateQuickNote(2, nameof(values.CostsNote), values.CostsNote, "Costi e gestione"),
                CreateQuickNote(3, nameof(values.ContextNote), values.ContextNote, "Contesto"),
                CreateQuickNote(4, nameof(values.DecisionMarginNote), values.DecisionMarginNote, "Margine decisionale")
            };
        }

        private static AdminPropertyQuickNoteViewModel CreateQuickNote(
            int number,
            string fieldName,
            string value,
            string placeholder)
        {
            return new AdminPropertyQuickNoteViewModel
            {
                Number = number,
                FieldName = fieldName,
                Value = value,
                Placeholder = placeholder
            };
        }

        private static IReadOnlyList<AdminPropertyMediaSlotViewModel> CreateMediaSlots(
            IReadOnlyList<AdminPropertyMediaDto> media,
            IReadOnlyList<AdminPropertyMediaInputModel> input,
            AdminPropertyDetailResultDto result)
        {
            var inputById = (input ?? Array.Empty<AdminPropertyMediaInputModel>())
                .Where(item => item.Id != Guid.Empty)
                .GroupBy(item => item.Id)
                .Select(group => group.First())
                .ToDictionary(item => item.Id);

            var orderedMedia = media
                .Where(item => !IsRemoved(item, inputById))
                .OrderBy(item => SortOrder(item, inputById))
                .ThenBy(item => item.SortOrder)
                .ToArray();

            if (orderedMedia.Length == 0 && ShouldShowListingImageFallback(result))
            {
                return new[]
                {
                    new AdminPropertyMediaSlotViewModel
                    {
                        SlotNumber = 1,
                        MediaId = Guid.Empty,
                        AltText = result.Title,
                        Url = result.ImageUrl,
                        IsCover = true,
                        SortOrder = 1
                    }
                };
            }

            return orderedMedia
                .Select((mediaItem, index) =>
                {
                    inputById.TryGetValue(mediaItem.Id, out var inputItem);
                    return new AdminPropertyMediaSlotViewModel
                    {
                        SlotNumber = index + 1,
                        MediaId = mediaItem.Id,
                        AltText = inputItem != null
                            ? inputItem.AltText
                            : mediaItem.AltText,
                        Url = mediaItem.Url,
                        IsCover = index == 0,
                        SortOrder = inputItem != null && inputItem.SortOrder > 0
                            ? inputItem.SortOrder
                            : mediaItem.SortOrder
                    };
                })
                .ToArray();
        }

        private static bool ShouldShowListingImageFallback(AdminPropertyDetailResultDto result)
        {
            return result != null &&
                !result.IsCreateMode &&
                !string.IsNullOrWhiteSpace(result.ImageUrl) &&
                (result.PublicationStatus != "Incomplete" ||
                 result.ImageUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsRemoved(
            AdminPropertyMediaDto media,
            IReadOnlyDictionary<Guid, AdminPropertyMediaInputModel> inputById)
        {
            return inputById.TryGetValue(media.Id, out var input) && input.Remove;
        }

        private static int SortOrder(
            AdminPropertyMediaDto media,
            IReadOnlyDictionary<Guid, AdminPropertyMediaInputModel> inputById)
        {
            return inputById.TryGetValue(media.Id, out var input) && input.SortOrder > 0
                ? input.SortOrder
                : media.SortOrder;
        }

        private static IReadOnlyList<AdminPropertyOptionViewModel> CreateAdvisorOptions(
            IReadOnlyList<AdminPropertyAdvisorDto> advisors,
            Guid? selected)
        {
            var selectedValue = selected?.ToString();
            var orderedAdvisors = advisors
                .OrderBy(advisor => advisor.DisplayName);

            return new[] { CreateOption(null, "Non assegnato", selectedValue) }
                .Concat(orderedAdvisors.Select(advisor =>
                    CreateOption(advisor.Id.ToString(), FirstName(advisor.DisplayName), selectedValue)))
                .ToArray();
        }

        private static IReadOnlyList<AdminPropertyOptionViewModel> CreateListingCategoryOptions(
            string selected)
        {
            return DistinctOptions(new[]
            {
                CreateOption("Abitazione", "Abitazione", selected),
                CreateOption("Terreno agricolo", "Terreno agricolo", selected),
                CreateOption("Locale commerciale", "Locale commerciale", selected),
                CreateOption("Garage / posto auto", "Garage / posto auto", selected),
                CreateOption(selected, selected, selected)
            });
        }

        private static IReadOnlyList<AdminPropertyOptionViewModel> CreateContractOptions(string selected)
        {
            return new[]
            {
                CreateOption("Vendita", "Vendita", selected),
                CreateOption("Affitto", "Affitto", selected)
            };
        }

        private static AdminPropertyOptionViewModel CreateOption(
            string value,
            string label,
            string selected)
        {
            return new AdminPropertyOptionViewModel
            {
                Value = value,
                Label = string.IsNullOrWhiteSpace(label) ? value : label,
                IsSelected = value == selected
            };
        }

        private static IReadOnlyList<AdminPropertyOptionViewModel> DistinctOptions(
            IEnumerable<AdminPropertyOptionViewModel> options)
        {
            return options
                .Where(option => !string.IsNullOrWhiteSpace(option.Value))
                .GroupBy(option => option.Value)
                .Select(group => group.First())
                .ToArray();
        }

        private static IReadOnlyList<string> TechnicalFacts(AdminPropertyDetailResultDto result)
        {
            return new[]
            {
                Fact("Camere", result.BedroomsLabel),
                Fact("Piano", result.FloorLabel),
                Fact("Ascensore", result.ElevatorLabel),
                Fact("Esterni", result.OutdoorSpaceLabel),
                Fact("Garage / posto auto", result.GarageLabel),
                Fact("Disponibilita", result.AvailabilityLabel),
                Fact("Riscaldamento", result.HeatingLabel),
                Fact("Anno costruzione", result.ConstructionYearLabel),
                Fact("Balcone", result.BalconyLabel),
                Fact("Cantina", result.CellarLabel),
                Fact("Servizi vicini", result.NearbyServicesLabel),
                Fact("Spese condominiali", result.CondoFeesLabel)
            }
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToArray();
        }

        private static string Fact(string label, string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : $"{label}: {value}";
        }

        private static string PriceLabel(decimal price, string contract)
        {
            if (price <= 0m)
            {
                return "Prezzo da completare";
            }

            var suffix = contract == "Affitto" ? " / mese" : string.Empty;
            return $"EUR {price.ToString("N0", CultureInfo.GetCultureInfo("it-IT"))}{suffix}";
        }

        private static string ShortState(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Da completare";
            }

            var commaIndex = value.IndexOf(',', StringComparison.Ordinal);
            return commaIndex < 0 ? value : value.Substring(0, commaIndex);
        }

        private static string MissingItems(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "Nessun dato mancante" : value;
        }

        private static string StatusLabel(string status)
        {
            return status switch
            {
                "Incomplete" => "Incompleto",
                "Ready" => "Pronto",
                "Published" => "Pubblicato",
                "NeedsUpdate" => "Da aggiornare",
                _ => "Riservato"
            };
        }

        private static string StatusClass(string status)
        {
            return status switch
            {
                "Incomplete" => "is-incomplete",
                "Ready" => "is-success",
                "Published" => "is-active",
                "NeedsUpdate" => "is-update-needed",
                _ => "is-low"
            };
        }

        private static string AdvisorLabel(string displayName)
        {
            return string.IsNullOrWhiteSpace(displayName)
                ? "Non assegnato"
                : FirstName(displayName);
        }

        private static string FirstName(string displayName)
        {
            return displayName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault() ?? displayName;
        }
    }
}

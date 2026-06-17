using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Medri.Services.Medri.Application;
using Medri.Web.Areas.Admin;
using Medri.Web.Features.Search;

namespace Medri.Web.Areas.Admin.Properties
{
    public sealed class AdminPropertyDetailInputModel : IValidatableObject
    {
        [Required, StringLength(80)]
        public string ListingCategory { get; set; }

        [Required, StringLength(160)]
        public string Title { get; set; }

        [Required, StringLength(30)]
        public string Contract { get; set; }

        [Required, StringLength(80)]
        public string PriceText { get; set; }

        [Required, StringLength(160)]
        public string DisplayLocation { get; set; }

        [Required, StringLength(300)]
        public string Address { get; set; }

        [StringLength(40)]
        public string LatitudeText { get; set; }

        [StringLength(40)]
        public string LongitudeText { get; set; }

        [Required, StringLength(40)]
        public string SurfaceText { get; set; }

        [Required, StringLength(40)]
        public string RoomsText { get; set; }

        [StringLength(80)]
        public string BedroomsLabel { get; set; }

        [Required, StringLength(40)]
        public string BathroomsText { get; set; }

        [StringLength(80)]
        public string EnergyClass { get; set; }

        [StringLength(160)]
        public string RequiredWorksLabel { get; set; }

        [StringLength(160)]
        public string CondoFeesLabel { get; set; }

        [StringLength(160)]
        public string FloorLabel { get; set; }

        [StringLength(160)]
        public string ElevatorLabel { get; set; }

        [StringLength(160)]
        public string OutdoorSpaceLabel { get; set; }

        [StringLength(160)]
        public string GarageLabel { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }

        [StringLength(160)]
        public string SummaryTitle { get; set; }

        [StringLength(2000)]
        public string SummaryParagraph2 { get; set; }

        [StringLength(500)]
        public string ReadinessNote { get; set; }

        [StringLength(500)]
        public string CostsNote { get; set; }

        [StringLength(500)]
        public string ContextNote { get; set; }

        [StringLength(500)]
        public string DecisionMarginNote { get; set; }

        [StringLength(160)]
        public string AvailabilityLabel { get; set; }

        [StringLength(160)]
        public string HeatingLabel { get; set; }

        [StringLength(160)]
        public string ConstructionYearLabel { get; set; }

        [StringLength(160)]
        public string BalconyLabel { get; set; }

        [StringLength(160)]
        public string CellarLabel { get; set; }

        [StringLength(300)]
        public string NearbyServicesLabel { get; set; }

        [StringLength(2000)]
        public string HumanFitNote { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        public IFormFile[] UploadedMedia { get; set; } =
            Array.Empty<IFormFile>();

        public List<AdminPropertyMediaInputModel> Media { get; set; } =
            new List<AdminPropertyMediaInputModel>();

        public AdminPropertyDetailUpdateDto ToUpdateDto()
        {
            return new AdminPropertyDetailUpdateDto
            {
                ListingCategory = ListingCategory,
                Title = Title,
                Contract = Contract,
                Price = AdminPropertyInputParser.ParseDecimal(PriceText),
                DisplayLocation = DisplayLocation,
                Address = Address,
                Latitude = AdminPropertyInputParser.ParseLatitude(LatitudeText),
                Longitude = AdminPropertyInputParser.ParseLongitude(LongitudeText),
                SurfaceSquareMeters = AdminPropertyInputParser.ParseInt(SurfaceText),
                Rooms = AdminPropertyInputParser.ParseInt(RoomsText),
                BedroomsLabel = BedroomsLabel,
                Bathrooms = AdminPropertyInputParser.ParseInt(BathroomsText),
                EnergyClass = EnergyClass,
                RequiredWorksLabel = RequiredWorksLabel,
                CondoFeesLabel = CondoFeesLabel,
                FloorLabel = FloorLabel,
                ElevatorLabel = ElevatorLabel,
                OutdoorSpaceLabel = OutdoorSpaceLabel,
                GarageLabel = GarageLabel,
                Description = Description,
                SummaryTitle = SummaryTitle,
                SummaryParagraph2 = SummaryParagraph2,
                ReadinessNote = ReadinessNote,
                CostsNote = CostsNote,
                ContextNote = ContextNote,
                DecisionMarginNote = DecisionMarginNote,
                AvailabilityLabel = AvailabilityLabel,
                HeatingLabel = HeatingLabel,
                ConstructionYearLabel = ConstructionYearLabel,
                BalconyLabel = BalconyLabel,
                CellarLabel = CellarLabel,
                NearbyServicesLabel = NearbyServicesLabel,
                HumanFitNote = HumanFitNote,
                AssignedAgencyUserId = AssignedAgencyUserId,
                Media = (Media ?? new List<AdminPropertyMediaInputModel>())
                    .Where(media => media != null)
                    .Select(media => new AdminPropertyMediaUpdateDto
                    {
                        Id = media.Id,
                        AltText = media.AltText,
                        SortOrder = media.SortOrder,
                        Remove = media.Remove
                    })
                    .ToArray()
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var hasLatitude = AdminPropertyInputParser.TryParseLatitude(LatitudeText, out var latitude);
            var hasLongitude = AdminPropertyInputParser.TryParseLongitude(LongitudeText, out var longitude);

            if (!string.IsNullOrWhiteSpace(Address) &&
                (!hasLatitude ||
                 !hasLongitude ||
                 !AdminPropertyInputParser.HasUsableCoordinates(latitude, longitude)))
            {
                yield return new ValidationResult(
                    "Verifica l'indirizzo: la posizione mappa non e stata trovata.",
                    new[] { nameof(Address) });
            }
        }
    }

    public sealed class AdminPropertyMediaInputModel
    {
        public Guid Id { get; set; }

        [StringLength(200)]
        public string AltText { get; set; }

        public int SortOrder { get; set; }

        public bool Remove { get; set; }
    }

    public sealed class AdminPropertyDetailViewModel : IAdminPageViewModel
    {
        public AdminNavigationViewModel Navigation { get; set; }

        public string Reference { get; set; }

        public bool IsCreateMode { get; set; }

        public bool CanDiscardDraft { get; set; }

        public string StatusLabel { get; set; }

        public string StatusClass { get; set; }

        public int CompletionPercent { get; set; }

        public string MissingItems { get; set; }

        public string AdvisorLabel { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        public IFormFile[] UploadedMedia { get; set; } =
            Array.Empty<IFormFile>();

        public string ListingCategory { get; set; }

        public string Title { get; set; }

        public string Contract { get; set; }

        public string PriceText { get; set; }

        public string DisplayLocation { get; set; }

        public string Address { get; set; }

        public string LatitudeText { get; set; }

        public string LongitudeText { get; set; }

        public string SurfaceText { get; set; }

        public string RoomsText { get; set; }

        public string BedroomsLabel { get; set; }

        public string BathroomsText { get; set; }

        public string EnergyClass { get; set; }

        public string RequiredWorksLabel { get; set; }

        public string CondoFeesLabel { get; set; }

        public string FloorLabel { get; set; }

        public string ElevatorLabel { get; set; }

        public string OutdoorSpaceLabel { get; set; }

        public string GarageLabel { get; set; }

        public string Description { get; set; }

        public string SummaryTitle { get; set; }

        public string SummaryParagraph2 { get; set; }

        public string ReadinessNote { get; set; }

        public string CostsNote { get; set; }

        public string ContextNote { get; set; }

        public string DecisionMarginNote { get; set; }

        public string AvailabilityLabel { get; set; }

        public string HeatingLabel { get; set; }

        public string ConstructionYearLabel { get; set; }

        public string BalconyLabel { get; set; }

        public string CellarLabel { get; set; }

        public string NearbyServicesLabel { get; set; }

        public string HumanFitNote { get; set; }

        public IReadOnlyList<AdminPropertyQuickNoteViewModel> QuickNotes { get; set; } =
            Array.Empty<AdminPropertyQuickNoteViewModel>();

        public IReadOnlyList<AdminPropertyMediaSlotViewModel> MediaSlots { get; set; } =
            Array.Empty<AdminPropertyMediaSlotViewModel>();

        public IReadOnlyList<AdminPropertyChecklistItemViewModel> Checklist { get; set; } =
            Array.Empty<AdminPropertyChecklistItemViewModel>();

        public IReadOnlyList<AdminPropertyOptionViewModel> ListingCategoryOptions { get; set; } =
            Array.Empty<AdminPropertyOptionViewModel>();

        public IReadOnlyList<AdminPropertyOptionViewModel> ContractOptions { get; set; } =
            Array.Empty<AdminPropertyOptionViewModel>();

        public IReadOnlyList<AdminPropertyOptionViewModel> AdvisorOptions { get; set; } =
            Array.Empty<AdminPropertyOptionViewModel>();
    }

    public sealed class AdminPropertyPreviewViewModel : IAdminPageViewModel
    {
        public AdminNavigationViewModel Navigation { get; set; }

        public string Reference { get; set; }

        public string StatusLabel { get; set; }

        public int CompletionPercent { get; set; }

        public string AdvisorLabel { get; set; }

        public string Title { get; set; }

        public string PublicPreviewUrl { get; set; }

        public string DisplayLocation { get; set; }

        public string Contract { get; set; }

        public string PriceLabel { get; set; }

        public string SurfaceLabel { get; set; }

        public string RoomsLabel { get; set; }

        public string BathroomsLabel { get; set; }

        public string EnergyClass { get; set; }

        public string RequiredWorksLabel { get; set; }

        public string Description { get; set; }

        public string HumanFitNote { get; set; }

        public string ViewMode { get; set; }

        public bool IsMobilePreview => ViewMode == "mobile";

        public IReadOnlyList<string> GalleryLabels { get; set; } =
            Array.Empty<string>();

        public IReadOnlyList<string> QuickNotes { get; set; } =
            Array.Empty<string>();

        public IReadOnlyList<string> TechnicalFacts { get; set; } =
            Array.Empty<string>();
    }

    public sealed class AdminPropertyPreviewMapViewModel : IAdminPageViewModel
    {
        public AdminNavigationViewModel Navigation { get; set; }

        public string Reference { get; set; }

        public string Title { get; set; }

        public string EditUrl => "/admin/immobili/" + Uri.EscapeDataString(Reference);

        public string PreviewUrl => "/admin/immobili/" + Uri.EscapeDataString(Reference) + "/anteprima";

        public IReadOnlyList<SearchMapMarkerViewModel> Markers { get; set; } =
            Array.Empty<SearchMapMarkerViewModel>();

        public bool HasMarker => Markers.Count > 0;

        public string SelectedId => Markers.FirstOrDefault()?.Id;

        public string MapConfigJson(string apiKey, string mapId)
        {
            return JsonSerializer.Serialize(
                new
                {
                    apiKey,
                    mapId,
                    isConfigured = HasMarker &&
                        !string.IsNullOrWhiteSpace(apiKey) &&
                        !string.IsNullOrWhiteSpace(mapId),
                    selectedId = SelectedId,
                    markers = Markers
                },
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }

    public sealed class AdminPropertyQuickNoteViewModel
    {
        public int Number { get; set; }

        public string FieldName { get; set; }

        public string Value { get; set; }

        public string Placeholder { get; set; }
    }

    public sealed class AdminPropertyMediaSlotViewModel
    {
        public int SlotNumber { get; set; }

        public Guid? MediaId { get; set; }

        public string AltText { get; set; }

        public string Url { get; set; }

        public bool IsCover { get; set; }

        public int SortOrder { get; set; }

        public bool IsEmpty => !MediaId.HasValue;
    }

    public sealed class AdminPropertyChecklistItemViewModel
    {
        public string Label { get; set; }

        public bool IsDone { get; set; }
    }

    internal static class AdminPropertyInputParser
    {
        public static int ParseInt(string value)
        {
            var digits = Digits(value);
            return int.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
                ? result
                : 0;
        }

        public static decimal ParseDecimal(string value)
        {
            var number = NumberText(value);
            if (string.IsNullOrWhiteSpace(number))
            {
                return 0m;
            }

            if (decimal.TryParse(number, NumberStyles.Number, CultureInfo.GetCultureInfo("it-IT"), out var result) ||
                decimal.TryParse(number, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            return 0m;
        }

        public static double? ParseLatitude(string value)
        {
            return TryParseLatitude(value, out var result) ? result : null;
        }

        public static double? ParseLongitude(string value)
        {
            return TryParseLongitude(value, out var result) ? result : null;
        }

        public static bool TryParseLatitude(string value, out double result)
        {
            return TryParseCoordinate(value, -90d, 90d, out result);
        }

        public static bool TryParseLongitude(string value, out double result)
        {
            return TryParseCoordinate(value, -180d, 180d, out result);
        }

        public static bool HasUsableCoordinates(double latitude, double longitude)
        {
            return latitude >= -90d &&
                latitude <= 90d &&
                longitude >= -180d &&
                longitude <= 180d &&
                (latitude != 0d || longitude != 0d);
        }

        private static string NumberText(string value)
        {
            var parts = string.IsNullOrWhiteSpace(value)
                ? null
                : new string(value
                    .Where(character => char.IsDigit(character) ||
                                        character == ',' ||
                                        character == '.')
                    .ToArray());

            return string.IsNullOrWhiteSpace(parts)
                ? null
                : parts;
        }

        private static bool TryParseCoordinate(
            string value,
            double minimum,
            double maximum,
            out double result)
        {
            result = 0d;
            var normalized = string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim().Replace(',', '.');

            return !string.IsNullOrWhiteSpace(normalized) &&
                double.TryParse(
                    normalized,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out result) &&
                result >= minimum &&
                result <= maximum;
        }

        private static string Digits(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : new string(value.Where(char.IsDigit).ToArray());
        }
    }
}

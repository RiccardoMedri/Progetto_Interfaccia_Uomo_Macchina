using System;
using System.Collections.Generic;
using System.Linq;

namespace Medri.Services.Medri.Application
{
    internal static class AdminPropertyCompletionCalculator
    {
        public static AdminPropertyCompletionResult Calculate(
            PropertyListing listing,
            int mediaCount,
            bool hasFloorPlan)
        {
            if (listing == null)
            {
                return new AdminPropertyCompletionResult();
            }

            var items = new[]
            {
                Item("Prezzo", listing.Price > 0),
                Item("Zona e indirizzo", HasText(listing.DisplayLocation) && HasText(listing.Address)),
                Item("Superficie, locali e bagni", listing.SurfaceSquareMeters > 0 && listing.Rooms > 0 && listing.Bathrooms > 0),
                Item("Classe energetica", HasText(listing.EnergyClass)),
                Item("Descrizione", HasText(listing.SummaryParagraph1)),
                Item("6 foto", mediaCount >= 6),
                Item("Planimetria", hasFloorPlan),
                Item("Stato e lavori", HasText(listing.RequiredWorksLabel)),
                Item("CTA contatto", true)
            };

            var doneCount = items.Count(item => item.IsDone);
            var completionPercent = (int)Math.Round(doneCount * 100m / items.Length, MidpointRounding.AwayFromZero);
            var missingItems = items
                .Where(item => !item.IsDone)
                .Select(item => item.Label)
                .ToArray();

            return new AdminPropertyCompletionResult
            {
                CompletionPercent = completionPercent,
                MissingItems = missingItems.Length == 0
                    ? "Nessun dato mancante"
                    : string.Join(", ", missingItems),
                Checklist = items
            };
        }

        public static bool HasFloorPlan(IEnumerable<PropertyMedia> media)
        {
            return (media ?? Array.Empty<PropertyMedia>())
                .Any(item =>
                    Contains(item.AltText, "planimetr") ||
                    Contains(item.Url, "planimetr"));
        }

        public static void ApplyToListing(
            PropertyListing listing,
            AdminPropertyCompletionResult completion)
        {
            listing.CompletionPercent = completion.CompletionPercent;
            listing.MissingItems = completion.MissingItems;
        }

        private static AdminPropertyCompletionChecklistItem Item(
            string label,
            bool isDone)
        {
            return new AdminPropertyCompletionChecklistItem
            {
                Label = label,
                IsDone = isDone
            };
        }

        private static bool HasText(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        private static bool Contains(string value, string searchTerm)
        {
            return value?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }

    internal sealed class AdminPropertyCompletionResult
    {
        public int CompletionPercent { get; set; }

        public string MissingItems { get; set; } = "Dati minimi, testi e media";

        public bool IsComplete => CompletionPercent >= 100;

        public IReadOnlyList<AdminPropertyCompletionChecklistItem> Checklist { get; set; } =
            Array.Empty<AdminPropertyCompletionChecklistItem>();
    }

    internal sealed class AdminPropertyCompletionChecklistItem
    {
        public string Label { get; set; }

        public bool IsDone { get; set; }
    }
}

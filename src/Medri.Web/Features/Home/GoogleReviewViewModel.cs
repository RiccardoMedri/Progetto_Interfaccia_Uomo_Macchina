using System;

namespace Medri.Web.Features.Home
{
    public class GoogleReviewViewModel
    {
        public int Rating { get; set; }

        public string RatingLabel { get; set; }

        public string Text { get; set; }

        public static GoogleReviewViewModel Unavailable()
        {
            return new GoogleReviewViewModel
            {
                RatingLabel = "-",
                Rating = 0,
                Text = "Recensioni Google momentaneamente non disponibili."
            };
        }

        public static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Recensione senza testo disponibile.";
            }

            var normalized = value.Trim();
            if (normalized.Length <= maxLength)
            {
                return normalized;
            }

            return normalized.Substring(0, Math.Max(0, maxLength - 1)).TrimEnd() + "...";
        }
    }
}

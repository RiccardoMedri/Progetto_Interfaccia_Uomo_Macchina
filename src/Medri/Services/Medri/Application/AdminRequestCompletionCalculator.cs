using System;
using System.Linq;

namespace Medri.Services.Medri.Application
{
    internal static class AdminRequestCompletionCalculator
    {
        public static int Calculate(
            string phone,
            string email,
            Guid? assignedAgencyUserId,
            string maximumBudgetLabel,
            string preferredBudgetLabel,
            string desiredLocation,
            string acceptableLocations,
            int? minimumRooms,
            string propertyType,
            string valuationGoal,
            string propertyToSellStatus,
            string desiredPreferenceTags,
            string negotiablePreferenceTags)
        {
            var checks = new[]
            {
                HasText(phone) || HasText(email),
                assignedAgencyUserId.HasValue,
                HasText(maximumBudgetLabel) || HasText(preferredBudgetLabel),
                HasText(desiredLocation) || HasText(acceptableLocations),
                minimumRooms.HasValue ||
                    HasText(propertyType) ||
                    HasText(valuationGoal) ||
                    HasText(propertyToSellStatus) ||
                    HasText(desiredPreferenceTags) ||
                    HasText(negotiablePreferenceTags)
            };

            var doneCount = checks.Count(isDone => isDone);
            return (int)Math.Round(doneCount * 100m / checks.Length, MidpointRounding.AwayFromZero);
        }

        private static bool HasText(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}

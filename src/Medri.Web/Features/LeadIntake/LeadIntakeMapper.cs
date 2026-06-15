using System.Linq;
using Medri.Services.Medri.Application;
using Medri.Web.Features.Property;

namespace Medri.Web.Features.LeadIntake
{
    internal static class LeadIntakeMapper
    {
        public static LeadRequestDto Create(BuyLeadRequestViewModel input)
        {
            return CreateBase(input, "Buy", input.DesiredLocations, input.SustainableBudget, input.PropertyType) with
            {
                AcceptableLocations = input.AcceptableLocations,
                SearchStage = input.SearchStage,
                FinancingStatus = input.FinancingStatus,
                PropertyToSellStatus = input.PropertyToSellStatus,
                Timing = input.Timing,
                PreferencesAndCompromises = input.PreferencesAndCompromises
            };
        }

        public static LeadRequestDto Create(RentLeadRequestViewModel input)
        {
            return CreateBase(input, "Rent", input.DesiredLocations, input.SustainableBudget, null) with
            {
                HouseholdDescription = input.HouseholdDescription,
                WorkStudySituation = input.WorkStudySituation,
                AvailableGuarantees = input.AvailableGuarantees,
                DesiredMoveIn = input.DesiredMoveIn,
                PreferencesAndCompromises = input.PreferencesAndCompromises
            };
        }

        public static LeadRequestDto Create(SellLeadRequestViewModel input)
        {
            return CreateBase(input, "Sell", input.DesiredLocation, null, input.PropertyType) with
            {
                PropertyCondition = input.PropertyCondition,
                Availability = input.Availability,
                Timing = input.Timing,
                ExpectedPriceOrMainQuestion = input.ExpectedPriceOrMainQuestion,
                PreferencesAndCompromises = input.PreferencesAndCompromises
            };
        }

        public static LeadRequestDto Create(RentOutLeadRequestViewModel input)
        {
            return CreateBase(input, "RentOut", input.DesiredLocation, null, input.PropertyType) with
            {
                PropertyCondition = input.PropertyCondition,
                Availability = input.Availability,
                ExpectedPriceOrMainQuestion = input.ExpectedPriceOrMainQuestion,
                DesiredContractType = input.DesiredContractType,
                PreferencesAndCompromises = input.PreferencesAndCompromises
            };
        }

        public static LeadRequestDto Create(ValuationRequestViewModel input)
        {
            return CreateBase(input, "Valuation", input.DesiredLocation, null, input.PropertyType) with
            {
                IndicativeSurface = input.IndicativeSurface,
                PropertyCondition = input.PropertyCondition,
                Appurtenances = input.Appurtenances,
                ValuationGoal = input.ValuationGoal,
                PreferencesAndCompromises = input.PreferencesAndCompromises
            };
        }

        public static LeadConfirmationViewModel Create(LeadConfirmationDto result)
        {
            return new LeadConfirmationViewModel
            {
                Id = result.Id,
                PathLabel = result.RequestType == "Rent"
                    ? "Affitta casa"
                    : result.RequestType == "Sell"
                        ? "Vendere casa"
                        : result.RequestType == "RentOut"
                            ? "Mettere in affitto"
                            : result.RequestType == "Valuation"
                                ? "Valutazione immobile"
                                : "Cerchi casa",
                PreferredContactMode = result.PreferredContactMode,
                WhenLabel = ToWhenLabel(
                    result.PreferredTimeSlot,
                    result.PreferredDay,
                    result.PreferredTime),
                FeaturedProperties = result.FeaturedProperties
                    .Select(PropertyViewModelMapper.CreateSummary)
                    .ToList()
            };
        }

        private static LeadRequestDto CreateBase(
            LeadRequestInputModel input,
            string requestType,
            string desiredLocation,
            string sustainableBudget,
            string propertyType)
        {
            return new LeadRequestDto
            {
                RequestType = requestType,
                FullName = input.FullName,
                Phone = input.Phone,
                Email = input.Email,
                PreferredContactMode = input.PreferredContactMode,
                PreferredTimeSlot = input.PreferredTimeSlot,
                PreferredDay = input.PreferredDay,
                PreferredTime = input.PreferredTime,
                DesiredLocation = desiredLocation,
                SustainableBudget = sustainableBudget,
                PropertyType = propertyType
            };
        }

        public static string ToWhenLabel(string timeSlot, string day, string time)
        {
            if (timeSlot != "Altro orario")
            {
                return timeSlot;
            }

            if (string.IsNullOrWhiteSpace(day) && string.IsNullOrWhiteSpace(time))
            {
                return "Altro orario";
            }

            if (string.IsNullOrWhiteSpace(day))
            {
                return "Ore " + time;
            }

            return string.IsNullOrWhiteSpace(time)
                ? day
                : day + ", ore " + time;
        }
    }
}

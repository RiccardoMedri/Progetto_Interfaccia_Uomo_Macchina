using System;
using System.ComponentModel.DataAnnotations;

namespace Medri.Services.Medri
{
    public class LeadPreference
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid LeadId { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal? MinimumPrice { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal? MaximumPrice { get; set; }

        [StringLength(500)]
        public string DesiredLocation { get; set; }

        [Range(0, 100)]
        public int? MinimumRooms { get; set; }

        public string SustainableBudgetLabel { get; set; }

        public string AcceptableLocations { get; set; }

        public string PropertyType { get; set; }

        public string SearchStage { get; set; }

        public string FinancingStatus { get; set; }

        public string PropertyToSellStatus { get; set; }

        public string Timing { get; set; }

        [StringLength(2000)]
        public string PreferencesAndCompromises { get; set; }

        public string HouseholdDescription { get; set; }

        public string WorkStudySituation { get; set; }

        public string AvailableGuarantees { get; set; }

        public string DesiredMoveIn { get; set; }

        public string PropertyCondition { get; set; }

        public string Availability { get; set; }

        public string ExpectedPriceOrMainQuestion { get; set; }

        public string LinkedPropertyReference { get; set; }

        public string DesiredContractType { get; set; }

        public string IndicativeSurface { get; set; }

        public string Appurtenances { get; set; }

        public string ValuationGoal { get; set; }
    }
}

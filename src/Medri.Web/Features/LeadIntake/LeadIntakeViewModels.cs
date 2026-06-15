using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Medri.Web.Features.Property;

namespace Medri.Web.Features.LeadIntake
{
    public abstract class ContactPreferenceInputModel
    {
        [Required(ErrorMessage = "Scegli come preferisci essere ricontattato.")]
        public string PreferredContactMode { get; set; } = "Essere ricontattato";

        [Required(ErrorMessage = "Scegli una fascia oraria.")]
        public string PreferredTimeSlot { get; set; } = "Mattina";

        public string PreferredDay { get; set; } = string.Empty;

        public string PreferredTime { get; set; } = string.Empty;
    }

    public class LeadFunnelContactOptionViewModel
    {
        public LeadFunnelContactOptionViewModel()
        {
        }

        public LeadFunnelContactOptionViewModel(string value, string title, string description, string iconClass)
        {
            Value = value;
            Title = title;
            Description = description;
            IconClass = iconClass;
        }

        public string Value { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string IconClass { get; set; }
    }

    public class LeadFunnelContactPreferencesViewModel
    {
        public string SectionClass { get; set; } =
            "visit-section medri-lead-funnel__section medri-lead-funnel__contact-section";

        public string Heading { get; set; }

        public string SelectedValue { get; set; }

        public string FieldName { get; set; } = nameof(ContactPreferenceInputModel.PreferredContactMode);

        public string IdPrefix { get; set; } = "lead-contact-option";

        public IReadOnlyList<LeadFunnelContactOptionViewModel> Options { get; set; } =
            new List<LeadFunnelContactOptionViewModel>();
    }

    public class LeadFunnelTimePreferencesViewModel
    {
        public string SectionClass { get; set; } =
            "visit-section medri-lead-funnel__section medri-lead-funnel__time-section";

        public string Heading { get; set; } = "Quando preferisci essere contattato?";

        public string SelectedTimeSlot { get; set; }

        public string PreferredDay { get; set; }

        public string PreferredTime { get; set; }

        public string SlotFieldName { get; set; } = nameof(ContactPreferenceInputModel.PreferredTimeSlot);

        public string DayFieldName { get; set; } = nameof(ContactPreferenceInputModel.PreferredDay);

        public string TimeFieldName { get; set; } = nameof(ContactPreferenceInputModel.PreferredTime);

        public string NormalizedSelectedTimeSlot => SelectedTimeSlot ?? "Mattina";

        public bool IsCustomTime => NormalizedSelectedTimeSlot == "Altro orario";
    }

    public static class LeadFunnelPartials
    {
        public static LeadFunnelContactPreferencesViewModel SearchContact(ContactPreferenceInputModel model)
        {
            return CreateContact(
                model,
                "Come preferisci essere ricontattato?",
                new List<LeadFunnelContactOptionViewModel>
                {
                    new LeadFunnelContactOptionViewModel(
                        "Essere ricontattato",
                        "Essere ricontattato",
                        "Un agente ti chiama per capire meglio richiesta, tempi e priorita.",
                        "phone-option-icon"),
                    new LeadFunnelContactOptionViewModel(
                        "Consulenza da remoto",
                        "Consulenza da remoto",
                        "Confronto online per ordinare le opzioni prima di un eventuale incontro.",
                        "remote-option-icon"),
                    new LeadFunnelContactOptionViewModel(
                        "Consulenza in agenzia",
                        "Consulenza in agenzia",
                        "Incontro in ufficio per costruire una richiesta piu precisa con un referente.",
                        "office-option-icon")
                });
        }

        public static LeadFunnelContactPreferencesViewModel SellContact(ContactPreferenceInputModel model)
        {
            return CreateContact(
                model,
                "Come preferisci essere contattato",
                new List<LeadFunnelContactOptionViewModel>
                {
                    new LeadFunnelContactOptionViewModel(
                        "Essere ricontattato",
                        "Essere ricontattato",
                        "Telefonata per chiarire valore atteso, urgenza e passaggi successivi.",
                        "phone-option-icon"),
                    new LeadFunnelContactOptionViewModel(
                        "Consulenza da remoto",
                        "Consulenza da remoto",
                        "Primo confronto online se vuoi preparare documenti e scenario di vendita.",
                        "remote-option-icon"),
                    new LeadFunnelContactOptionViewModel(
                        "Consulenza in agenzia",
                        "Consulenza in agenzia",
                        "Incontro in sede per impostare valutazione e piano commerciale.",
                        "office-option-icon")
                });
        }

        public static LeadFunnelContactPreferencesViewModel RentOutContact(ContactPreferenceInputModel model)
        {
            return CreateContact(
                model,
                "Come preferisci essere contattato",
                new List<LeadFunnelContactOptionViewModel>
                {
                    new LeadFunnelContactOptionViewModel(
                        "Essere ricontattato",
                        "Essere ricontattato",
                        "Telefonata per chiarire obiettivo, stato dell'immobile e gestione desiderata.",
                        "phone-option-icon"),
                    new LeadFunnelContactOptionViewModel(
                        "Consulenza da remoto",
                        "Consulenza da remoto",
                        "Confronto online per valutare canone, documenti e strategia di selezione.",
                        "remote-option-icon"),
                    new LeadFunnelContactOptionViewModel(
                        "Consulenza in agenzia",
                        "Consulenza in agenzia",
                        "Incontro in sede per impostare pubblicazione, visite e contratto.",
                        "office-option-icon")
                });
        }

        public static LeadFunnelContactPreferencesViewModel ValuationContact(ContactPreferenceInputModel model)
        {
            return CreateContact(
                model,
                "Come preferisci essere contattato",
                new List<LeadFunnelContactOptionViewModel>
                {
                    new LeadFunnelContactOptionViewModel(
                        "Essere ricontattato",
                        "Essere ricontattato",
                        "Telefonata per chiarire obiettivo della valutazione e dati mancanti.",
                        "phone-option-icon"),
                    new LeadFunnelContactOptionViewModel(
                        "Consulenza da remoto",
                        "Consulenza da remoto",
                        "Prima lettura online se vuoi capire documenti e scenario prima del sopralluogo.",
                        "remote-option-icon"),
                    new LeadFunnelContactOptionViewModel(
                        "Consulenza in agenzia",
                        "Consulenza in agenzia",
                        "Incontro in sede per impostare un percorso di valutazione completo.",
                        "office-option-icon")
                });
        }

        public static LeadFunnelContactPreferencesViewModel VisitContact(ContactPreferenceInputModel model)
        {
            return CreateContact(
                model,
                "Modalita di contatto",
                new List<LeadFunnelContactOptionViewModel>
                {
                    new LeadFunnelContactOptionViewModel(
                        "Essere ricontattato",
                        "Essere ricontattato",
                        "Un agente ti chiama per approfondire le informazioni su questo immobile.",
                        "phone-option-icon"),
                    new LeadFunnelContactOptionViewModel(
                        "Consulenza da remoto",
                        "Consulenza da remoto",
                        "Un confronto online per chiarire i primi dubbi sull'annuncio.",
                        "remote-option-icon"),
                    new LeadFunnelContactOptionViewModel(
                        "Consulenza in agenzia",
                        "Consulenza in agenzia",
                        "Un incontro in ufficio per valutare immobile e prossimi passaggi.",
                        "office-option-icon")
                });
        }

        public static LeadFunnelTimePreferencesViewModel Time(ContactPreferenceInputModel model, string heading = "Quando preferisci essere contattato?")
        {
            return new LeadFunnelTimePreferencesViewModel
            {
                Heading = heading,
                SelectedTimeSlot = model?.PreferredTimeSlot ?? "Mattina",
                PreferredDay = model?.PreferredDay ?? string.Empty,
                PreferredTime = model?.PreferredTime ?? string.Empty
            };
        }

        private static LeadFunnelContactPreferencesViewModel CreateContact(
            ContactPreferenceInputModel model,
            string heading,
            IReadOnlyList<LeadFunnelContactOptionViewModel> options)
        {
            return new LeadFunnelContactPreferencesViewModel
            {
                Heading = heading,
                SelectedValue = model?.PreferredContactMode ?? "Essere ricontattato",
                Options = options
            };
        }
    }

    public abstract class LeadRequestInputModel : ContactPreferenceInputModel
    {
        public virtual LeadFunnelContactPreferencesViewModel ContactPreferences =>
            LeadFunnelPartials.SearchContact(this);

        public virtual LeadFunnelTimePreferencesViewModel TimePreferences =>
            LeadFunnelPartials.Time(this);

        [Required(ErrorMessage = "Inserisci nome e cognome.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Inserisci il telefono.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Inserisci l'email.")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido.")]
        public string Email { get; set; }
    }

    public class BuyLeadRequestViewModel : LeadRequestInputModel
    {
        [Required(ErrorMessage = "Inserisci il budget sostenibile.")]
        public string SustainableBudget { get; set; }

        [Required(ErrorMessage = "Inserisci le zone ideali.")]
        public string DesiredLocations { get; set; }

        public string AcceptableLocations { get; set; }

        [Required(ErrorMessage = "Scegli una tipologia.")]
        public string PropertyType { get; set; }

        [Required(ErrorMessage = "Indica lo stato della ricerca.")]
        public string SearchStage { get; set; }

        [Required(ErrorMessage = "Indica lo stato della verifica banca.")]
        public string FinancingStatus { get; set; }

        [Required(ErrorMessage = "Indica se devi vendere casa prima.")]
        public string PropertyToSellStatus { get; set; }

        public string Timing { get; set; }

        public string PreferencesAndCompromises { get; set; }
    }

    public class RentLeadRequestViewModel : LeadRequestInputModel
    {
        [Required(ErrorMessage = "Inserisci il canone massimo sostenibile.")]
        public string SustainableBudget { get; set; }

        [Required(ErrorMessage = "Inserisci le zone cercate.")]
        public string DesiredLocations { get; set; }

        [Required(ErrorMessage = "Indica chi abitera la casa.")]
        public string HouseholdDescription { get; set; }

        [Required(ErrorMessage = "Indica la situazione lavorativa o di studio.")]
        public string WorkStudySituation { get; set; }

        [Required(ErrorMessage = "Indica le garanzie disponibili.")]
        public string AvailableGuarantees { get; set; }

        [Required(ErrorMessage = "Indica l'ingresso desiderato.")]
        public string DesiredMoveIn { get; set; }

        public string PreferencesAndCompromises { get; set; }
    }

    public class SellLeadRequestViewModel : LeadRequestInputModel
    {
        public override LeadFunnelContactPreferencesViewModel ContactPreferences =>
            LeadFunnelPartials.SellContact(this);

        public override LeadFunnelTimePreferencesViewModel TimePreferences =>
            LeadFunnelPartials.Time(this, "Fascia oraria preferita");

        [Required(ErrorMessage = "Scegli la tipologia dell'immobile.")]
        public string PropertyType { get; set; }

        [Required(ErrorMessage = "Inserisci la zona o un indirizzo indicativo.")]
        public string DesiredLocation { get; set; }

        [Required(ErrorMessage = "Indica lo stato dell'immobile.")]
        public string PropertyCondition { get; set; }

        [Required(ErrorMessage = "Indica la disponibilita dell'immobile.")]
        public string Availability { get; set; }

        [Required(ErrorMessage = "Indica l'urgenza.")]
        public string Timing { get; set; }

        public string ExpectedPriceOrMainQuestion { get; set; }

        public string PreferencesAndCompromises { get; set; }
    }

    public class RentOutLeadRequestViewModel : LeadRequestInputModel
    {
        public override LeadFunnelContactPreferencesViewModel ContactPreferences =>
            LeadFunnelPartials.RentOutContact(this);

        public override LeadFunnelTimePreferencesViewModel TimePreferences =>
            LeadFunnelPartials.Time(this, "Fascia oraria preferita");

        [Required(ErrorMessage = "Scegli la tipologia dell'immobile.")]
        public string PropertyType { get; set; }

        [Required(ErrorMessage = "Inserisci la zona o un indirizzo indicativo.")]
        public string DesiredLocation { get; set; }

        [Required(ErrorMessage = "Indica lo stato e l'arredo.")]
        public string PropertyCondition { get; set; }

        public string ExpectedPriceOrMainQuestion { get; set; }

        [Required(ErrorMessage = "Indica il tipo di contratto desiderato.")]
        public string DesiredContractType { get; set; }

        [Required(ErrorMessage = "Indica la disponibilita.")]
        public string Availability { get; set; }

        public string PreferencesAndCompromises { get; set; }
    }

    public class ValuationRequestViewModel : LeadRequestInputModel
    {
        public override LeadFunnelContactPreferencesViewModel ContactPreferences =>
            LeadFunnelPartials.ValuationContact(this);

        public override LeadFunnelTimePreferencesViewModel TimePreferences =>
            LeadFunnelPartials.Time(this, "Fascia oraria preferita");

        [Required(ErrorMessage = "Inserisci un indirizzo o una zona.")]
        public string DesiredLocation { get; set; }

        [Required(ErrorMessage = "Scegli la tipologia dell'immobile.")]
        public string PropertyType { get; set; }

        [Required(ErrorMessage = "Inserisci una superficie indicativa.")]
        public string IndicativeSurface { get; set; }

        [Required(ErrorMessage = "Indica lo stato generale.")]
        public string PropertyCondition { get; set; }

        public string Appurtenances { get; set; }

        [Required(ErrorMessage = "Indica l'obiettivo della valutazione.")]
        public string ValuationGoal { get; set; }

        public string PreferencesAndCompromises { get; set; }
    }

    public class LeadConfirmationViewModel
    {
        public Guid Id { get; set; }

        public string PathLabel { get; set; }

        public string PreferredContactMode { get; set; }

        public string WhenLabel { get; set; }

        public IReadOnlyList<PropertySummaryCardViewModel> FeaturedProperties { get; set; } =
            new List<PropertySummaryCardViewModel>();
    }
}

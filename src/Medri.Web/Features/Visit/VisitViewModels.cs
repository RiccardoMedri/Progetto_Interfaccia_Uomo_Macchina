using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Medri.Web.Features.LeadIntake;
using Medri.Web.Features.Property;

namespace Medri.Web.Features.Visit
{
    public class PropertyContactStartViewModel
    {
        [Required(ErrorMessage = "Inserisci nome e cognome.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Inserisci il telefono.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Inserisci l'email.")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido.")]
        public string Email { get; set; }

        public string Message { get; set; }
    }

    public class VisitPropertySummaryViewModel
    {
        public string Slug { get; set; }

        public string Contract { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public string PriceLabel { get; set; }

        public string DisplayLocation { get; set; }

        public string SurfaceLabel { get; set; }

        public string RoomsLabel { get; set; }

        public string Tag { get; set; }

        public string NavSection => Contract == "Affitto" ? "Rent" : "Buy";
    }

    public class VisitOptionViewModel : ContactPreferenceInputModel
    {
        public LeadFunnelContactPreferencesViewModel ContactPreferences
        {
            get
            {
                var preferences = LeadFunnelPartials.VisitContact(this);
                preferences.SectionClass = "visit-section medri-request-funnel__section";
                return preferences;
            }
        }

        public LeadFunnelTimePreferencesViewModel TimePreferences
        {
            get
            {
                var preferences = LeadFunnelPartials.Time(this);
                preferences.SectionClass = "visit-section medri-request-funnel__section";
                return preferences;
            }
        }

        public Guid? AppointmentId { get; set; }

        public VisitPropertySummaryViewModel Property { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Inserisci l'email.")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido.")]
        public string Email { get; set; }

        public string Message { get; set; }
    }

    public class VisitReviewViewModel
    {
        public Guid AppointmentId { get; set; }

        public VisitPropertySummaryViewModel Property { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }

        public string DisplayMessage => string.IsNullOrWhiteSpace(Message)
            ? "Nessun messaggio aggiuntivo."
            : Message;

        public string PreferredContactMode { get; set; }

        public string WhenLabel { get; set; }
    }

    public class VisitConfirmationViewModel
    {
        public VisitPropertySummaryViewModel Property { get; set; }

        public IReadOnlyList<PropertySummaryCardViewModel> SimilarProperties { get; set; } =
            new List<PropertySummaryCardViewModel>();
    }
}

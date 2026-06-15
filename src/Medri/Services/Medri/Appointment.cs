using System;
using System.ComponentModel.DataAnnotations;

namespace Medri.Services.Medri
{
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid LeadId { get; set; }

        public Guid? PropertyListingId { get; set; }

        public Guid? AgencyUserId { get; set; }

        public DateTime? ScheduledAtUtc { get; set; }

        [Required, StringLength(80)]
        public string Status { get; set; }

        [Required, StringLength(80)]
        public string RequestType { get; set; }

        public string PreferredContactMode { get; set; }

        public string PreferredTimeSlot { get; set; }

        public string PreferredDay { get; set; }

        public string PreferredTime { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}

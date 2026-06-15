using System;
using System.ComponentModel.DataAnnotations;

namespace Medri.Services.Medri
{
    public class Lead
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? ClientUserId { get; set; }

        [StringLength(30)]
        public string PublicReference { get; set; }

        [StringLength(30)]
        public string InternalReference { get; set; }

        [StringLength(80)]
        public string WorkflowStatus { get; set; }

        [Range(0, 100)]
        public int QualificationPercent { get; set; }

        public string NextAction { get; set; }

        public Guid? AssignedAgencyUserId { get; set; }

        [Required, StringLength(160)]
        public string FullName { get; set; }

        [EmailAddress, StringLength(254)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [Required, StringLength(80)]
        public string SourceChannel { get; set; }

        [StringLength(80)]
        public string RequestType { get; set; }

        public string PreferredContactMode { get; set; }

        public string PreferredTimeSlot { get; set; }

        public string PreferredDay { get; set; }

        public string PreferredTime { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}

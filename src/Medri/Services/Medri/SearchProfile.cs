using System;
using System.ComponentModel.DataAnnotations;

namespace Medri.Services.Medri
{
    public class SearchProfile
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid LeadId { get; set; }

        [StringLength(30)]
        public string PublicReference { get; set; }

        [StringLength(80)]
        public string Status { get; set; }

        [Required, StringLength(500)]
        public string CriteriaSummary { get; set; }

        [EmailAddress, StringLength(254)]
        public string ContactEmail { get; set; }

        [StringLength(1000)]
        public string SourceQueryString { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}

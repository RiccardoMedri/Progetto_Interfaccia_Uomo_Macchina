using System;
using System.ComponentModel.DataAnnotations;

namespace Medri.Services.Medri
{
    public class Interaction
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid LeadId { get; set; }

        [Required, StringLength(80)]
        public string Channel { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        public DateTime OccurredAtUtc { get; set; }
    }
}

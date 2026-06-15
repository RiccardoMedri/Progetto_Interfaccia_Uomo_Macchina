using System;
using System.ComponentModel.DataAnnotations;

namespace Medri.Services.Medri
{
    public class PropertyMedia
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PropertyListingId { get; set; }

        [Required, StringLength(300)]
        public string Url { get; set; }

        [StringLength(200)]
        public string AltText { get; set; }

        public int SortOrder { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Medri.Services.Medri
{
    public class FavoriteProperty
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid PropertyListingId { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}

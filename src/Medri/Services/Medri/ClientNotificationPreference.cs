using System;
using System.ComponentModel.DataAnnotations;

namespace Medri.Services.Medri
{
    public class ClientNotificationPreference
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required, StringLength(40)]
        public string Category { get; set; }

        public bool IsActive { get; set; }

        public bool IsDaily { get; set; }

        public bool IsWeekly { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}

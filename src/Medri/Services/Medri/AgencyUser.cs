using System;
using System.ComponentModel.DataAnnotations;

namespace Medri.Services.Medri
{
    public class AgencyUser
    {
        [Key]
        public Guid Id { get; set; }

        [Required, StringLength(160)]
        public string DisplayName { get; set; }

        [Required, EmailAddress, StringLength(254)]
        public string Email { get; set; }

        [Required, StringLength(80)]
        public string Role { get; set; }

        public bool IsSystemSeed { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Medri.Services.Medri
{
    
    
    
    
    public class ClientSavedSearch
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public string Label { get; set; }

        public string QueryString { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}

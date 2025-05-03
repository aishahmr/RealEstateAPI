using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RealEstateAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Properties = new List<Property>();
            Favorites = new List<Favorite>();
            Filters = new List<Filter>();
            Bookings = new List<Booking>();
        }

        
        [Required]
        [Column("Email")] 
        public override string Email { get; set; } = string.Empty;

        [Column("PhoneNumber")]
        public override string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Optional fields
        public string? Bio { get; set; }
        public string? Gender { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // Navigation properties
        [JsonIgnore]
        public ICollection<Property> Properties { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<Filter> Filters { get; set; }
        public ICollection<Booking> Bookings { get; set; }

        
        public override bool TwoFactorEnabled { get; set; }
    }
}
using System;

namespace RealEstateAPI.Models
{
    public class Favorite
    {
        public Guid Id { get; set; }  // Primary Key
    
        public string UserId { get; set; }  // Foreign Key (User)

        public Guid PropertyId { get; set; }  // Foreign Key (Property)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ApplicationUser User { get; set; }
        public Property Property { get; set; }
    }
}



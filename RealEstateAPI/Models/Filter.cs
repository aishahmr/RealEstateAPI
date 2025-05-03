using System;

namespace RealEstateAPI.Models
{
    public class Filter
    {
        public Guid Id { get; set; }  // Primary Key
        public string UserId { get; set; }  // Foreign Key (User)
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Location { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public string? PropertyType { get; set; } // Apartment, Studio, Villa, Other
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public ApplicationUser User { get; set; }
    }
}


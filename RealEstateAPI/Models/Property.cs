using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealEstateAPI.Models
{
    public class Property
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }

        [JsonPropertyName("Ad title")]
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string? PostalCode { get; set; }

        [Column("Area")]
        [JsonPropertyName("area")]
        [Display(Name = "Area (m²)")]
        public int Size { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Floor { get; set; } // Added for floor number

        [Column("Type")]
        [JsonPropertyName("type")]
        [AllowedValues("Apartment", "apartment", "Villa","villa", "House", "house", ErrorMessage = "Invalid type")]
        public string Type { get; set; } // apartment, villa, etc.
        public string VerificationStatus { get; set; } = "Pending";

        [Column("FurnishingStatus")]
        [JsonPropertyName("furnish status")]
        public string FurnishingStatus { get; set; } // Furnished/Semi-furnished/Not-furnished


        public string Amenities { get; set; } // Comma-separated string (e.g., "Balcony,Garden,Pool")

        // Contact information
        public string yourName { get; set; } // Added for listing contact
        public string MobilePhone { get; set; } // Added for listing contact

        // Media and documents
        public string? DocumentUrl { get; set; }
        public bool IsFeatured { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser User { get; set; }
        public ICollection<Image> Images { get; set; } = new List<Image>();
        public ICollection<PriceEstimate> PriceEstimates { get; set; } = new List<PriceEstimate>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        // Optional: Consider adding these if needed
        // public bool HasParking { get; set; }
        // public bool HasElevator { get; set; }
    }
}
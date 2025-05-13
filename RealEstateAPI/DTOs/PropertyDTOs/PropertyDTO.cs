namespace RealEstateAPI.DTOs.PropertyDTOs
{
    public class PropertyDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } // Added to match UI
        public decimal Price { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string? PostalCode { get; set; }
        public int Size { get; set; } // Consider renaming to 'Area' for UI consistency
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public string Type { get; set; } // apartment, studio, villa
        public string Description { get; set; }
        public string VerificationStatus { get; set; } // pending, verified, rejected
        public string FurnishingStatus { get; set; } 
        public List<string> Amenities { get; set; } // Changed from string to List<string>
        public string DocumentUrl { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Floor { get; set; } // Added to match UI
        public string yourName { get; set; } // Added for UI contact info
        public string MobilePhone { get; set; } // Added for UI contact info

        // Consider adding these if needed for UI gallery
        public List<string> ImageUrls { get; set; }
        public string MainImageUrl { get; set; }
    }
}
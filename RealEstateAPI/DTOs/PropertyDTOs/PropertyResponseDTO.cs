namespace RealEstateAPI.DTOs.PropertyDTOs
{
    public class PropertyResponseDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? OwnerName { get; set; }
        public string? ContactInfo { get; set; }
        public bool IsOwner { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; } 
        public string Location { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Floor { get; set; }
        public List<string> Images { get; set; } = new();

        public int Area { get; set; } // Matches Property.Size (int)

        // UI-Specific Fields
        public string FurnishStatus { get; set; } // Note: Matches controller's naming
        public string MainImageUrl { get; set; }  // Change from computed to settable
        public List<string> Amenities { get; set; } = new(); // Empty list by default
        public DateTime CreatedAt { get; set; }

        // Computed Properties
        public string ShortDescription =>
            string.IsNullOrEmpty(Description) ?
            string.Empty :
            (Description.Length > 100 ? Description[..100] + "..." : Description);

        // Additional UI Helpers
        public string PriceFormatted => Price.ToString("C0"); // $1,234 format
        public string LocationShort => Location?.Split(',').FirstOrDefault() ?? Location;
    }
}
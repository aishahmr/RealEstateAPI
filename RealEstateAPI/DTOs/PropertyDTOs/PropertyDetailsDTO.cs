namespace RealEstateAPI.DTOs.PropertyDTOs
{
    public class PropertyDetailsDTO 
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        

        public  string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string? PostalCode { get; set; }

        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Area { get; set; }
        public string FurnishStatus { get; set; } 
        public List<string> Amenities { get; set; }
        public List<string> Images { get; set; } = new();
        public string OwnerName { get; set; }
        public bool IsOwner { get; set; }
        public string ContactInfo { get; set; }

        // Added from controller usage
        public string VerificationStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        // Optional additional fields
        public int? Floor { get; set; }
        public string? Type { get; set; }
        public bool? IsFeatured { get; set; }
                public string ShortDescription => Description.Length > 100 ?
        Description[..100] + "..." : Description;
        public string PriceFormatted => Price.ToString("C0");
        public string LocationShort => $"{City}, {Governorate}";


    }
}
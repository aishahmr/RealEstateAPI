using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace RealEstateAPI.DTOs.PropertyDTOs
{
    public class AddPropertyWithImagesDTO
    {
        // Property fields from AddPropertyDTO
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string AddressLine1 { get; set; } 
        public string? AddressLine2 { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string? PostalCode { get; set; }

        public int Bedrooms { get; set; }

        public int Bathrooms { get; set; }

        [Display(Name = "Area (m²)")]
        public double Area { get; set; }

        [Required]
        public string YourName { get; set; }

        [Required]
        public string MobilePhone { get; set; }

        [Required]
        public string FurnishStatus { get; set; } = "Not-furnished";

        public List<string> Amenities { get; set; } = new();

        public int Floor { get; set; }

        [AllowedValues("Apartment", "Villa", "House", ErrorMessage = "Invalid type")]
        public string? Type { get; set; } = "Apartment";

        // Image fields
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
}
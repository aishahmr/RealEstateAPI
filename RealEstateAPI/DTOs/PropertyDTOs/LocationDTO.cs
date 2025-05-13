using System.ComponentModel.DataAnnotations;

namespace RealEstateAPI.DTOs.PropertyDTOs
{
    public class LocationDTO
    {
        [Required]
        public string AddressLine1 { get; set; } // e.g., "35 Nile Corniche"

        public string? AddressLine2 { get; set; } // e.g., "Floor 4, Apartment 12" (optional)

        [Required]
        public string City { get; set; } // e.g., "Garden City"

        [Required]
        public string Governorate { get; set; } // e.g., "Cairo"

        public string? PostalCode { get; set; } // e.g., "11511" (optional)
    }
}
using RealEstateAPI.DTOs.PropertyDTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class AddPropertyDTO
{
    [Required]
    [JsonPropertyName("Ad title")]
    public string Title { get; set; }

    [Required]
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [Required]
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [Required]
    public string AddressLine1 { get; set; } // e.g., "35 Nile Corniche"

    public string? AddressLine2 { get; set; } // e.g., "Floor 4, Apartment 12" (optional)

    [Required]
    public string City { get; set; } // e.g., "Garden City"

    [Required]
    public string Governorate { get; set; } // e.g., "Cairo"

    public string? PostalCode { get; set; } // e.g., "11511" (optional)

    [JsonPropertyName("bedrooms")]
    public int Bedrooms { get; set; }

    [JsonPropertyName("bathrooms")]
    public int Bathrooms { get; set; }

    [Column("Area")]
    [JsonPropertyName("area")]
    [Display(Name = "Area (m²)")]
    public double Area { get; set; }

    [Required]
    [JsonPropertyName("yourName")] 
    public string YourName { get; set; }

    [Required]
    [JsonPropertyName("mobilePhone")] 
    public string MobilePhone { get; set; }

    [Column("FurnishingStatus")]
    [JsonPropertyName("furnish status")] 
    [Required]
    public string FurnishStatus { get; set; } = "Not-furnished";

    [JsonPropertyName("amenities")]
    public List<string> Amenities { get; set; } = new();

    [JsonPropertyName("floor")]
    public int Floor { get; set; }

    [Column("Type")]
    [JsonPropertyName("type")]
    [AllowedValues("Apartment", "Villa", "House", ErrorMessage = "Invalid type")]
    public string? Type { get; set; } = "Apartment";
}

using RealEstateAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;

public class Property
{
    public Guid Id { get; set; }
    public string UserId { get; set; }

    [JsonPropertyName("Ad title")]
    public string Title { get; set; }

    public string Description { get; set; }

        [Column("Price_2025")]
    public decimal Price2025 { get; set; }

    [Column("Area")]
    [JsonPropertyName("area")]
    [Display(Name = "Area (sqm)")]
    public int Size { get; set; } // matches "Area (sqm)"

    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }

    [Column("Floor_Level")]
    public int FloorLevel { get; set; }

    [Name("Building Age (years)")]
    public int BuildingAge { get; set; } // "Building Age (years)"

    [Column("Type")]
    [JsonPropertyName("type")]
    public string Type { get; set; } // "Property Type"

    public string FurnishingStatus { get; set; } // Furnishing Status

    public string Amenities { get; set; } // Comma-separated

    public string NearbyFacility { get; set; }

    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string City { get; set; } // Map to "Location"
    public string Governorate { get; set; }
    public string? PostalCode { get; set; }

    public string yourName { get; set; }
    public string MobilePhone { get; set; }

    public string VerificationStatus { get; set; } = "Pending";
    public bool IsFeatured { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? DocumentUrl { get; set; }

    // Navigation properties
    public ApplicationUser User { get; set; }
    public ICollection<Image> Images { get; set; } = new List<Image>();
    public ICollection<PriceEstimate> PriceEstimates { get; set; } = new List<PriceEstimate>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

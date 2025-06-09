using CsvHelper.Configuration.Attributes;
namespace RealEstateAPI.Models
{
    public class PropertyCSVRow
    {
        [Name("Location")]
        public string City { get; set; }

        [Name("Area (sqm)")]
        public int Size { get; set; }

        [Name("Property Type")]
        public string Type { get; set; }

        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }

        [Name("Building Age (years)")]
        public int BuildingAge { get; set; }

        public string FurnishingStatus { get; set; }

        [Name("Floor Level")]
        public int FloorLevel { get; set; }

        public string Amenities { get; set; }

        public string NearbyFacility { get; set; }

        [Name("Price 2023 (EGP)")]
        public decimal Price2023 { get; set; }

        [Name("Price 2024 (EGP)")]
        public decimal Price2024 { get; set; }

        [Name("Price 2025 (EGP)")]
        public decimal Price2025 { get; set; }


    }
}

// DTOs/HomeDTOs.cs
using RealEstateAPI.DTOs.PropertyDTOs;

namespace RealEstateAPI.DTOs.HomeDTOs
{
    public class HomePageResponseDTO
    {
        public List<RecommendedPropertyDTO> RecommendedProperties { get; set; }
        public List<PropertyResponseDTO> AllProperties { get; set; }
    }

    public class FeaturedPropertyDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public string BedBathInfo { get; set; } // "3:1" format
    }
}
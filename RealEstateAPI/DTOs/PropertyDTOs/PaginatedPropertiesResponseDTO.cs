using RealEstateAPI.DTOs.PropertyDTOs;

namespace RealEstateAPI.DTOs.PropertyDTOs
{
    public class PaginatedPropertiesResponseDTO
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<PropertyResponseDTO> Properties { get; set; }

    }
}

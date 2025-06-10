using RealEstateAPI.DTOs.HomeDTOs;
using RealEstateAPI.DTOs.PropertyDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RealEstateAPI.Service.IServices
{
    public interface IPropertyService
    {
        Task<List<PropertyResponseDTO>> GetAllPropertiesAsync(
            string? searchTerm,
            string? location,
            decimal? minPrice,
            decimal? maxPrice,
            int? minSize,
            int? maxSize,
            int? bedrooms,
            int? bathrooms,
            string? type,
            string? furnishingStatus,
            List<string>? amenities,
            string? currentUserId = null);
        Task<HomePageResponseDTO> GetHomePageDataAsync();
        Task<PaginatedPropertiesResponseDTO> GetUserPropertiesPaginatedAsync(string userId,int page = 1,int pageSize = 10);
        Task<PropertyResponseDTO> AddPropertyAsync(AddPropertyDTO propertyDto, string userId);
        Task<List<string>> UploadImagesAsync(Guid propertyId, List<IFormFile> images);
        Task<bool> UpdatePropertyAsync(UpdatePropertyDTO propertyDto, string currentUserId); 
        Task<bool> DeletePropertyAsync(Guid id, string currentUserId);
        Task<List<PropertyResponseDTO>> GetPropertiesNearUser(string userId, int radiusKm = 10);
        Task<List<PropertyResponseDTO>> GetPropertiesByUserIdAsync(string userId);
    }
}
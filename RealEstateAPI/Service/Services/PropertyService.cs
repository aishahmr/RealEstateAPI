using RealEstateAPI.Models;
using RealEstateAPI.DTOs.PropertyDTOs;
using RealEstateAPI.Models.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealEstateAPI.Service.IServices;
using RealEstateAPI.DTOs.HomeDTOs;
using Microsoft.AspNetCore.Identity;

namespace RealEstateAPI.Service.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PropertyService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Original parameterless version
        public async Task<List<PropertyResponseDTO>> GetAllPropertiesAsync()
        {
            return await GetAllPropertiesAsync(
                searchTerm: null,
                location: null,
                minPrice: null,
                maxPrice: null,
                minSize: null,
                maxSize: null,
                bedrooms: null,
                bathrooms: null,
                type: null,
                furnishingStatus: null,
                amenities: null
                );
        }

        // New filtered version
        public async Task<List<PropertyResponseDTO>> GetAllPropertiesAsync(
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
            string? currentUserId = null)
        {
            var query = _context.Properties
                .AsNoTracking()
                .Include(p => p.Images)
                .Include(p => p.User)
                .AsQueryable();

            // Search by text (title or description)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.Title.ToLower().Contains(searchTerm) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
            }

            // Location filter
            if (!string.IsNullOrWhiteSpace(location))
{
    location = location.Trim().ToLower();
    query = query.Where(p => 
        p.City.ToLower().Contains(location) ||
        p.Governorate.ToLower().Contains(location) ||
        p.AddressLine1.ToLower().Contains(location));
}

            // Price range
            if (minPrice.HasValue)
                query = query.Where(p => p.Price2025 >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price2025 <= maxPrice.Value);

            // Size range
            if (minSize.HasValue)
                query = query.Where(p => p.Size >= minSize.Value);
            if (maxSize.HasValue)
                query = query.Where(p => p.Size <= maxSize.Value);

            // Bedrooms and bathrooms
            if (bedrooms.HasValue)
                query = query.Where(p => p.Bedrooms == bedrooms.Value);
            if (bathrooms.HasValue)
                query = query.Where(p => p.Bathrooms == bathrooms.Value);

            // Property type
            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(p => p.Type == type);

            // Furnishing status
            if (!string.IsNullOrWhiteSpace(furnishingStatus))
                query = query.Where(p => p.FurnishingStatus == furnishingStatus);

            // Amenities filter
            if (amenities != null && amenities.Any())
            {
                foreach (var amenity in amenities)
                {
                    query = query.Where(p => p.Amenities.Contains(amenity));
                }
            }

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PropertyResponseDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    OwnerName = p.yourName ?? p.User.UserName,
                    ContactInfo = p.MobilePhone,
                    Price = p.Price2025,
                    AddressLine1 = p.AddressLine1,
                    AddressLine2 = p.AddressLine2,
                    City = p.City,
                    Governorate = p.Governorate,
                    PostalCode = p.PostalCode,
                    Bedrooms = p.Bedrooms,
                    Bathrooms = p.Bathrooms,
                    Area = p.Size,
                    FurnishStatus = p.FurnishingStatus,
                    Type = p.Type,
                    Images = p.Images.Select(i => i.Url).ToList(),
                    MainImageUrl = p.Images.OrderBy(i => i.CreatedAt)
                                         .Select(i => i.Url)
                                         .FirstOrDefault() ?? "default-image.jpg",
                    Amenities = !string.IsNullOrEmpty(p.Amenities)
                        ? p.Amenities.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                        : new List<string>(),
                    IsOwner = currentUserId != null &&
                     p.UserId.ToString().ToLower() == currentUserId.ToLower(),
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<PropertyResponseDTO> GetPropertyByIdAsync(Guid id, string currentUserId = null)
        {
            var property = await _context.Properties
                .Include(p => p.Images)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null) return null;

            return new PropertyResponseDTO
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description,
                OwnerName = property.yourName,
                ContactInfo = property.MobilePhone,
                Price = property.Price2025,
                AddressLine1 = property.AddressLine1,
                AddressLine2 = property.AddressLine2,
                City = property.City,
                Governorate = property.Governorate,
                PostalCode = property.PostalCode,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                Area = property.Size,
                FurnishStatus = property.FurnishingStatus,
                MainImageUrl = property.Images.FirstOrDefault()?.Url ?? "default-image.jpg",
                Images = property.Images.Select(i => i.Url).ToList(),
                Amenities = property.Amenities?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                           ?? new List<string>(),
                IsOwner = currentUserId != null &&
                 property.UserId.Equals(currentUserId, StringComparison.OrdinalIgnoreCase),
                CreatedAt = property.CreatedAt
            };
        }

        public async Task<PropertyResponseDTO> AddPropertyAsync(AddPropertyDTO propertyDto, string userId)
        {
            var property = new Property
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = propertyDto.Title,
                Description = propertyDto.Description,
                Price2025 = propertyDto.Price,
                AddressLine1 = propertyDto.AddressLine1,
                AddressLine2 = propertyDto.AddressLine2,
                City = propertyDto.City,
                Governorate = propertyDto.Governorate,
                PostalCode = propertyDto.PostalCode,
                Bedrooms = propertyDto.Bedrooms,
                Bathrooms = propertyDto.Bathrooms,
                Size = (int)propertyDto.Area,
                FurnishingStatus = propertyDto.FurnishStatus,
                Amenities = string.Join(",", propertyDto.Amenities ?? new List<string>()),
                Type = propertyDto.Type ?? "Apartment",
                VerificationStatus = "Pending",
                CreatedAt = DateTime.UtcNow,
                yourName = propertyDto.YourName,
                MobilePhone = propertyDto.MobilePhone
            };

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            return new PropertyResponseDTO
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description,
                OwnerName = property.yourName, 
                ContactInfo = property.MobilePhone,
                IsOwner = true,
                Price = property.Price2025,
                AddressLine1 = property.AddressLine1,
                AddressLine2 = property.AddressLine2,
                City = property.City,
                Governorate = property.Governorate,
                PostalCode = property.PostalCode,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                Area = property.Size,
                FurnishStatus = property.FurnishingStatus,
                Amenities = property.Amenities?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                           ?? new List<string>()
            };
        }

        public async Task<bool> UpdatePropertyAsync(UpdatePropertyDTO propertyDto, string currentUserIdentifier)
        {
            var property = await _context.Properties
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == propertyDto.Id);

            if (property == null) return false;

            // Flexible ownership check - works with both email or GUID user ID
            bool isOwner;

            if (currentUserIdentifier.Contains("@"))
            {
                // Email comparison
                isOwner = property.User?.Email != null &&
                         property.User.Email.Equals(currentUserIdentifier, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                // GUID comparison
                isOwner = property.UserId != null &&
                         property.UserId.Equals(currentUserIdentifier, StringComparison.OrdinalIgnoreCase);
            }

            if (!isOwner) return false;

            // Property update logic
            property.Title = propertyDto.Title;
            property.Description = propertyDto.Description;
            property.Price2025 = propertyDto.Price;
            property.AddressLine1 = propertyDto.AddressLine1;
            property.AddressLine2 = propertyDto.AddressLine2;
            property.City = propertyDto.City;
            property.Governorate = propertyDto.Governorate;
            property.PostalCode = propertyDto.PostalCode;
            property.Bedrooms = propertyDto.Bedrooms;
            property.Bathrooms = propertyDto.Bathrooms;
            property.Size = (int)propertyDto.Area;
            property.FurnishingStatus = propertyDto.FurnishStatus;
            property.Amenities = string.Join(",", propertyDto.Amenities ?? new List<string>());

            _context.Properties.Update(property);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePropertyAsync(Guid id, string currentUserIdentifier)
        {
            var property = await _context.Properties
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null) return false;

            // Flexible ownership check
            bool isOwner;

            if (currentUserIdentifier.Contains("@"))
            {
                isOwner = property.User?.Email != null &&
                         property.User.Email.Equals(currentUserIdentifier, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                isOwner = property.UserId != null &&
                         property.UserId.Equals(currentUserIdentifier, StringComparison.OrdinalIgnoreCase);
            }

            if (!isOwner) return false;

            _context.Properties.Remove(property);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<PropertyResponseDTO>> GetPropertiesByUserIdAsync(string userId)
        {
            var properties = await _context.Properties
                .Where(p => p.UserId == userId)
                .Include(p => p.Images)
                .ToListAsync();

            return properties.Select(p => new PropertyResponseDTO
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Price = p.Price2025,
                AddressLine1 = p.AddressLine1,
                AddressLine2 = p.AddressLine2,
                City = p.City,
                Governorate = p.Governorate,
                PostalCode = p.PostalCode,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Area = p.Size,
                FurnishStatus = p.FurnishingStatus,
                Images = p.Images.Select(i => i.Url).ToList(), // Set Images list
                CreatedAt = p.CreatedAt
            }).ToList();
        }

        public async Task<List<string>> UploadImagesAsync(Guid propertyId, List<IFormFile> images)
        {
            var property = await _context.Properties.FindAsync(propertyId);
            if (property == null)
                throw new ArgumentException("Property not found");

            var imageUrls = new List<string>();
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            foreach (var image in images)
            {
                if (image.Length == 0) continue;

                var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/{uniqueFileName}";

                _context.Images.Add(new Image
                {
                    Url = imageUrl,
                    PropertyId = propertyId,
                    FileName = image.FileName,
                    FileSize = image.Length,
                    ContentType = image.ContentType
                });

                imageUrls.Add(imageUrl);
            }

            await _context.SaveChangesAsync();
            return imageUrls;
        }

        public async Task<HomePageResponseDTO> GetHomePageDataAsync()
        {
            // Get 6 most recent properties (matching UI exactly)
            var recentProperties = await _context.Properties
                .OrderByDescending(p => p.CreatedAt)
                .Take(6)
                .Select(p => new RecommendedPropertyDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    City = p.City, 
                    PriceFormatted = $"${p.Price2025:n0}", // Formats as "$20,000"
                    AreaFormatted = $"{p.Size} sqft",  // Formats as "2360 sqft"
                    ImageUrl = p.Images.FirstOrDefault().Url ?? "default-image.jpg"
                })
                .ToListAsync();

            return new HomePageResponseDTO
            {
                RecommendedProperties = recentProperties, // Now matches UI perfectly
                AllProperties = new List<PropertyResponseDTO>() // Omit if not needed
            };
        }

        public async Task<List<PropertyResponseDTO>> GetPropertiesNearUser(string userId, int radiusKm = 10)
        {
            // 1. Get user's location
            var user = await _userManager.FindByIdAsync(userId);
            if (user?.City == null || user.Governorate == null)
                return new List<PropertyResponseDTO>();

            // 2. Find properties in same governorate (basic implementation)
            return await _context.Properties
                .Where(p => p.Governorate == user.Governorate)
                .OrderBy(p => p.City == user.City ? 0 : 1) // Prioritize same city
                .Take(20) // Limit results
                .Select(p => new PropertyResponseDTO
                {
                    // Core fields
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price2025,

                    // Location fields (REQUIRED for LocationShort computation)
                    City = p.City,
                    Governorate = p.Governorate,
                    AddressLine1 = p.AddressLine1,
                    AddressLine2 = p.AddressLine2,
                    PostalCode = p.PostalCode,

                    // Property details
                    Bedrooms = p.Bedrooms,
                    Bathrooms = p.Bathrooms,
                    Floor = p.FloorLevel,
                    Area = p.Size,
                    FurnishStatus = p.FurnishingStatus,
                    Type = p.Type,
                    CreatedAt = p.CreatedAt,
                    IsOwner = p.UserId == userId, // Critical fix
                    OwnerName = p.UserId == userId ? "You" : p.yourName ?? "Owner",
                    ContactInfo = p.UserId == userId ? user.PhoneNumber : p.MobilePhone ?? "Contact unavailable",

                    // Images
                    Images = p.Images.OrderBy(i => i.CreatedAt).Select(i => i.Url).ToList(),
                    MainImageUrl = p.Images.OrderBy(i => i.CreatedAt)
                             .Select(i => i.Url)
                             .FirstOrDefault() ?? "/images/default.jpg",

                    // Amenities
                    Amenities = !string.IsNullOrEmpty(p.Amenities)
            ? p.Amenities.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            : new List<string>()

                    // LocationShort is automatically computed from City/Governorate!
                })
    .AsNoTracking()
    .ToListAsync();
        }



    }
}
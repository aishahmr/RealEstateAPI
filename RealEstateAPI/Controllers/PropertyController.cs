using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RealEstateAPI.DTOs.PropertyDTOs;
using RealEstateAPI.Models;
using RealEstateAPI.Models.Data;
using RealEstateAPI.Service.IServices;
using System.Diagnostics;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPropertyService _propertyService;
        private readonly UserManager<ApplicationUser> _userManager;    


        public PropertyController(AppDbContext context, IPropertyService propertyService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _propertyService = propertyService;
            _userManager = userManager;

        }

        [HttpGet("GetAllProperties")]
        public async Task<IActionResult> GetAllProperties(
            [FromQuery] string? searchTerm,
            [FromQuery] string? location,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int? minSize,
            [FromQuery] int? maxSize,
            [FromQuery] int? bedrooms,
            [FromQuery] int? bathrooms,
            [FromQuery] string? type,
            [FromQuery] string? furnishingStatus,
            [FromQuery] List<string>? amenities)
        {
            var userIdClaim = User.FindFirstValue("userId") ??
                      User.FindFirstValue(ClaimTypes.NameIdentifier);

            var properties = await _propertyService.GetAllPropertiesAsync(
                searchTerm,
                location,
                minPrice,
                maxPrice,
                minSize,
                maxSize,
                bedrooms,
                bathrooms,
                type,
                furnishingStatus,
                amenities,
                userIdClaim
            );
            return Ok(properties);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PropertyResponseDTO>> AddPropertyWithImages(
    [FromForm] AddPropertyWithImagesDTO dto)
        {
            // 1. VALIDATION
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Validation failed",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            // 2. GET USER
            // 2. GET USER ID FROM TOKEN
            var userIdClaim = User.FindFirstValue("userId") ??
                              User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Invalid user token");
            }

            var userId = Guid.Parse(userIdClaim); // parse claim into real GUID


            // 3. CREATE PROPERTY ENTITY
            var property = new Property
            {
                Id = Guid.NewGuid(),
                UserId = userId.ToString(),
                Title = dto.Title?.Trim(),
                Description = dto.Description?.Trim(),
                Price2025 = dto.Price,
                AddressLine1 = dto.AddressLine1?.Trim(),
                AddressLine2 = dto.AddressLine2?.Trim(),
                City = dto.City?.Trim(),
                Governorate = dto.Governorate?.Trim(),
                PostalCode = dto.PostalCode?.Trim(),

                Bedrooms = dto.Bedrooms,
                Bathrooms = dto.Bathrooms,
                Size = (int)(dto.Area > 0 ? dto.Area : 0),
                yourName = dto.YourName?.Trim() ?? User.Identity?.Name,
                MobilePhone = dto.MobilePhone?.Trim(),
                FurnishingStatus = dto.FurnishStatus ?? "Not Furnished",
                Amenities = dto.Amenities != null ? string.Join(",", dto.Amenities) : "",
                Type = dto.Type ?? "Apartment",
                VerificationStatus = "Pending",
                CreatedAt = DateTime.UtcNow,
                FloorLevel = dto.Floor
            };

            // 4. DATABASE TRANSACTION
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 5. SAVE PROPERTY FIRST
                _context.Properties.Add(property);
                await _context.SaveChangesAsync();

                var imageUrls = new List<string>();

                // 6. HANDLE IMAGES IF ANY
                if (dto.Files != null && dto.Files.Count > 0)
                {
                    var uploadsFolder = Path.Combine("wwwroot", "uploads", "properties");
                    Directory.CreateDirectory(uploadsFolder);

                    foreach (var file in dto.Files)
                    {
                        if (file.Length == 0) continue;

                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var imageUrl = $"/uploads/properties/{fileName}";

                        _context.Images.Add(new Image
                        {
                            Url = imageUrl,
                            PropertyId = property.Id,
                            FileName = file.FileName,
                            FileSize = file.Length,
                            ContentType = file.ContentType
                        });

                        imageUrls.Add(imageUrl);
                    }
                }

                // 7. FINAL SAVE
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // 8. RETURN RESPONSE
                return Ok(new PropertyResponseDTO
                {
                    Id = property.Id,
                    Images = imageUrls,
                    MainImageUrl = imageUrls.FirstOrDefault() ?? "/images/default.jpg",
                    Title = property.Title,
                    Description = property.Description,
                    OwnerName = property.yourName, // Add this
                    ContactInfo = property.MobilePhone, // Add this
                    IsOwner = true,
                    Price = property.Price2025,
                    AddressLine1 = dto.AddressLine1?.Trim(),  
                    AddressLine2 = dto.AddressLine2?.Trim(),
                    City = dto.City?.Trim(),
                    Governorate = dto.Governorate?.Trim(),
                    PostalCode = dto.PostalCode?.Trim(),
                    Bedrooms = property.Bedrooms,
                    Bathrooms = property.Bathrooms,
                    Floor = property.FloorLevel,
                    Area = property.Size,
                    FurnishStatus = property.FurnishingStatus,
                    Amenities = property.Amenities?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                    CreatedAt = property.CreatedAt,
                    Type = property.Type
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    Message = "Error creating property",
                    Error = ex.Message
                });
            }
        }


        [HttpPost]
        [Authorize]
        [Route("api/Property/Basic")] 
        [Obsolete("Use AddPropertyWithImages instead")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> AddProperty([FromBody] AddPropertyDTO propertyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Validation failed",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            // . Get user with case-insensitive search
            var userId = User.FindFirstValue("userId") ??
                        User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => EF.Functions.Collate(u.Id, "SQL_Latin1_General_CP1_CI_AS") == userId);

            if (user == null)
            {
                return BadRequest(new
                {
                    Message = "User account not found",
                    DebugInfo = new
                    {
                        ClaimUserId = userId,
                        FirstUserIdInDb = await _context.Users.Select(u => u.Id).FirstOrDefaultAsync()
                    }
                });
            }

            var property = new Property
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,    
                Title = propertyDto.Title?.Trim(),
                Description = propertyDto.Description?.Trim(),
                Price2025 = propertyDto.Price,
                AddressLine1 = propertyDto.AddressLine1?.Trim(),
                AddressLine2 = propertyDto.AddressLine2?.Trim(),
                City = propertyDto.City?.Trim(),
                Governorate = propertyDto.Governorate?.Trim(),
                PostalCode = propertyDto.PostalCode?.Trim(),
                Bedrooms = propertyDto.Bedrooms,
                Bathrooms = propertyDto.Bathrooms,
                Size = (int)(propertyDto.Area > 0 ? propertyDto.Area : 0),
                yourName = propertyDto.YourName?.Trim(),
                MobilePhone = propertyDto.MobilePhone?.Trim(),
                FurnishingStatus = propertyDto.FurnishStatus ?? "Not Furnished",
                Amenities = propertyDto.Amenities != null ? string.Join(",", propertyDto.Amenities) : "",
                Type = propertyDto.Type ?? "Apartment",
                VerificationStatus = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Properties.Add(property);
                await _context.SaveChangesAsync();

                return Ok(new PropertyResponseDTO
                {
                    Id = property.Id,
                    Title = property.Title,
                    Description = property.Description,
                    Price = property.Price2025,
                    AddressLine1 = property.AddressLine1,
                    AddressLine2 = property.AddressLine2,
                    City = property.City,
                    Governorate = property.Governorate,
                    PostalCode = property.PostalCode,
                    Bedrooms = property.Bedrooms,
                    Bathrooms = property.Bathrooms,
                    Area = property.Size,
                    FurnishStatus = property.FurnishingStatus
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    Message = "Database error",
                    Error = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Server error",
                    Error = ex.Message
                });
            }
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(
            Guid id,
            [FromBody] UpdatePropertyDTO propertyDto, // JSON data
            [FromForm] UpdatePropertyImagesDTO? imageDto = null) // Optional form-data
        {
            // 1. Authentication (unchanged)
            var currentUserId = User.FindFirst("userId")?.Value
                             ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            // 2. Get property with images
            var property = await _context.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null) return NotFound();

            // 3. Core property update (unchanged)
            var result = await _propertyService.UpdatePropertyAsync(propertyDto, currentUserId);
            if (!result)
            {
                var debugInfo = await _context.Properties
                    .Where(p => p.Id == id)
                    .Select(p => new
                    {
                        PropertyExists = true,
                        CurrentUserEmail = currentUserId,
                        PropertyOwnerId = p.UserId,
                        OwnerEmail = p.User.Email,
                        p.VerificationStatus
                    })
                    .FirstOrDefaultAsync();

                return StatusCode(403, new
                {
                    Message = "Update failed - ownership or validation issue",
                    DebugInfo = debugInfo ?? new
                    {
                        PropertyExists = false,
                        CurrentUserEmail = (string)null,
                        PropertyOwnerId = (string)null,
                        OwnerEmail = (string)null,
                        VerificationStatus = (string)null
                    }
                });
            }

            // 4. Handle images if provided
            if (imageDto != null)
            {
                // Remove specified images
                if (imageDto.ImagesToRemove?.Any() == true)
                {
                    var imagesToDelete = property.Images
                        .Where(img => imageDto.ImagesToRemove.Contains(img.Url))
                        .ToList();

                    foreach (var img in imagesToDelete)
                    {
                        var filePath = Path.Combine("wwwroot", img.Url.TrimStart('/'));
                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);
                        _context.Images.Remove(img);
                    }
                }

                // Add new images
                if (imageDto.NewImages?.Any() == true)
                {
                    var uploadsDir = Path.Combine("wwwroot", "uploads", "properties");
                    Directory.CreateDirectory(uploadsDir);

                    foreach (var file in imageDto.NewImages)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var filePath = Path.Combine(uploadsDir, fileName);

                        using (var stream = System.IO.File.Create(filePath))
                            await file.CopyToAsync(stream);

                        property.Images.Add(new Image
                        {
                            Url = $"/uploads/properties/{fileName}",
                            FileName = file.FileName,
                            ContentType = file.ContentType,
                            PropertyId = property.Id
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/images")]
        [Authorize]
        public async Task<IActionResult> UpdatePropertyImages(
    Guid id,
    [FromForm] UpdatePropertyImagesDTO dto)
        {
            // Handle image updates only
            if (dto.ImagesToRemove?.Any() == true)
            {
                var images = await _context.Images
                    .Where(i => i.PropertyId == id && dto.ImagesToRemove.Contains(i.Url))
                    .ToListAsync();

                foreach (var img in images)
                {
                    System.IO.File.Delete(Path.Combine("wwwroot", img.Url.TrimStart('/')));
                    _context.Images.Remove(img);
                }
            }

            if (dto.NewImages?.Any() == true)
            {
                foreach (var file in dto.NewImages)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine("wwwroot", "uploads", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                        await file.CopyToAsync(stream);

                    _context.Images.Add(new Image
                    {
                        Url = $"/uploads/{fileName}",
                        PropertyId = id,
                        FileName = file.FileName,
                        ContentType = file.ContentType 

                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(Guid id)
        {
   
            var currentUserIdentifier = User.FindFirst("userId")?.Value
                                     ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                     ?? User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(currentUserIdentifier))
            {
                return Unauthorized(new { Message = "User identification not found in token" });
            }

            // 2. Attempt deletion
            var result = await _propertyService.DeletePropertyAsync(id, currentUserIdentifier);

            if (!result)
            {
                var propertyExists = await _context.Properties.AnyAsync(p => p.Id == id);

                return propertyExists
                    ? Forbid()
                    : NotFound();
            }

            return NoContent();
        }


        [HttpPost("{propertyId}/upload-images")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> UploadPropertyImages(Guid propertyId, [FromForm] UploadPropertyImagesDTO dto)
        {
            if (dto.Files == null || !dto.Files.Any())
                return BadRequest("No files uploaded");

            var property = await _context.Properties.FindAsync(propertyId);
            if (property == null) return NotFound("Property not found");

            try
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "properties");
                Directory.CreateDirectory(uploadsFolder);

                var imageUrls = new List<string>();

                foreach (var file in dto.Files)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var imageUrl = $"/uploads/properties/{fileName}";

                    _context.Images.Add(new Image
                    {
                        Url = imageUrl,
                        PropertyId = propertyId,
                        FileName = file.FileName,
                        FileSize = file.Length,
                        ContentType = file.ContentType
                    });

                    imageUrls.Add(imageUrl);
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    imageUrls,
                    message = "Files uploaded and linked to property successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error uploading files: " + ex.Message);
            }
        }

        private static Exception GetInnermostException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex;
        }

        private static bool IsColumnMissingError(string message)
        {
            return message.Contains("Invalid column name") ||
                   message.Contains("column does not exist");
        }


        [HttpGet("GetHomePageData")]
        public async Task<IActionResult> GetHomePageData()
        {
            var response = await _propertyService.GetHomePageDataAsync();
            return Ok(response);
        }



        [Authorize] 
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PropertyDetailsDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<PropertyDetailsDTO>> GetPropertyById(Guid id)
        {
            var emailClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var guidClaim = User.FindFirstValue("userId") ??
                           User.FindFirstValue(ClaimTypes.NameIdentifier);

            var property = await _context.Properties
                .Include(p => p.Images)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null) return NotFound();
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)?.ToLower().Trim();

            bool isOwner = !string.IsNullOrEmpty(guidClaim) &&
                          property.UserId.ToString() == guidClaim;


            return new PropertyDetailsDTO
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description,
                ContactInfo = property.MobilePhone,
                IsOwner = isOwner,
                OwnerName = property.yourName ?? property.User?.UserName,
                Price = property.Price2025,
                AddressLine1=property.AddressLine1,
                AddressLine2 = property.AddressLine2,
                City = property.City,
                Governorate = property.Governorate,
                PostalCode = property.PostalCode,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                Area = property.Size,
                FurnishStatus = property.FurnishingStatus,
                Amenities = property.Amenities?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                           ?? new List<string>(),
                Images = property.Images.Select(i => i.Url).ToList(),
                CreatedAt = property.CreatedAt,
                Floor = property.FloorLevel,
                Type = property.Type,
                // Computed fields

            };
        }



        [HttpGet("near-me")]
        [Authorize]
        public async Task<IActionResult> GetPropertiesNearMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var properties = await _propertyService.GetPropertiesNearUser(userId);
            return Ok(properties);
        }




    }


}
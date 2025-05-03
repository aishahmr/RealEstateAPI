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
using Property = RealEstateAPI.Models.Property; 

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
                Price = dto.Price,
                Location = dto.Location?.Trim(),
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
                Floor = dto.Floor
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
                    Title = property.Title,
                    Description = property.Description,
                    Price = property.Price,
                    Location = property.Location,
                    Bedrooms = property.Bedrooms,
                    Bathrooms = property.Bathrooms,
                    Floor = property.Floor,
                    Area = property.Size,
                    FurnishStatus = property.FurnishingStatus,
                    Amenities = property.Amenities?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                    Images = imageUrls,
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
                Price = propertyDto.Price,
                Location = propertyDto.Location?.Trim(),
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
                    Price = property.Price,
                    Location = property.Location,
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
        public async Task<IActionResult> UpdateProperty(Guid id, [FromBody] UpdatePropertyDTO updateDto)
        {
            // 1. Get current user info - multiple fallback options
            var currentUserId = User.FindFirst("userId")?.Value
                             ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("sub")?.Value; // email fallback

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new { Message = "User identification not found in token" });
            }

            // 2. Verify ID match
            if (id != updateDto.Id)
            {
                return BadRequest(new { Message = "Path ID does not match body ID" });
            }

            // 3. Attempt update
            var result = await _propertyService.UpdatePropertyAsync(updateDto, currentUserId);
            if (!result)
            {
                // Debug ownership issues
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
                    .FirstOrDefaultAsync() ?? new
                    {
                        PropertyExists = false,
                        CurrentUserEmail = currentUserId,
                        PropertyOwnerId = (string)null,
                        OwnerEmail = (string)null,
                        VerificationStatus = (string)null
                    };

                return StatusCode(403, new
                {
                    Message = "Update failed - ownership or validation issue",
                    DebugInfo = debugInfo



                });
            }

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
                Price = property.Price,
                Location = property.Location,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                Area = property.Size,
                FurnishStatus = property.FurnishingStatus,
                Amenities = property.Amenities?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                           ?? new List<string>(),
                Images = property.Images.Select(i => i.Url).ToList(),
                CreatedAt = property.CreatedAt,
                Floor = property.Floor,
                Type = property.Type,
                // Computed fields

            };
        }



    }


}
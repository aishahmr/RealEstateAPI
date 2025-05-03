using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.DTOs.UserDTos.Profile;
using RealEstateAPI.Models.Data;
using RealEstateAPI.Models;
using RealEstateAPI.Service.IServices;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("GetUserDetails")]
        [Authorize]
        public async Task<IActionResult> GetUserDetails([FromServices] AppDbContext context)
        {
            var userId = User.FindFirst("userId")?.Value?.ToLowerInvariant()
                       ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value?.ToLowerInvariant();

            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID not found in token");

            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Id.ToLower() == userId);

            if (user == null)
            {
                _logger.LogError("User not found. Requested ID: {UserId} (Normalized: {NormalizedId})",
                    userId, userId.ToLower());
                return NotFound("User not found");
            }

            // to Map to DTO before returning
            var userProfile = new UserProfileDTO
            {
                Id = user.Id,
                UserName = user.UserName, 
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                Gender = user.Gender,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            return Ok(userProfile);
        }


        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(
     [FromBody] UpdateUserProfileDTO dto,
     [FromServices] AppDbContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst("userId")?.Value?.ToLowerInvariant()
                       ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value?.ToLowerInvariant();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            bool userExists = await context.Users
                .AnyAsync(u => u.Id.ToLower() == userId);

            if (!userExists)
                return NotFound("User not found");

            var updated = await _userService.UpdateUserProfile(userId, dto, _logger);
            return updated ? Ok("Updated") : BadRequest("Update failed");
        }

        [HttpPost("profile/upload-picture")]
        [Authorize]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5 * 1024 * 1024)]

        public async Task<IActionResult> UploadProfilePicture([FromForm] UploadProfilePictureDTO dto)
        {
            var file = dto.File;

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type. Only images are allowed.");

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("File size exceeds 5MB limit.");

            var userId = User.FindFirst("userId")?.Value?.ToLowerInvariant()
                       ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value?.ToLowerInvariant();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            try
            {
                var imageUrl = await _userService.UploadProfilePicture(userId, file);

                if (string.IsNullOrEmpty(imageUrl))
                    return StatusCode(500, "Failed to process upload.");

                return Ok(new
                {
                    imageUrl,
                    message = "Upload successful"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture for user {UserId}", userId);
                return StatusCode(500, "An error occurred during upload.");
            }
        }

    }
}
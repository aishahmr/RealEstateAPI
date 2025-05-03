using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.DTOs.FavoriteDTO;
using RealEstateAPI.Service.IServices;
using System;
using System.Threading.Tasks;

namespace RealEstateAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        // POST: api/favorites
        [HttpPost]
        public async Task<IActionResult> AddToFavorites([FromBody] FavoriteDTO favoriteDto)
        {
            if (favoriteDto == null || string.IsNullOrEmpty(favoriteDto.UserId) || favoriteDto.PropertyId == Guid.Empty)
            {
                return BadRequest("Invalid favorite data.");
            }

            var result = await _favoriteService.AddToFavorites(favoriteDto.UserId, favoriteDto.PropertyId);
            if (!result)
            {
                return Conflict("The property is already in the user's favorites.");
            }

            return Ok("Favorite added successfully.");
        }


        // GET: api/favorites/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserFavorites(string userId)
        {
            // Return empty list if userId is invalid (optional, depending on requirements)
            if (string.IsNullOrEmpty(userId))
            {
                return Ok(new List<FavoritePropertyDTO>()); // Return empty list instead of BadRequest
            }

            var favorites = await _favoriteService.GetUserFavorites(userId);

            return Ok(favorites ?? new List<FavoritePropertyDTO>()); // Return empty list if null
        }


        // DELETE: api/favorites/{userId}/{propertyId}
        [HttpDelete("{userId}/{propertyId}")]
        public async Task<IActionResult> DeleteFavorite(string userId, Guid propertyId)
        {
            if (string.IsNullOrEmpty(userId) || propertyId == Guid.Empty)
            {
                return BadRequest("Invalid user ID or property ID.");
            }

            var result = await _favoriteService.RemoveFromFavorites(userId, propertyId);
            if (!result)
            {
                return NotFound("Favorite not found or could not be deleted.");
            }

            return Ok("Favorite deleted successfully.");
        }
    }
}
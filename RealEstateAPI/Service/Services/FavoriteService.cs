using RealEstateAPI.Models;
using RealEstateAPI.Models.Data;
using RealEstateAPI.Service.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealEstateAPI.DTOs.FavoriteDTO;


namespace RealEstateAPI.Service.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly AppDbContext _context;

        public FavoriteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FavoritePropertyDTO>> GetUserFavorites(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new List<FavoritePropertyDTO>(); // Early return empty list
            }

            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Property)
                    .ThenInclude(p => p.Images)
                .Select(f => new FavoritePropertyDTO
                {
                    UserId = f.UserId,
                    PropertyId = f.PropertyId,
                    Title = f.Property.Title.Length > 20
                        ? f.Property.Title.Substring(0, 17) + "..."
                        : f.Property.Title,
                    Address = $"{f.Property.AddressLine1}, {f.Property.City}, {f.Property.Governorate}",
                    Price = f.Property.Price,
                    FormattedPrice = f.Property.Price.ToString("C0"),  // "C0" = currency with no decimals
                    Area = f.Property.Size,
                    MainImageUrl = f.Property.Images.Any()
                        ? f.Property.Images.First().Url
                        : "/images/default-property.jpg"
                })
                .ToListAsync() ?? new List<FavoritePropertyDTO>(); // Fallback to empty list
        }

        public async Task<bool> AddToFavorites(string userId, Guid propertyId)
        {
            if (await _context.Favorites.AnyAsync(f => f.UserId == userId && f.PropertyId == propertyId))
                return false;

            var favorite = new Favorite
            {
                UserId = userId,
                PropertyId = propertyId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromFavorites(string userId, Guid propertyId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PropertyId == propertyId);

            if (favorite == null)
                return false;

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}


using RealEstateAPI.Models;
using RealEstateAPI.Models.Data;
using RealEstateAPI.Repository.GenericRepo;
using RealEstateAPI.Repository.IRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstateAPI.Repository.Repo
{
    public class FavoriteRepository : GenericRepositoryAsync<Favorite>, IFavoriteRepository
    {
        private readonly DbSet<Favorite> _favorites;

        public FavoriteRepository(AppDbContext db) : base(db)
        {
            _favorites = db.Set<Favorite>();
        }

        public async Task<List<Favorite>> GetFavoritesByUserIdAsync(Guid userId)
        {
            return await _favorites.Where(f => f.UserId == userId.ToString()).ToListAsync();
        }


    }
}

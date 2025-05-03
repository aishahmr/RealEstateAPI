using RealEstateAPI.Models;
using RealEstateAPI.Models.Data;
using RealEstateAPI.Repository.GenericRepo;
using RealEstateAPI.Repository.IRepo;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstateAPI.Repository.Repo
{
    public class PropertyRepository : GenericRepositoryAsync<Property>, IPropertyRepository
    {
        private readonly DbSet<Property> _properties;

        public PropertyRepository(AppDbContext db) : base(db)
        {
            _properties = db.Set<Property>();
        }

        public async Task<List<Property>> GetPropertiesByUserIdAsync(string userId)
        {
            // Directly compare the string userId with Property.UserId
            return await _properties.Where(p => p.UserId == userId).ToListAsync();
        }
    }
}

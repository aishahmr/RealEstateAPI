using RealEstateAPI.Models;
using RealEstateAPI.Repository.GenericRepo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstateAPI.Repository.IRepo
{
    public interface IPropertyRepository : IGenericRepositoryAsync<Property>
    {
        // Add property-specific methods if needed
        Task<List<Property>> GetPropertiesByUserIdAsync(string userId);
    }
}

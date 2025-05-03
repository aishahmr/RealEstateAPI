using RealEstateAPI.Models;
using RealEstateAPI.Repository.GenericRepo;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace RealEstateAPI.Repository.IRepo
{
    public interface IFavoriteRepository : IGenericRepositoryAsync<Favorite>
    {
        Task<List<Favorite>> GetFavoritesByUserIdAsync(Guid userId);
    }
}


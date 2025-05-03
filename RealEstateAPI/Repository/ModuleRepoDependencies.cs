using Microsoft.Extensions.DependencyInjection;
using RealEstateAPI.Repository.IRepo;
using RealEstateAPI.Repository.Repo;

namespace RealEstateAPI.Repository
{
    public static class ModuleRepoDependencies
    {
        public static IServiceCollection AddRepoDependencies(this IServiceCollection services)
        {
            services.AddTransient<IPropertyRepository, PropertyRepository>();
            services.AddTransient<IFavoriteRepository, FavoriteRepository>();
            // Remove registrations for IRequestDriveRepository and ITripRepository
            return services;
        }
    }
}

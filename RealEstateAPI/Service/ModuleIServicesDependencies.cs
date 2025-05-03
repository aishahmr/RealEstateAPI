using Microsoft.Extensions.DependencyInjection;
using RealEstateAPI.Service.IServices;
using RealEstateAPI.Service.Services;


namespace RealEstateAPI.Service
{
    public static class ModuleIServicesDependencies
    {
        public static IServiceCollection AddServicesDependencies(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IFavoriteService, FavoriteService>();

            return services;
        }
    }
}


using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstateAPI.Helpers
{
    public static class DataSeeder
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Ensure roleManager is not null
                if (roleManager == null)
                {
                    throw new Exception("RoleManager is not initialized.");
                }

                List<string> rolesList = new List<string> { "Admin", "User" }; // Add necessary roles here

                // Ensure rolesList is not empty
                if (rolesList == null || rolesList.Count == 0)
                {
                    throw new Exception("Roles list is empty or null.");
                }

                foreach (var role in rolesList)
                {
                    var existingRole = await roleManager.FindByNameAsync(role);
                    if (existingRole == null)
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }
        }
    }
}

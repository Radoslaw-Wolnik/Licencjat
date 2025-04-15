// Backend.Infrastructure/Seeders/InfrastructureSeeder.cs
using Microsoft.AspNetCore.Identity;
namespace Backend.Infrastructure.Data.Seeders;

public static class InfrastructureSeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        var roles = new[] { "Admin", "User" };
        
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}
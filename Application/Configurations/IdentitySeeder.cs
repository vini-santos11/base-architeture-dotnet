using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Role = Domain.Entities.Role;

namespace Application.Configurations;

public static class IdentitySeeder
{
    public static async Task SeedRolesAndClaims(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        var roles = new List<string>
        {
            "Administrator",
            "User"
        };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new Role()
                {
                    Name = role,
                    CreatedAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                });
            }
        }
    }
}
using Application.Configurations.Interfaces;
using Domain.Entities;
using Infra.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Configurations;

public static class DatabaseSetup
{   
    public static void AddDatabaseSetup(this IServiceCollection services, IDatabaseConfiguration configuration)
    {
        if(services is null) throw new ArgumentNullException(nameof(services));
        
        services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.RequireUniqueEmail = true;
        });

        services.AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<BaseContext>()
            .AddDefaultTokenProviders();
        
        services.AddDbContext<BaseContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString()));
    }
}
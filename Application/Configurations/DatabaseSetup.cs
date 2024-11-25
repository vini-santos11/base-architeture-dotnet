using Application.Configurations.Interfaces;
using Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Configurations;

public static class DatabaseSetup
{   
    public static void AddDatabaseSetup(this IServiceCollection services, IDatabaseConfiguration configuration)
    {
        if(services is null) throw new ArgumentNullException(nameof(services));
        
        services.AddDbContextPool<BaseContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString()));
    }
}
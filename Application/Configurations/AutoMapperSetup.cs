using Application.AutoMapper;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Configurations;

public static class AutoMapperSetup
{
    public static void AddAutoMapperSetup(this IServiceCollection services)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        
        services.AddSingleton<IConfigurationProvider>(AutoMapperConfig.RegisterMappings());
        services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
    }
}
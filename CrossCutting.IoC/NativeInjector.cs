using Application.Interfaces;
using Application.Services;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Services;
using Infra.Context;
using Infra.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.IoC;

public static class NativeInjector
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IAuthenticationAppService, AuthenticationAppService>();
        services.AddScoped<IClaimAppService, ClaimAppService>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserAppService, UserAppService>();

        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IRoleAppService, RoleAppService>();

        services.AddScoped<IRoleClaimService, RoleClaimService>();
        services.AddScoped<IRoleClaimRepository, RoleClaimRepository>();
        
        //configurations
        services.AddTransient<BaseContext>();
    }
}
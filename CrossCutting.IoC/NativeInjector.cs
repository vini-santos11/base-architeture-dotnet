using Application.Interfaces;
using Application.Services;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Services;
using Infra.Context;
using Infra.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.IoC;

public static class NativeInjector
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IAuthenticationAppService, AuthenticationAppService>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserAppService, UserAppService>();
        
        //configurations
        services.AddTransient<BaseContext>();
    }
}
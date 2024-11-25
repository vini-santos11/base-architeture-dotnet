using CrossCutting.IoC;

namespace API.Helpers;

public static class DependencyInjector
{
    public static void AddDependencyInjector(this IServiceCollection services)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        services.AddSingleton<ILoggerFactory, LoggerFactory>();

        NativeInjector.RegisterServices(services);
    }
}
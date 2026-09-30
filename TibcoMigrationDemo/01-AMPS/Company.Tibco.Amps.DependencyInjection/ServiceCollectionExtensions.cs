using Company.Tibco.Amps.Abstractions;
using Company.Tibco.Amps.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Company.Tibco.Amps.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTibcoAmps(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AmpsOptions>(configuration.GetSection("Amps"));
        //services.AddSingleton<IAmpsMessaging, AmpsMessaging>();
        services.AddSingleton<IAmpsConnection, AmpsConnection>();
        services.AddSingleton<IAmpsPublisher, AmpsPublisher>();
        services.AddSingleton<IAmpsSubscriber, AmpsSubscriber>();
        return services;
    }
}

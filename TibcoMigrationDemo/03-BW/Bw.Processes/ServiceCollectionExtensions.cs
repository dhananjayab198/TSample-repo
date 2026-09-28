using Company.Tibco.Adb.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Company.Tibco.Bw.Processes;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTibcoBwProcesses(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTibcoAdb(configuration);
        services.AddScoped<ICustomerProcess, CustomerProcess>();
        services.AddScoped<OrderProcess>();
        return services;
    }
}

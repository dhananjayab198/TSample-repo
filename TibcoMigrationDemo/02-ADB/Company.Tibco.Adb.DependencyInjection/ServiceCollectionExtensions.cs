using Company.Tibco.Adb.Abstractions;
using Company.Tibco.Adb.Database;
using Company.Tibco.Adb.Messaging;
using Company.Tibco.Adb.Processing;
using Company.Tibco.Amps.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Company.Tibco.Adb.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTibcoAdb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTibcoAmps(configuration);
        var connection = configuration.GetConnectionString("AdbDb") ?? "Data Source=adb-demo.db";
        // Choose provider based on connection string. Default connection points to a local sqlite file.
        //if (connection.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase) || connection.EndsWith(".db", StringComparison.OrdinalIgnoreCase))
        //{
        //    services.AddDbContextFactory<Entities>(o => o.UseSqlite(connection));
        //}
        //else
        //{
            services.AddDbContextFactory<Entities>(o => o.UseSqlServer(connection));
        //}
        services.AddScoped<ISourceEventPublisher, SourceEventPublisher>();
        services.AddScoped<IAdbProcessor, AdbProcessor>();
        services.AddScoped(typeof(ICrudRepository<>), typeof(EfCrudRepository<>));

        return services;
    }
}

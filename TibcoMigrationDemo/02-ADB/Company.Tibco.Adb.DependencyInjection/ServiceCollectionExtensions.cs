using Company.Tibco.Adb.Abstractions;
using Company.Tibco.Adb.Database;
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
        services.AddDbContext<AdbDbContext>(o => o.UseSqlite(connection));
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IAdbEventPublisher, AmpsAdbEventPublisher>();
        return services;
    }
}

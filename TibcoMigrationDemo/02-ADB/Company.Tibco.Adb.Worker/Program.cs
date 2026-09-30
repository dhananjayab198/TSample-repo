
using Company.Tibco.Adb.DependencyInjection;
using Company.Tibco.Adb.Processing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTibcoAdb(builder.Configuration);
builder.Services.AddHostedService<AdbWorker>();
await builder.Build().RunAsync();

public sealed class AdbWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IConfiguration _config;

    public AdbWorker(IServiceProvider services, IConfiguration config)
    {
        _services = services; _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delay = _config.GetValue("Adb:PollingIntervalSeconds", 1);

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IAdbProcessor>();
            await processor.ProcessAllAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(delay), stoppingToken);
        }
    }
}

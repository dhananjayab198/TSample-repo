using Company.Tibco.Amps.Abstractions;
using Company.Tibco.Amps.DependencyInjection;
using Company.Tibco.Bw.Processes;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTibcoAmps(builder.Configuration);
builder.Services.AddSingleton<OrderProcess>();
builder.Services.AddHostedService<BwWorker>();

await builder.Build().RunAsync();

public sealed class BwWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IAmpsSubscriber _subscriber;

    public BwWorker(IServiceProvider services, IAmpsSubscriber subscriber)
    {
        _services = services; _subscriber = subscriber;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return _subscriber.SubscribeAsync(
            AmpsDestinationType.Topic,
            "ORDER.CREATED",
            async (message, ct) =>
            {
                using var scope = _services.CreateScope();
                var process = scope.ServiceProvider.GetRequiredService<OrderProcess>();
                await process.ExecuteAsync(message, ct);
            },
            stoppingToken);
    }
}

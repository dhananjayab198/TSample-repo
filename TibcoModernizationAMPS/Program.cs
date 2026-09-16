using InboundWorker;
using OutboundWorker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // 1. Register universal dynamic table DML execution strategy
        services.AddSingleton<IDynamicTableHandler, GenericDynamicHandler>();

        // 2. Register AMPS-backed Workers (Replacing TIBCO ADB Adapter + TIBCO EMS Server)
        services.AddHostedService<AmpsOutboundPoller>();
        services.AddHostedService<AmpsInboundConsumer>();
    })
    .Build();

await host.RunAsync();
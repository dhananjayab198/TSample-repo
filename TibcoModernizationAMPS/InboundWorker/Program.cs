using InboundWorker;
using OutboundWorker;

// Build and run the generic host configuration for the worker services
var builder = Host.CreateApplicationBuilder(args);

// 1. Register the universal dynamic table handler as a singleton for dependency injection
builder.Services.AddSingleton<IDynamicTableHandler, GenericDynamicHandler>();

// 2. Register AMPS-backed background worker services 
// (These replace the legacy TIBCO ADB Adapters and TIBCO EMS server)
builder.Services.AddHostedService<AmpsInboundConsumer>();

var host = builder.Build();
host.Run();

using OutboundWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<AmpsOutboundPoller>();

var host = builder.Build();
host.Run();


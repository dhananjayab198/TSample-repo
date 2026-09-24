using Company.Tibco.Amps.Abstractions;
using Company.Tibco.Amps.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTibcoAmps(builder.Configuration);
using var host = builder.Build();
var amps = host.Services.GetRequiredService<IAmpsMessaging>();

await amps.PublishAsync("CUSTOMER.EVENT", "{\"id\":1,\"event\":\"created\"}");
Console.WriteLine("Published CUSTOMER.EVENT. Configure AMPS in appsettings.json before running against a server.");

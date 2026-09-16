using System.Text.Json;
using AMPS.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedContracts;

namespace InboundWorker;

/// <summary>
/// Background worker service that connects to the 60East AMPS broker, 
/// subscribes to wildcard topics, and delegates incoming payloads to the dynamic handler.
/// </summary>
public class AmpsInboundConsumer : BackgroundService
{
    private readonly ILogger<AmpsInboundConsumer> _logger;
    private readonly IDynamicTableHandler _dynamicHandler;
    private Client? _ampsClient;
    // Connection URI for the 60East AMPS messaging engine
    private readonly string _ampsUri = "tcp://127.0.0.1:9007/amps/json";

    public AmpsInboundConsumer(ILogger<AmpsInboundConsumer> logger, IDynamicTableHandler dynamicHandler)
    {
        _logger = logger;
        _dynamicHandler = dynamicHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Initializing 60East AMPS Inbound Consumer...");

        try
        {
            // Connect and authenticate with the AMPS server
            _ampsClient = new Client("DynamicInboundConsumer");
            await Task.Run(() => _ampsClient.connect(_ampsUri), stoppingToken);
            _ampsClient.logon();

            _logger.LogInformation("Subscribing to AMPS wildcard topic stream: db-cdc-*");

            // Use an AMPS wildcard subscription (`db-cdc-*`) to automatically listen to 
            // changes across all database tables without needing explicit individual subscriptions.
            var messages = _ampsClient.subscribe("db-cdc-*");

            foreach (var message in messages)
            {
                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    // Extract and deserialize the incoming message data into our shared contract
                    string jsonString = message.Data;
                    var changeEvent = JsonSerializer.Deserialize<DatabaseChangeEvent>(jsonString);

                    if (changeEvent != null)
                    {
                        _logger.LogInformation("[AMPS INBOUND] Intercepted stream for Table: {Table}", changeEvent.TableName);

                        // Route payload directly into the metadata-driven dynamic table handler
                        await _dynamicHandler.HandleDynamicEventAsync(
                            changeEvent.TableName,
                            changeEvent.OperationType,
                            changeEvent.PayloadJson);
                    }
                } 
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing dynamic handler on incoming AMPS message payload.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AMPS consumer connection or subscription stream failure.");
        }
    }
}
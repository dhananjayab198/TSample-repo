using AMPS.Client;
using SharedContracts;
using System.Text.Json;

namespace OutboundWorker;

/// <summary>
/// Background worker service that polls the source database (simulated here) 
/// and publishes multi-table change events directly to the 60East AMPS message broker.
/// </summary>
public class AmpsOutboundPoller : BackgroundService
{
    private readonly ILogger<AmpsOutboundPoller> _logger;
    private Client? _ampsClient;
    // Connection URI for the 60East AMPS messaging engine (JSON transport protocol)
    private readonly string _ampsUri = "tcp://127.0.0.1:9007/amps/json";

    // Simulates continuous mutations happening across ANY distinct table in the source DB
    private readonly List<(string Table, string Op)> _simulatedDbMutations = new()
    {
        ("Orders", "INSERT"),
        ("Customers", "UPDATE"),
        ("Inventory", "DELETE"),
        ("Products", "INSERT"),
        ("Shipments", "UPDATE")
    }; 

    public AmpsOutboundPoller(ILogger<AmpsOutboundPoller> logger)
    {
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Initializing 60East AMPS Outbound Poller...");

        try
        {
            // Instantiate and connect the official AMPS client SDK
            _ampsClient = new Client("DynamicOutboundProducer");
            await Task.Run(() => _ampsClient.connect(_ampsUri), stoppingToken);
            _ampsClient.logon();
            _logger.LogInformation("Connected and logged into AMPS server successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to AMPS engine. Ensure AMPS server is active.");
            return;
        }

        int index = 0;
        // Loop continuously to simulate streaming database change data capture (CDC)
        while (!stoppingToken.IsCancellationRequested)
        {
            var mutation = _simulatedDbMutations[index % _simulatedDbMutations.Count];
            index++;

            // Create a standardized metadata change event envelope
            var changeEvent = new DatabaseChangeEvent
            {
                TableName = mutation.Table,
                OperationType = mutation.Op,
                PayloadJson = JsonSerializer.Serialize(new { PrimaryKeyId = new Random().Next(1000, 9999), UpdatedDate = DateTime.UtcNow })
            };

            // Dynamically construct a table-specific topic name (e.g., db-cdc-orders)
            string topicName = $"db-cdc-{mutation.Table.ToLowerInvariant()}";
            string payload = JsonSerializer.Serialize(changeEvent);

            try
            {
                // Publish structured row change event to AMPS topic
                _ampsClient.publish(topicName, payload);
                _logger.LogInformation("[AMPS OUTBOUND] Published Topic: {Topic} | Table: {Table} | Op: {Op}",
                    topicName, changeEvent.TableName, changeEvent.OperationType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message to AMPS topic {Topic}", topicName);
            }

            await Task.Delay(4000, stoppingToken);
        }
    }
     
}

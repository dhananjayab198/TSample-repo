using AMPS.Client;
using Company.Tibco.Amps.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; 

namespace Company.Tibco.Amps.Client;

public sealed class AmpsPublisher : IAmpsPublisher
{
    private readonly IAmpsConnection _connection;
    private readonly ILogger<AmpsMessaging> _logger;
    private readonly HAClient _client;
    private readonly AmpsOptions _options;

    public AmpsPublisher(IAmpsConnection connection, IOptions<AmpsOptions> options, ILogger<AmpsMessaging> logger) { 
        _connection = connection;
        _options = options.Value;
        _logger = logger;
    }

    public async Task PublishAsync(
        AmpsDestinationType destinationType,
        string destination,
        AmpsMessage message,
        CancellationToken cancellationToken = default)
    {
        if (!_connection.IsConnected)
            await _connection.ConnectAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        _client.publish(destinationType.ToString(), message.ToString());
        await Task.CompletedTask;
    }
}

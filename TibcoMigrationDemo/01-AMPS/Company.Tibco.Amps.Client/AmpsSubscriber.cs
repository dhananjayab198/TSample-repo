using AMPS.Client;
using Company.Tibco.Amps.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Company.Tibco.Amps.Client;

public sealed class AmpsSubscriber : IAmpsSubscriber
{
    private readonly IAmpsConnection _connection;
    private readonly ILogger<AmpsMessaging> _logger;
    private readonly HAClient _client;
    private readonly AmpsOptions _options;

    public AmpsSubscriber(IAmpsConnection connection, IOptions<AmpsOptions> options, ILogger<AmpsMessaging> logger)
    {
        _connection = connection;
        _options = options.Value;
        _logger = logger;
    }

    public async IAsyncEnumerable<AmpsMessage> SubscribeAsync(
        AmpsDestinationType destinationType,
        string destination,
        Func<AmpsMessage, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default)
    {
        if (!_connection.IsConnected)
            await _connection.ConnectAsync(cancellationToken);

        var stream = handler != null
        ? _client.subscribe(handler.ToString(), destination)
        : _client.subscribe(destination, null); // Adjusted to use 'destination' instead of undefined 'topic/filter'

        foreach (var message in stream)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new AmpsMessage(destination, message.getData(), message.getCommand().ToString(), message.getBookmark());
        }
        //await Task.CompletedTask;
    }
}

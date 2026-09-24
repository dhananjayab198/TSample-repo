using System.Runtime.CompilerServices; 
using Company.Tibco.Amps.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AMPS.Client;

namespace Company.Tibco.Amps.Client;

public sealed class AmpsMessaging : IAmpsMessaging, IDisposable
{
    private readonly AmpsOptions _options;
    private readonly ILogger<AmpsMessaging> _logger;
    private readonly HAClient _client;
    private bool _connected;

    public AmpsMessaging(IOptions<AmpsOptions> options, ILogger<AmpsMessaging> logger)
    {
        _options = options.Value;
        _logger = logger;
        _client = new HAClient(_options.ClientName);
    }

    public Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_connected) return Task.CompletedTask;
        _client.connect(_options.ServerUrl);
        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            //_client.logon(_options.Username , _options.Password ?? string.Empty);
        }
        else
            _client.logon();
        _connected = true;
        _logger.LogInformation("Connected to AMPS {Server}", _options.ServerUrl);
        return Task.CompletedTask;
    }

    public Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (_connected)
        {
            _client.disconnect();
            _connected = false;
        }
        return Task.CompletedTask;
    }

    public async Task PublishAsync(string topic, string data, CancellationToken cancellationToken = default)
    {
        await ConnectAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        _client.publish(topic, data);
    }

    public async Task DeleteAsync(string topic, string key, CancellationToken cancellationToken = default)
    {
        await ConnectAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        _client.sowDelete(topic, key);
        //_client.sowd(topic, key);
    }

    public async IAsyncEnumerable<AmpsMessage> SubscribeAsync(
        string topic,
        string? filter = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await ConnectAsync(cancellationToken);
        var stream = string.IsNullOrWhiteSpace(filter)
            ? _client.subscribe(topic)
            : _client.subscribe(topic, filter);

        foreach (var message in stream)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new AmpsMessage(topic, message.getData(), message.getCommand().ToString(), message.getBookmark());
        }
    }

    public async IAsyncEnumerable<AmpsMessage> QueryStateOfWorldAsync(
        string topic,
        string? filter = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await ConnectAsync(cancellationToken);
        var stream = string.IsNullOrWhiteSpace(filter)
            ? _client.sow(topic)
            : _client.sow(topic, filter);

        foreach (var message in stream)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new AmpsMessage(topic, message.getData(), message.getCommand().ToString(), message.getBookmark());
        }
    }

    public void Dispose()
    {
        if (_connected) _client.disconnect();
        _client.Dispose();
    }
}

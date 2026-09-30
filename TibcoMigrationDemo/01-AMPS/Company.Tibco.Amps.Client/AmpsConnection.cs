using AMPS.Client;
using Company.Tibco.Amps.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; 

namespace Company.Tibco.Amps.Client;

public sealed class AmpsConnection : IAmpsConnection
{
    private readonly AmpsOptions _options;
    private readonly ILogger<AmpsMessaging> _logger;
    private readonly HAClient _client;
    public bool IsConnected { get; private set; }

    public AmpsConnection(IOptions<AmpsOptions> options, ILogger<AmpsMessaging> logger)
    { 
        _options = options.Value;
        _logger = logger;
        _client = new HAClient(_options.ClientName);
    }

    public Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (IsConnected) return Task.CompletedTask;
        _client.connect(_options.ServerUrl);
        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            //_client.logon(_options.Username , _options.Password ?? string.Empty);
        }
        else
            _client.logon(); 
        IsConnected = true;
        _logger.LogInformation("Connected to AMPS {Server}", _options.ServerUrl);
        return Task.CompletedTask;
    }

    public Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (IsConnected)
        {
            _client.disconnect();
            IsConnected = false;
        }
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (IsConnected) _client.disconnect();
        _client.Dispose();
    }
}

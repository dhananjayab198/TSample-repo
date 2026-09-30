using System;
using System.Threading;
using System.Threading.Tasks;

namespace Company.Tibco.Amps.Abstractions;

public enum AmpsDestinationType { Topic, Queue }

public sealed record AmpsMessage(
    string MessageId,
    string? CorrelationId,
    string MessageType,
    string Payload);

public interface IAmpsConnection
{
    bool IsConnected { get; }
    Task ConnectAsync(CancellationToken cancellationToken = default);
    Task DisconnectAsync(CancellationToken cancellationToken = default);
    void Dispose();
}

public interface IAmpsPublisher
{
    Task PublishAsync(
        AmpsDestinationType destinationType,
        string destination,
        AmpsMessage message,
        CancellationToken cancellationToken = default);
}

public interface IAmpsSubscriber
{
    IAsyncEnumerable<AmpsMessage> SubscribeAsync(
        AmpsDestinationType destinationType,
        string destination,
        Func<AmpsMessage, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default);
}

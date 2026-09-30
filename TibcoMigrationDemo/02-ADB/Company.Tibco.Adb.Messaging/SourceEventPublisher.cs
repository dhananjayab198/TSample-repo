using Company.Tibco.Adb.Abstractions;
using Company.Tibco.Amps.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Company.Tibco.Adb.Messaging;

public interface ISourceEventPublisher
{
    Task PublishAsync(SourceEvent sourceEvent, CancellationToken ct);
}

public sealed class SourceEventPublisher : ISourceEventPublisher
{
    private readonly IAmpsPublisher _publisher;

    public SourceEventPublisher(IAmpsPublisher publisher) => _publisher = publisher;

    public Task PublishAsync(SourceEvent e, CancellationToken ct)
    {
        var type = e.DestinationType.Equals("QUEUE", StringComparison.OrdinalIgnoreCase)
            ? AmpsDestinationType.Queue
            : AmpsDestinationType.Topic;

        return _publisher.PublishAsync(
            type,
            e.DestinationName,
            new AmpsMessage(e.MessageId, e.CorrelationId, e.MessageType, e.Payload),
            ct);
    }
}

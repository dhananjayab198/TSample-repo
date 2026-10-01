 
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Company.Tibco.Adb.Abstractions;
using Company.Tibco.Adb.Database;
using Company.Tibco.Adb.Messaging;
using Company.Tibco.Amps.Abstractions;

namespace Company.Tibco.Bw.Processes;

public sealed class OrderProcess(ICrudRepository<Order> orders, ISourceEventPublisher events)
{
    public Task<Order?> GetAsync(long id, CancellationToken ct = default) => orders.GetAsync(id, ct);
    public Task<List<Order>> GetAllAsync(CancellationToken ct = default) => orders.GetAllAsync(ct);
    public async Task<Order> CreateAsync(Order order, CancellationToken ct = default)
    {
        var result = await orders.AddAsync(order, ct);
        var sourceEvent = new SourceEvent(
            Id: result.Id,
            SourceTable: "Order",
            SourceId: result.Id.ToString(),
            Operation: "CREATE",
            MessageType: "OrderCreated",
            DestinationType: "TOPIC",
            DestinationName: "BW.ORDER.CREATED",
            Payload: JsonSerializer.Serialize(result),
            MessageId: Guid.NewGuid().ToString(),
            CorrelationId: null,
            Status: ProcessingStatus.Published,
            RetryCount: 0
        );
        await events.PublishAsync(sourceEvent, ct);
        return result;
    }

    public async Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        await orders.UpdateAsync(order, ct);
        var sourceEvent = new SourceEvent(
            Id: order.Id,
            SourceTable: "Order",
            SourceId: order.Id.ToString(),
            Operation: "UPDATE",
            MessageType: "OrderUpdated",
            DestinationType: "TOPIC",
            DestinationName: "BW.ORDER.UPDATED",
            Payload: JsonSerializer.Serialize(order),
            MessageId: Guid.NewGuid().ToString(),
            CorrelationId: null,
            Status: ProcessingStatus.Published,
            RetryCount: 0
        );
        await events.PublishAsync(sourceEvent, ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        await orders.DeleteAsync(id, ct);
        var sourceEvent = new SourceEvent(
            Id: id,
            SourceTable: "Order",
            SourceId: id.ToString(),
            Operation: "DELETE",
            MessageType: "OrderDeleted",
            DestinationType: "TOPIC",
            DestinationName: "BW.ORDER.DELETED",
            Payload: JsonSerializer.Serialize(new { Id = id }),
            MessageId: Guid.NewGuid().ToString(),
            CorrelationId: null,
            Status: ProcessingStatus.Published,
            RetryCount: 0
        );
        await events.PublishAsync(sourceEvent, ct);
    }

    public async Task ExecuteAsync(AmpsMessage message, CancellationToken ct)
    {
        if (message is null) return;

        // Try to deserialize payload into Order
        var order = JsonSerializer.Deserialize<Order>(message.Payload);
        if (order is null)
            return;

        var m = message.MessageType?.ToLowerInvariant();
        if (m == "created" || m == "create")
        {
            await CreateAsync(order, ct);
            return;
        }

        if (m == "updated" || m == "update")
        {
            await UpdateAsync(order, ct);
            return;
        }

        if (m == "deleted" || m == "delete")
        {
            await DeleteAsync(order.Id, ct);
            return;
        }

        // Default: try create
        await CreateAsync(order, ct);
    }
}

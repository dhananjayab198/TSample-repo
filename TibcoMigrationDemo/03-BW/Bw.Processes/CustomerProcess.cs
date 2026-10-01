 
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Company.Tibco.Adb.Abstractions;
using Company.Tibco.Adb.Database;
using Company.Tibco.Amps.Abstractions;
using Company.Tibco.Adb.Messaging;

namespace Company.Tibco.Bw.Processes;

public interface ICustomerProcess
{
    Task<Customer?> GetAsync(long id, CancellationToken ct = default);
    Task<List<Customer>> GetAllAsync(CancellationToken ct = default);
    Task<Customer> CreateAsync(Customer customer, CancellationToken ct = default);
    Task UpdateAsync(Customer customer, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}

public sealed class CustomerProcess(ICrudRepository<Customer> customers, ISourceEventPublisher events) : ICustomerProcess
{
    public Task<Customer?> GetAsync(long id, CancellationToken ct = default) => customers.GetAsync(id, ct);
    public Task<List<Customer>> GetAllAsync(CancellationToken ct = default) => customers.GetAllAsync(ct);
    public async Task<Customer> CreateAsync(Customer customer, CancellationToken ct = default)
    {
        var result = await customers.AddAsync(customer, ct);
        var sourceEvent = new SourceEvent(
            Id: result.Id,
            SourceTable: "Customer",
            SourceId: result.Id.ToString(),
            Operation: "CREATE",
            MessageType: "CustomerCreated",
            DestinationType: "TOPIC",
            DestinationName: "BW.CUSTOMER.CREATED",
            Payload: JsonSerializer.Serialize(result),
            MessageId: Guid.NewGuid().ToString(),
            CorrelationId: null,
            Status: ProcessingStatus.Published,
            RetryCount: 0
        );
        await events.PublishAsync(sourceEvent, ct);
        return result;
    }
    public async Task UpdateAsync(Customer customer, CancellationToken ct = default)
    {
        await customers.UpdateAsync(customer, ct);
        var sourceEvent = new SourceEvent(
            Id: customer.Id,
            SourceTable: "Customer",
            SourceId: customer.Id.ToString(),
            Operation: "UPDATE",
            MessageType: "CustomerUpdated",
            DestinationType: "TOPIC",
            DestinationName: "BW.CUSTOMER.UPDATED",
            Payload: JsonSerializer.Serialize(customer),
            MessageId: Guid.NewGuid().ToString(),
            CorrelationId: null,
            Status: ProcessingStatus.Published,
            RetryCount: 0
        );
        await events.PublishAsync(sourceEvent, ct);
    }
    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        await customers.DeleteAsync(id, ct);
        var sourceEvent = new SourceEvent(
            Id: id,
            SourceTable: "Customer",
            SourceId: id.ToString(),
            Operation: "DELETE",
            MessageType: "CustomerDeleted",
            DestinationType: "TOPIC",
            DestinationName: "BW.CUSTOMER.DELETED",
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

        var customer = JsonSerializer.Deserialize<Customer>(message.Payload);
        if (customer is null) return;

        var m = message.MessageType?.ToLowerInvariant();
        if (m == "created" || m == "create")
        {
            await CreateAsync(customer, ct);
            return;
        }

        if (m == "updated" || m == "update")
        {
            await UpdateAsync(customer, ct);
            return;
        }

        if (m == "deleted" || m == "delete")
        {
            await DeleteAsync(customer.Id, ct);
            return;
        }

        await CreateAsync(customer, ct);
    }
}

using Company.Tibco.Adb.Core;
using Company.Tibco.Adb.Database;
using Company.Tibco.Adb.Abstractions;

namespace Company.Tibco.Bw.Processes;

public interface ICustomerProcess
{
    Task<Customer?> GetAsync(int id, CancellationToken ct = default);
    Task<List<Customer>> GetAllAsync(CancellationToken ct = default);
    Task<Customer> CreateAsync(Customer customer, CancellationToken ct = default);
    Task UpdateAsync(Customer customer, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

public sealed class CustomerProcess(ICustomerRepository customers, IAdbEventPublisher events) : ICustomerProcess
{
    public Task<Customer?> GetAsync(int id, CancellationToken ct = default) => customers.GetAsync(id, ct);
    public Task<List<Customer>> GetAllAsync(CancellationToken ct = default) => customers.GetAllAsync(ct);
    public async Task<Customer> CreateAsync(Customer customer, CancellationToken ct = default)
    {
        var result = await customers.AddAsync(customer, ct);
        await events.PublishAsync("BW.CUSTOMER.CREATED", result.Id.ToString(), result, ct);
        return result;
    }
    public async Task UpdateAsync(Customer customer, CancellationToken ct = default)
    {
        await customers.UpdateAsync(customer, ct);
        await events.PublishAsync("BW.CUSTOMER.UPDATED", customer.Id.ToString(), customer, ct);
    }
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await customers.DeleteAsync(id, ct);
        await events.PublishAsync("BW.CUSTOMER.DELETED", id.ToString(), new { Id = id }, ct);
    }
}

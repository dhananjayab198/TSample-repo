using Company.Tibco.Adb.Abstractions;
using Company.Tibco.Adb.Core;
using Company.Tibco.Adb.Database;

namespace Company.Tibco.Bw.Processes;

public sealed class OrderProcess(IOrderRepository orders, IAdbEventPublisher events)
{
    public Task<Order?> GetAsync(int id, CancellationToken ct = default) => orders.GetAsync(id, ct);
    public Task<List<Order>> GetAllAsync(CancellationToken ct = default) => orders.GetAllAsync(ct);
    public async Task<Order> CreateAsync(Order order, CancellationToken ct = default)
    {
        if (order.TotalAmount < 0) throw new ArgumentException("Total amount cannot be negative.");
        var result = await orders.AddAsync(order, ct);
        await events.PublishAsync("BW.ORDER.CREATED", result.Id.ToString(), result, ct);
        return result;
    }
    public async Task UpdateAsync(Order order, CancellationToken ct = default) { await orders.UpdateAsync(order, ct); await events.PublishAsync("BW.ORDER.UPDATED", order.Id.ToString(), order, ct); }
    public async Task DeleteAsync(int id, CancellationToken ct = default) { await orders.DeleteAsync(id, ct); await events.PublishAsync("BW.ORDER.DELETED", id.ToString(), new { Id = id }, ct); }
}

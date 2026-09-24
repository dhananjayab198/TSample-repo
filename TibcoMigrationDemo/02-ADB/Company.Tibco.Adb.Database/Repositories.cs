using Company.Tibco.Adb.Abstractions;
using Company.Tibco.Adb.Core;
using Microsoft.EntityFrameworkCore;

namespace Company.Tibco.Adb.Database;

public class CrudRepository<T> : ICrudRepository<T, int> where T : class
{
    protected readonly AdbDbContext Db;
    protected DbSet<T> Set => Db.Set<T>();
    public CrudRepository(AdbDbContext db) => Db = db;
    public Task<T?> GetAsync(int id, CancellationToken ct = default) => Set.FindAsync([id], ct).AsTask();
    public Task<List<T>> GetAllAsync(CancellationToken ct = default) => Set.AsNoTracking().ToListAsync(ct);
    public async Task<T> AddAsync(T entity, CancellationToken ct = default) { await Set.AddAsync(entity, ct); await Db.SaveChangesAsync(ct); return entity; }
    public async Task UpdateAsync(T entity, CancellationToken ct = default) { Set.Update(entity); await Db.SaveChangesAsync(ct); }
    public async Task DeleteAsync(int id, CancellationToken ct = default) { var e = await Set.FindAsync([id], ct); if (e is not null) { Set.Remove(e); await Db.SaveChangesAsync(ct); } }
}

public interface ICustomerRepository : ICrudRepository<Customer, int> { }
public interface IAddressRepository : ICrudRepository<Address, int> { }
public interface IOrderRepository : ICrudRepository<Order, int> { }
public interface IOrderItemRepository : ICrudRepository<OrderItem, int> { }
public interface IPaymentRepository : ICrudRepository<Payment, int> { }

public sealed class CustomerRepository(AdbDbContext db) : CrudRepository<Customer>(db), ICustomerRepository { }
public sealed class AddressRepository(AdbDbContext db) : CrudRepository<Address>(db), IAddressRepository { }
public sealed class OrderRepository(AdbDbContext db) : CrudRepository<Order>(db), IOrderRepository { }
public sealed class OrderItemRepository(AdbDbContext db) : CrudRepository<OrderItem>(db), IOrderItemRepository { }
public sealed class PaymentRepository(AdbDbContext db) : CrudRepository<Payment>(db), IPaymentRepository { }

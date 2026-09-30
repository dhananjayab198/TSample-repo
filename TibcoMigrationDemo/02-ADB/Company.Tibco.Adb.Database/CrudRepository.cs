using Microsoft.EntityFrameworkCore;

namespace Company.Tibco.Adb.Database;

public interface ICrudRepository<T> where T : class
{
    Task<T?> GetAsync(long id, CancellationToken ct = default);
    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}

public class EfCrudRepository<T> : ICrudRepository<T> where T : class
{
    private readonly Entities _db;
    private readonly DbSet<T> _set;

    public EfCrudRepository(Entities db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public Task<T?> GetAsync(long id, CancellationToken ct = default) =>
        _set.FindAsync(new object?[] { id }, ct).AsTask();

    public Task<List<T>> GetAllAsync(CancellationToken ct = default) =>
        _set.AsNoTracking().ToListAsync(ct);

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        _set.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _set.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await GetAsync(id, ct);
        if (entity is null) return;
        _set.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}

namespace Company.Tibco.Adb.Abstractions;

public interface ICrudRepository<T, in TKey> where T : class
{
    Task<T?> GetAsync(TKey id, CancellationToken ct = default);
    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(TKey id, CancellationToken ct = default);
}

public interface IAdbUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public interface IAdbEventPublisher
{
    Task PublishAsync<T>(string eventType, string entityId, T data, CancellationToken ct = default);
}

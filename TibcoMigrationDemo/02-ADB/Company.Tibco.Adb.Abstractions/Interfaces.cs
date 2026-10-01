namespace Company.Tibco.Adb.Abstractions;

using System.Threading;
using System.Threading.Tasks;

public interface IAdbEventPublisher
{
    Task PublishAsync<T>(string eventType, string entityId, T data, CancellationToken ct = default);
}


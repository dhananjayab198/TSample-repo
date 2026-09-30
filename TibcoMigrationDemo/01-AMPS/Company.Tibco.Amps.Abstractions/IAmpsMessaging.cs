namespace Company.Tibco.Amps.Abstractions;

//public sealed record AmpsMessage(string Topic, string Data, string? Command = null, string? Bookmark = null);

//public interface IAmpsMessaging
//{
//    Task ConnectAsync(CancellationToken cancellationToken = default);
//    Task DisconnectAsync(CancellationToken cancellationToken = default);
//    Task PublishAsync(string topic, string data, CancellationToken cancellationToken = default);
//    Task DeleteAsync(string topic, string key, CancellationToken cancellationToken = default);
//    IAsyncEnumerable<AmpsMessage> SubscribeAsync(string topic, string? filter = null, CancellationToken cancellationToken = default);
//    IAsyncEnumerable<AmpsMessage> QueryStateOfWorldAsync(string topic, string? filter = null, CancellationToken cancellationToken = default);
//}

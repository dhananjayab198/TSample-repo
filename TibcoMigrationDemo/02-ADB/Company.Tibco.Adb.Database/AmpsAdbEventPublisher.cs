//using Company.Tibco.Adb.Abstractions;
//using Company.Tibco.Amps.Abstractions;
//using System.Text.Json;
////using Company.Tibco.Amps.Abstractions;

//namespace Company.Tibco.Adb.Database;

//public sealed class AmpsAdbEventPublisher(IAmpsMessage amps) : IAdbEventPublisher
//{
//    //IAmpsMessaging amps
//    public Task PublishAsync<T>(string eventType, string entityId, T data, CancellationToken ct = default)
//    {
//        var envelope = new { EventType = eventType, EntityId = entityId, OccurredAt = DateTimeOffset.UtcNow, Data = data };
//        return amps.PublishAsync($"ADB.{eventType}", JsonSerializer.Serialize(envelope), ct);
//        //return Task.CompletedTask;
//    }
//}

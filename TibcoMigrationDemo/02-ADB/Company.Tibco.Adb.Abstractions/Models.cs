namespace Company.Tibco.Adb.Abstractions;

public enum ProcessingStatus { New, Processing, Published, Failed }

public sealed record SourceEvent(
    long Id,
    string SourceTable,
    string SourceId,
    string Operation,
    string MessageType,
    string DestinationType,
    string DestinationName,
    string Payload,
    string MessageId,
    string? CorrelationId,
    ProcessingStatus Status,
    int RetryCount);

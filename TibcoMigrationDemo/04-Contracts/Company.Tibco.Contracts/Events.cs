namespace Company.Tibco.Contracts;

public sealed record EntityEvent<T>(
    string EventType,
    string EntityName,
    string EntityId,
    DateTimeOffset OccurredAt,
    T Data,
    string CorrelationId);

public sealed record CustomerDto(int Id, string CustomerNumber, string Name, string Email);
public sealed record AddressDto(int Id, int CustomerId, string Line1, string City, string State, string PostalCode);
public sealed record OrderDto(int Id, int CustomerId, string OrderNumber, decimal TotalAmount, string Status);
public sealed record PaymentDto(int Id, int OrderId, decimal Amount, string Status, string TransactionReference);

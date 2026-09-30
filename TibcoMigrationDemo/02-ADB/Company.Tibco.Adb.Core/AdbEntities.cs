//using Company.Tibco.Adb.Abstractions;

//namespace Company.Tibco.Adb.Core;

////public class Customer { public int Id { get; set; } public string CustomerNumber { get; set; } = ""; public string Name { get; set; } = ""; public string Email { get; set; } = ""; public ICollection<Address> Addresses { get; set; } = new List<Address>(); public ICollection<Order> Orders { get; set; } = new List<Order>(); }
////public class Address { public int Id { get; set; } public int CustomerId { get; set; } public string Line1 { get; set; } = ""; public string City { get; set; } = ""; public string State { get; set; } = ""; public string PostalCode { get; set; } = ""; public Customer? Customer { get; set; } }
////public class Order { public int Id { get; set; } public int CustomerId { get; set; } public string OrderNumber { get; set; } = ""; public decimal TotalAmount { get; set; } public string Status { get; set; } = "NEW"; public Customer? Customer { get; set; } public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>(); public ICollection<Payment> Payments { get; set; } = new List<Payment>(); }
////public class OrderItem { public int Id { get; set; } public int OrderId { get; set; } public string ProductCode { get; set; } = ""; public int Quantity { get; set; } public decimal UnitPrice { get; set; } public Order? Order { get; set; } }
////public class Payment { public int Id { get; set; } public int OrderId { get; set; } public decimal Amount { get; set; } public string Status { get; set; } = "PENDING"; public string TransactionReference { get; set; } = ""; public Order? Order { get; set; } }

//public class Order
//{
//    public long Id { get; set; }
//    public string OrderNumber { get; set; } = "";
//    public decimal Amount { get; set; }
//}

//public class Customer
//{
//    public long Id { get; set; }
//    public string Name { get; set; } = "";
//}

//public class Payment
//{
//    public long Id { get; set; }
//    public long OrderId { get; set; }
//    public decimal Amount { get; set; }
//}

//public abstract class PBase
//{
//    public long Id { get; set; }
//    public string Operation { get; set; } = "";
//    public string MessageType { get; set; } = "";
//    public string DestinationType { get; set; } = "TOPIC";
//    public string DestinationName { get; set; } = "";
//    public string Payload { get; set; } = "";
//    public string MessageId { get; set; } = "";
//    public string? CorrelationId { get; set; }
//    public ProcessingStatus Status { get; set; } = ProcessingStatus.New;
//    public int RetryCount { get; set; }
//    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
//    public DateTime? ProcessedUtc { get; set; }
//    public string? LastError { get; set; }
//}

//public class POrder : PBase { public long OrderId { get; set; } }
//public class PCustomer : PBase { public long CustomerId { get; set; } }
//public class PPayment : PBase { public long PaymentId { get; set; } }

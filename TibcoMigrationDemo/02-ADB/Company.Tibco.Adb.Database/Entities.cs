using Company.Tibco.Adb.Abstractions; 
using Microsoft.EntityFrameworkCore;

namespace Company.Tibco.Adb.Database;
public class Order
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = "";
    public decimal Amount { get; set; }
}

public class Customer
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
}

public class Payment
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public decimal Amount { get; set; }
}

public abstract class PBase
{
    public long Id { get; set; }
    public string Operation { get; set; } = "";
    public string MessageType { get; set; } = "";
    public string DestinationType { get; set; } = "TOPIC";
    public string DestinationName { get; set; } = "";
    public string Payload { get; set; } = "";
    public string MessageId { get; set; } = "";
    public string? CorrelationId { get; set; }
    public ProcessingStatus Status { get; set; } = ProcessingStatus.New;
    public int RetryCount { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedUtc { get; set; }
    public string? LastError { get; set; }
}

public class POrder : PBase { public long OrderId { get; set; } }
public class PCustomer : PBase { public long CustomerId { get; set; } }
public class PPayment : PBase { public long PaymentId { get; set; } }

public sealed class Entities : DbContext
{
    public Entities(DbContextOptions<Entities> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<POrder> POrders => Set<POrder>();
    public DbSet<PCustomer> PCustomers => Set<PCustomer>();
    public DbSet<PPayment> PPayments => Set<PPayment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var e in new[] { typeof(POrder), typeof(PCustomer), typeof(PPayment) })
        {
            modelBuilder.Entity(e).Property(nameof(PBase.Status)).HasConversion<string>();
            modelBuilder.Entity(e).HasIndex(nameof(PBase.Status), nameof(PBase.CreatedUtc));
        }
    }
}

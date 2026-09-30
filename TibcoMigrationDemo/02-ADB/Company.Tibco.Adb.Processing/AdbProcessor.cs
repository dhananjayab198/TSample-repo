using Company.Tibco.Adb.Abstractions;
using Company.Tibco.Adb.Database;
using Company.Tibco.Adb.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Company.Tibco.Adb.Processing;

public interface IAdbProcessor { Task ProcessAllAsync(CancellationToken ct); }

public sealed class AdbProcessor : IAdbProcessor
{
    private readonly IDbContextFactory<Entities> _factory;
    private readonly ISourceEventPublisher _publisher;

    public AdbProcessor(IDbContextFactory<Entities> factory, ISourceEventPublisher publisher)
    {
        _factory = factory;
        _publisher = publisher;
    }

    public async Task ProcessAllAsync(CancellationToken ct)
    {
        await ProcessOrders(ct);
        await ProcessCustomers(ct);
        await ProcessPayments(ct);
    }

    private async Task ProcessOrders(CancellationToken ct)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var rows = await db.POrders
            .Where(x => x.Status == ProcessingStatus.New || x.Status == ProcessingStatus.Failed)
            .OrderBy(x => x.CreatedUtc).Take(100).ToListAsync(ct);

        foreach (var row in rows)
        {
            await ProcessOne(row, "ORDER", row.OrderId.ToString(), db, ct);
        }
    }

    private async Task ProcessCustomers(CancellationToken ct)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var rows = await db.PCustomers
            .Where(x => x.Status == ProcessingStatus.New || x.Status == ProcessingStatus.Failed)
            .OrderBy(x => x.CreatedUtc).Take(100).ToListAsync(ct);

        foreach (var row in rows)
        {
            await ProcessOne(row, "CUSTOMER", row.CustomerId.ToString(), db, ct);
        }
    }

    private async Task ProcessPayments(CancellationToken ct)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var rows = await db.PPayments
            .Where(x => x.Status == ProcessingStatus.New || x.Status == ProcessingStatus.Failed)
            .OrderBy(x => x.CreatedUtc).Take(100).ToListAsync(ct);

        foreach (var row in rows)
        {
            await ProcessOne(row, "PAYMENT", row.PaymentId.ToString(), db, ct);
        }
    }

    private async Task ProcessOne(
        PBase row,
        string sourceTable,
        string sourceId,
        Entities db,
        CancellationToken ct)
    {
        try
        {
            row.Status = ProcessingStatus.Processing;
            await db.SaveChangesAsync(ct);

            var e = new SourceEvent(
                row.Id, sourceTable, sourceId, row.Operation, row.MessageType,
                row.DestinationType, row.DestinationName, row.Payload,
                row.MessageId, row.CorrelationId, row.Status, row.RetryCount);

            await _publisher.PublishAsync(e, ct);

            row.Status = ProcessingStatus.Published;
            row.ProcessedUtc = DateTime.UtcNow;
            row.LastError = null;
            await db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            row.Status = ProcessingStatus.Failed;
            row.RetryCount++;
            row.LastError = ex.Message;
            await db.SaveChangesAsync(ct);
        }
    }
}

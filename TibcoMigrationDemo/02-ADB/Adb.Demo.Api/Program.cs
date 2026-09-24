using Company.Tibco.Adb.Core;
using Company.Tibco.Adb.Database;
using Company.Tibco.Adb.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTibcoAdb(builder.Configuration);
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AdbDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.MapGet("/api/customers", async (ICustomerRepository r, CancellationToken ct) => Results.Ok(await r.GetAllAsync(ct)));
app.MapGet("/api/customers/{id:int}", async (int id, ICustomerRepository r, CancellationToken ct) => { var x = await r.GetAsync(id, ct); return x is null ? Results.NotFound() : Results.Ok(x); });
app.MapPost("/api/customers", async (Customer x, ICustomerRepository r, IAdbEventPublisher p, CancellationToken ct) => { var e = await r.AddAsync(x, ct); await p.PublishAsync("CUSTOMER.CREATED", e.Id.ToString(), e, ct); return Results.Created($"/api/customers/{e.Id}", e); });
app.MapPut("/api/customers/{id:int}", async (int id, Customer x, ICustomerRepository r, IAdbEventPublisher p, CancellationToken ct) => { if (id != x.Id) return Results.BadRequest(); if (await r.GetAsync(id, ct) is null) return Results.NotFound(); await r.UpdateAsync(x, ct); await p.PublishAsync("CUSTOMER.UPDATED", id.ToString(), x, ct); return Results.NoContent(); });
app.MapDelete("/api/customers/{id:int}", async (int id, ICustomerRepository r, IAdbEventPublisher p, CancellationToken ct) => { if (await r.GetAsync(id, ct) is null) return Results.NotFound(); await r.DeleteAsync(id, ct); await p.PublishAsync("CUSTOMER.DELETED", id.ToString(), new { Id = id }, ct); return Results.NoContent(); });

app.MapGet("/api/orders", async (IOrderRepository r, CancellationToken ct) => Results.Ok(await r.GetAllAsync(ct)));
app.MapPost("/api/orders", async (Order x, IOrderRepository r, IAdbEventPublisher p, CancellationToken ct) => { var e = await r.AddAsync(x, ct); await p.PublishAsync("ORDER.CREATED", e.Id.ToString(), e, ct); return Results.Created($"/api/orders/{e.Id}", e); });
app.MapPut("/api/orders/{id:int}", async (int id, Order x, IOrderRepository r, IAdbEventPublisher p, CancellationToken ct) => { if (id != x.Id || await r.GetAsync(id, ct) is null) return Results.NotFound(); await r.UpdateAsync(x, ct); await p.PublishAsync("ORDER.UPDATED", id.ToString(), x, ct); return Results.NoContent(); });
app.MapDelete("/api/orders/{id:int}", async (int id, IOrderRepository r, IAdbEventPublisher p, CancellationToken ct) => { if (await r.GetAsync(id, ct) is null) return Results.NotFound(); await r.DeleteAsync(id, ct); await p.PublishAsync("ORDER.DELETED", id.ToString(), new { Id = id }, ct); return Results.NoContent(); });

app.Run();

 
using Company.Tibco.Adb.Database; 
using Company.Tibco.Bw.Processes;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTibcoBwProcesses(builder.Configuration);
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Entities>();
    await db.Database.EnsureCreatedAsync();
}

app.MapGet("/api/bw/customers", async (ICustomerProcess p, CancellationToken ct) => Results.Ok(await p.GetAllAsync(ct)));
app.MapGet("/api/bw/customers/{id:long}", async (long id, ICustomerProcess p, CancellationToken ct) => { var x = await p.GetAsync(id, ct); return x is null ? Results.NotFound() : Results.Ok(x); });
app.MapPost("/api/bw/customers", async (Customer x, ICustomerProcess p, CancellationToken ct) => { var e = await p.CreateAsync(x, ct); return Results.Created($"/api/bw/customers/{e.Id}", e); });
app.MapPut("/api/bw/customers/{id:long}", async (long id, Customer x, ICustomerProcess p, CancellationToken ct) => { if (id != x.Id) return Results.BadRequest(); if (await p.GetAsync(id, ct) is null) return Results.NotFound(); await p.UpdateAsync(x, ct); return Results.NoContent(); });
app.MapDelete("/api/bw/customers/{id:long}", async (long id, ICustomerProcess p, CancellationToken ct) => { if (await p.GetAsync(id, ct) is null) return Results.NotFound(); await p.DeleteAsync(id, ct); return Results.NoContent(); });

app.MapGet("/api/bw/orders", async (OrderProcess p, CancellationToken ct) => Results.Ok(await p.GetAllAsync(ct)));
app.MapGet("/api/bw/orders/{id:long}", async (long id, OrderProcess p, CancellationToken ct) => { var x = await p.GetAsync(id, ct); return x is null ? Results.NotFound() : Results.Ok(x); });
app.MapPost("/api/bw/orders", async (Order x, OrderProcess p, CancellationToken ct) => { var e = await p.CreateAsync(x, ct); return Results.Created($"/api/bw/orders/{e.Id}", e); });
app.MapPut("/api/bw/orders/{id:long}", async (long id, Order x, OrderProcess p, CancellationToken ct) => { if (id != x.Id) return Results.BadRequest(); if (await p.GetAsync(id, ct) is null) return Results.NotFound(); await p.UpdateAsync(x, ct); return Results.NoContent(); });
app.MapDelete("/api/bw/orders/{id:long}", async (long id, OrderProcess p, CancellationToken ct) => { if (await p.GetAsync(id, ct) is null) return Results.NotFound(); await p.DeleteAsync(id, ct); return Results.NoContent(); });

app.Run();

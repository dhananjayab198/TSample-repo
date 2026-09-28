using Company.Tibco.Adb.Core;
using Company.Tibco.Adb.Database;
using Company.Tibco.Bw.Processes;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTibcoBwProcesses(builder.Configuration);
var app = builder.Build();
using (var scope = app.Services.CreateScope())
    await scope.ServiceProvider.GetRequiredService<AdbDbContext>().Database.EnsureCreatedAsync();

app.MapGet("/api/bw/customers", async (ICustomerProcess p, CancellationToken ct) => Results.Ok(await p.GetAllAsync(ct)));
app.MapGet("/api/bw/customers/{id:int}", async (int id, ICustomerProcess p, CancellationToken ct) => { var x = await p.GetAsync(id, ct); return x is null ? Results.NotFound() : Results.Ok(x); });
app.MapPost("/api/bw/customers", async (Customer x, ICustomerProcess p, CancellationToken ct) => { var e = await p.CreateAsync(x, ct); return Results.Created($"/api/bw/customers/{e.Id}", e); });
app.MapPut("/api/bw/customers/{id:int}", async (int id, Customer x, ICustomerProcess p, CancellationToken ct) => { if (id != x.Id) return Results.BadRequest(); if (await p.GetAsync(id, ct) is null) return Results.NotFound(); await p.UpdateAsync(x, ct); return Results.NoContent(); });
app.MapDelete("/api/bw/customers/{id:int}", async (int id, ICustomerProcess p, CancellationToken ct) => { if (await p.GetAsync(id, ct) is null) return Results.NotFound(); await p.DeleteAsync(id, ct); return Results.NoContent(); });

app.MapGet("/api/bw/orders", async (OrderProcess p, CancellationToken ct) => Results.Ok(await p.GetAllAsync(ct)));
app.MapGet("/api/bw/orders/{id:int}", async (int id, OrderProcess p, CancellationToken ct) => { var x = await p.GetAsync(id, ct); return x is null ? Results.NotFound() : Results.Ok(x); });
app.MapPost("/api/bw/orders", async (Order x, OrderProcess p, CancellationToken ct) => { var e = await p.CreateAsync(x, ct); return Results.Created($"/api/bw/orders/{e.Id}", e); });
app.MapPut("/api/bw/orders/{id:int}", async (int id, Order x, OrderProcess p, CancellationToken ct) => { if (id != x.Id) return Results.BadRequest(); if (await p.GetAsync(id, ct) is null) return Results.NotFound(); await p.UpdateAsync(x, ct); return Results.NoContent(); });
app.MapDelete("/api/bw/orders/{id:int}", async (int id, OrderProcess p, CancellationToken ct) => { if (await p.GetAsync(id, ct) is null) return Results.NotFound(); await p.DeleteAsync(id, ct); return Results.NoContent(); });
app.Run();

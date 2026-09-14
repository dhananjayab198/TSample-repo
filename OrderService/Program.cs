using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using OrderService.DAO;

var builder = WebApplication.CreateBuilder(args);
 

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("OrderDb")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

}

app.UseSwagger(); 

//app.UseHttpsRedirection();
app.MapControllers();




app.Run();

 
 
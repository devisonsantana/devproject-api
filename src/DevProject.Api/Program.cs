using DevProject.Application;
using DevProject.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string \"DefaultConnection\" not found.");

builder.Services.AddInfrastructure(connectionString);
builder.Services.AddServices();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

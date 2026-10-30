using DevProject.Application;
using DevProject.Infrastructure;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? (builder.Environment.IsEnvironment("Testing")
        ? null
        : throw new InvalidOperationException("Connection string \"DefaultConnection\" not found."));

if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddInfrastructure(connectionString);
}

builder.Services.AddServices();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApiDocument(options =>
{
    options.Title = "DevProject API";
    options.Version = "v1";
    options.Description = "API for projects, features, tasks, and user registration.";
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
{
    app.UseOpenApi();
    app.UseSwaggerUi(options => options.Path = "/swagger");
}

app.MapControllers();

app.Run();

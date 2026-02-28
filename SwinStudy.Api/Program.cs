using Microsoft.EntityFrameworkCore;
using SwinStudy.Api.Data;
using SwinStudy.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers (classic MVC-style API)
builder.Services.AddControllers();

// Swagger (OpenAPI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// EF Core + Postgres
var connectionString = builder.Configuration.GetConnectionString("Default");

// Fallback to environment variables / sensible defaults if config is missing or incomplete.
if (string.IsNullOrWhiteSpace(connectionString))
{
    var db = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "swinstudy";
    var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
    var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "sarthak";
    var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "sarthak";

    connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={password}";
}

// Log which connection string is being used (without password).
try
{
    var redacted = connectionString;
    const string passwordKey = "Password=";
    var idx = redacted.IndexOf(passwordKey, StringComparison.OrdinalIgnoreCase);
    if (idx >= 0)
    {
        var end = redacted.IndexOf(';', idx);
        if (end < 0) end = redacted.Length;
        redacted = redacted[..idx] + "Password=***" + redacted[end..];
    }
    Console.WriteLine($"Using Postgres connection: {redacted}");
}
catch
{
    // best-effort logging only
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Services (DI)
builder.Services.AddScoped<DegreesService>();
builder.Services.AddScoped<UnitsService>();
builder.Services.AddScoped<FlashcardsService>();

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// CORS
app.UseCors("AllowFrontend");

// Usual middleware
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
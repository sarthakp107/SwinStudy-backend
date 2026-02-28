using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SwinStudy.Api.Data;
using SwinStudy.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers (classic MVC-style API) - require JWT by default
builder.Services.AddControllers(options =>
{
    var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .Build();
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
});

// Swagger (OpenAPI) with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SwinStudy API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header. Example: \"Bearer {token}\"",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

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

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is required.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "SwinStudy.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "SwinStudy.Api";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                // Prevent default 401 response
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unauthorized",
                });
            }
        };
    });

builder.Services.AddAuthorization();

// Services (DI)
builder.Services.AddScoped<AuthService>();
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
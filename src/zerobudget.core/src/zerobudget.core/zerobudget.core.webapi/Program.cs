using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Wolverine;
using Scalar.AspNetCore;
using System.Text;
using zerobudget.core.webapi;
using zerobudget.core.webapi.Services;
using zerobudget.core.infrastructure.data;
using zerobudget.core.application.Handlers.Queries;
using zerobudget.core.application.Handlers.Commands;
using zerobudget.core.application.Middleware;
using zerobudget.core.identity.Data;
using zerobudget.core.identity.Entities;

var builder = WebApplication.CreateBuilder(args);

// Get JWT settings from configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "your-super-secret-key-change-in-production-at-least-32-chars";
var issuer = jwtSettings["Issuer"] ?? "ZeroBudgetAPI";
var audience = jwtSettings["Audience"] ?? "ZeroBudgetClients";

// Add services to the container.
builder.Services.AddControllers();

// Configure ASP.NET Core Identity
builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseInMemoryDatabase("ZeroBudget_Identity"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;
        options.Password.RequiredUniqueChars = 1;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Register JWT Token Service
builder.Services.AddScoped<ITokenService, JwtTokenService>();

// Configure OpenAPI with detailed documentation
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "ZeroBudget API",
            Version = "v1",
            Description = "A comprehensive API for managing budgets, buckets, spendings, and tags in the ZeroBudget application.",
            Contact = new()
            {
                Name = "ZeroBudget Team",
                Email = "support@zerobudget.com"
            }
        };
        return Task.CompletedTask;
    });
});

// Add application services (includes Wolverine configuration)
builder.Services.AddZeroBudgetApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Map OpenAPI document
    app.MapOpenApi();

    // Add Scalar UI for API testing and documentation
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("ZeroBudget API Documentation")
            .WithTheme(ScalarTheme.Purple)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .EnableDarkMode();
    });
}

app.UseHttpsRedirection();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using Scalar.AspNetCore;
using Wolverine;
using zerobudget.core.webapi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS configuration
var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowConfiguredOrigins", builder =>
    {
        builder
            .WithOrigins(allowedOrigins?.Split(";") ?? Array.Empty<string>())
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add identity services (JWT, ASP.NET Core Identity, etc.)
builder.Services.AddZeroBudgetIdentity(builder.Configuration);
// Add application services
builder.Services.AddZeroBudgetApplication();
// Add Wolverine with discovery
builder.Services.AddWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(zerobudget.core.application.MarkerClass).Assembly);
    opts.Discovery.IncludeAssembly(typeof(zerobudget.core.identity.MarkerClass).Assembly);
    // TODO: Re-enable exception middleware after fixing Wolverine compatibility
    // opts.Policies.AddMiddleware<GlobalExceptionMiddleware>();
});
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

// Add CORS middleware
app.UseCors("AllowConfiguredOrigins");

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

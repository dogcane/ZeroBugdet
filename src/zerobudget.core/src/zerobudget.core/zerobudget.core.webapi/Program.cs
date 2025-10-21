using Wolverine;
using Scalar.AspNetCore;
using zerobudget.core.webapi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

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
            .WithDarkMode(true);
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

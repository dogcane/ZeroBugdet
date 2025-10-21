using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using zerobudget.core.application.Handlers.Commands;
using zerobudget.core.application.Handlers.Queries;
using zerobudget.core.application.Middleware;

namespace zerobudget.core.application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddZeroBudgetApplication(this IServiceCollection services)
    {
        // Add Wolverine with global exception middleware
        services.AddWolverine(opts =>
        {
            // Add global exception middleware to all message handlers
            opts.Policies.AddMiddleware<GlobalExceptionMiddleware>();
        });

        // Note: Handlers registration is commented out until repository implementations are available
        // This allows the controllers to be created but they will return errors when called

        // Register command handlers
        // services.AddScoped<BucketCommandHandlers>();
        // services.AddScoped<MonthlyBucketCommandHandlers>();
        // services.AddScoped<TagCommandHandlers>();
        // services.AddScoped<SpendingCommandHandlers>();
        // services.AddScoped<MonthlySpendingCommandHandlers>();

        // Register query handlers
        // services.AddScoped<BucketQueryHandlers>();
        // services.AddScoped<MonthlyBucketQueryHandlers>();
        // services.AddScoped<TagQueryHandlers>();
        // services.AddScoped<SpendingQueryHandlers>();
        // services.AddScoped<MonthlySpendingQueryHandlers>();

        return services;
    }
}

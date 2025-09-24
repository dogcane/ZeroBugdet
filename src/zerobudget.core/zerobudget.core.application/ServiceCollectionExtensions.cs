using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using zerobudget.core.application.Handlers.Commands;
using zerobudget.core.application.Handlers.Queries;

namespace zerobudget.core.application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddZeroBudgetApplication(this IServiceCollection services)
    {
        // Add Wolverine
        services.AddWolverine();

        // Register command handlers
        services.AddScoped<BucketCommandHandlers>();
        services.AddScoped<MonthlyBucketCommandHandlers>();
        services.AddScoped<TagCommandHandlers>();
        services.AddScoped<SpendingCommandHandlers>();
        services.AddScoped<MonthlySpendingCommandHandlers>();

        // Register query handlers
        services.AddScoped<BucketQueryHandlers>();
        services.AddScoped<MonthlyBucketQueryHandlers>();
        services.AddScoped<TagQueryHandlers>();
        services.AddScoped<SpendingQueryHandlers>();
        services.AddScoped<MonthlySpendingQueryHandlers>();

        return services;
    }
}
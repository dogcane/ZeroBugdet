using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using zerobudget.core.application.Handlers.Commands;
using zerobudget.core.application.Handlers.Queries;
using zerobudget.core.application.Middleware;
using zerobudget.core.domain;
using zerobudget.core.infrastructure.data;
using ECO.Data;
using ECO.Providers.EntityFramework.Configuration;
using ECO.Integrations.Microsoft.DependencyInjection;

namespace zerobudget.core.webapi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddZeroBudgetApplication(this IServiceCollection services)
    {
        // Configure the data context with Entity Framework and In-Memory database
        services.AddDataContext(options =>
        {
            options.UseEntityFramework<ZBDbContext>("zerobudget.unit", opt =>
            opt.DbContextOptions.UseInMemoryDatabase("ZeroBudgetInMemoryDb"));
        });

        // Add Wolverine with global exception middleware
        services.AddWolverine(opts =>
        {
            // Add global exception middleware to all message handlers
            opts.Policies.AddMiddleware<GlobalExceptionMiddleware>();
        });

        // Register repository implementations
        services.AddScoped<IBucketRepository, BucketEFRepository>();
        services.AddScoped<IMonthlyBucketRepository, MonthlyBucketEFRepository>();
        services.AddScoped<ITagRepository, TagEFRepository>();
        services.AddScoped<ISpendingRepository, SpendingEFRepository>();
        services.AddScoped<IMonthlySpendingRepository, MonthlySpendingEFRepository>();

        // Register domain services
        services.AddScoped<ITagService, TagService>();

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

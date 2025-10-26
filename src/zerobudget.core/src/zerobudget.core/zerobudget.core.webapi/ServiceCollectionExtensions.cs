using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using zerobudget.core.application.Handlers.Commands;
using zerobudget.core.application.Handlers.Queries;
using zerobudget.core.application.Queries;
using zerobudget.core.application.Middleware;
using zerobudget.core.identity.Handlers.Commands;
using zerobudget.core.identity.Handlers.Queries;
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

        // Register repository implementations
        services.AddScoped<IBucketRepository, BucketEFRepository>();
        services.AddScoped<IMonthlyBucketRepository, MonthlyBucketEFRepository>();
        services.AddScoped<ITagRepository, TagEFRepository>();
        services.AddScoped<ISpendingRepository, SpendingEFRepository>();
        services.AddScoped<IMonthlySpendingRepository, MonthlySpendingEFRepository>();

        // Register domain services
        services.AddScoped<ITagService, TagService>();

        // Register command and query handlers (split into individual classes for Wolverine)
        // Query Handlers
        services.AddScoped<GetBucketByIdQueryHandler>();
        services.AddScoped<GetBucketsByNameQueryHandler>();
        services.AddScoped<GetMonthlyBucketByIdQueryHandler>();
        services.AddScoped<GetAllMonthlyBucketsQueryHandler>();
        services.AddScoped<GetMonthlyBucketsByYearMonthQueryHandler>();
        services.AddScoped<GetMonthlyBucketsByBucketIdQueryHandler>();
        services.AddScoped<GetTagByIdQueryHandler>();
        services.AddScoped<GetAllTagsQueryHandler>();
        services.AddScoped<GetTagsByNameQueryHandler>();
        services.AddScoped<GetSpendingByIdQueryHandler>();
        services.AddScoped<GetAllSpendingsQueryHandler>();
        services.AddScoped<GetSpendingsByBucketIdQueryHandler>();
        services.AddScoped<GetSpendingsByOwnerQueryHandler>();
        services.AddScoped<GetMonthlySpendingByIdQueryHandler>();
        services.AddScoped<GetAllMonthlySpendingsQueryHandler>();
        services.AddScoped<GetMonthlySpendingsByMonthlyBucketIdQueryHandler>();
        services.AddScoped<GetMonthlySpendingsByDateRangeQueryHandler>();
        services.AddScoped<GetMonthlySpendingsByOwnerQueryHandler>();

        // User Query Handlers
        services.AddScoped<IsMainUserRequiredQueryHandler>();
        services.AddScoped<GetAllUsersQueryHandler>();
        services.AddScoped<GetUserByIdQueryHandler>();
        services.AddScoped<ValidateInvitationTokenQueryHandler>();
        services.AddScoped<GetInvitationsByUserQueryHandler>();

        // Command Handlers
        services.AddScoped<CreateBucketCommandHandler>();
        services.AddScoped<UpdateBucketCommandHandler>();
        services.AddScoped<DeleteBucketCommandHandler>();
        services.AddScoped<EnableBucketCommandHandler>();
        services.AddScoped<GenerateMonthlyDataCommandHandler>();
        services.AddScoped<CreateTagCommandHandler>();
        services.AddScoped<DeleteTagCommandHandler>();
        services.AddScoped<CleanupUnusedTagsCommandHandler>();
        services.AddScoped<CreateSpendingCommandHandler>();
        services.AddScoped<UpdateSpendingCommandHandler>();
        services.AddScoped<DeleteSpendingCommandHandler>();
        services.AddScoped<EnableSpendingCommandHandler>();
        services.AddScoped<CreateMonthlySpendingCommandHandler>();
        services.AddScoped<UpdateMonthlySpendingCommandHandler>();
        services.AddScoped<DeleteMonthlySpendingCommandHandler>();

        // User Command Handlers
        services.AddScoped<RegisterMainUserCommandHandler>();
        services.AddScoped<InviteUserCommandHandler>();
        services.AddScoped<CompleteUserRegistrationCommandHandler>();
        services.AddScoped<DeleteUserCommandHandler>();

        // Add Wolverine with discovery
        services.AddWolverine(opts =>
        {
            // Enable discovery in the application assembly
            opts.Discovery.IncludeAssembly(typeof(GetBucketByIdQueryHandler).Assembly);

            // TODO: Re-enable exception middleware after fixing Wolverine compatibility
            // opts.Policies.AddMiddleware<GlobalExceptionMiddleware>();
        });

        return services;
    }
}

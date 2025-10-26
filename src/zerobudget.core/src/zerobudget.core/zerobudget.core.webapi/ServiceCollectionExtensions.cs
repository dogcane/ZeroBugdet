using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Wolverine;
using System.Text;
using zerobudget.core.application.Handlers.Commands;
using zerobudget.core.application.Handlers.Queries;
using zerobudget.core.application.Queries;
using zerobudget.core.application.Middleware;
using zerobudget.core.identity.Handlers.Commands;
using zerobudget.core.identity.Handlers.Queries;
using zerobudget.core.identity.Data;
using zerobudget.core.identity.Entities;
using zerobudget.core.domain;
using zerobudget.core.infrastructure.data;
using zerobudget.core.webapi.Services;
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

        return services;
    }

    public static IServiceCollection AddZeroBudgetIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        // Get JWT settings from configuration
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "your-super-secret-key-change-in-production-at-least-32-chars";
        var issuer = jwtSettings["Issuer"] ?? "ZeroBudgetAPI";
        var audience = jwtSettings["Audience"] ?? "ZeroBudgetClients";

        // Configure ASP.NET Core Identity
        services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseInMemoryDatabase("ZeroBudget_Identity"));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
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

        services
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
        services.AddScoped<ITokenService, JwtTokenService>();

        // Command Handlers
        services.AddScoped<RegisterMainUserCommandHandler>();
        services.AddScoped<InviteUserCommandHandler>();
        services.AddScoped<CompleteUserRegistrationCommandHandler>();
        services.AddScoped<DeleteUserCommandHandler>();

        // Query Handlers
        services.AddScoped<IsMainUserRequiredQueryHandler>();
        services.AddScoped<GetAllUsersQueryHandler>();
        services.AddScoped<GetUserByIdQueryHandler>();
        services.AddScoped<ValidateInvitationTokenQueryHandler>();
        services.AddScoped<GetInvitationsByUserQueryHandler>();

        return services;
    }
}

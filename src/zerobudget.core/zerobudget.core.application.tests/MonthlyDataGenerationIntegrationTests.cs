using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using zerobudget.core.application.Commands;
using zerobudget.core.application.Handlers.Commands;
using zerobudget.core.domain;
using zerobudget.core.infrastructure.data;

namespace zerobudget.core.application.tests.Integration;

/// <summary>
/// Integration tests for MonthlyDataGeneration using in-memory database
/// This demonstrates how to test the handler with actual IQueryable operations
/// </summary>
public class MonthlyDataGenerationIntegrationTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ZBDbContext _context;
    private readonly MonthlyBucketCommandHandlers _handler;

    public MonthlyDataGenerationIntegrationTests()
    {
        var services = new ServiceCollection();
        
        // Configure in-memory database
        services.AddDbContext<ZBDbContext>(options =>
            options.UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}"));
        
        // Register repositories
        services.AddScoped<IBucketRepository, BucketEFRepository>();
        services.AddScoped<ISpendingRepository, SpendingEFRepository>();
        services.AddScoped<IMonthlyBucketRepository, MonthlyBucketEFRepository>();
        services.AddScoped<IMonthlySpendingRepository, MonthlySpendingEFRepository>();
        
        // Register handler
        services.AddScoped<MonthlyBucketCommandHandlers>();
        services.AddLogging(builder => builder.AddConsole());
        
        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<ZBDbContext>();
        _handler = _serviceProvider.GetRequiredService<MonthlyBucketCommandHandlers>();
    }

    [Fact]
    public async Task Handle_GenerateMonthlyDataCommand_ShouldSucceedWithRealDatabase()
    {
        // Arrange
        await SeedTestDataAsync();
        var command = new GenerateMonthlyDataCommand(2025, 1);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2025, result.Value.Year);
        Assert.Equal(1, result.Value.Month);
        Assert.True(result.Value.MonthlyBucketsCreated > 0);
        Assert.True(result.Value.MonthlySpendingsCreated > 0);

        // Verify data was actually created in database
        var monthlyBuckets = await _context.Set<MonthlyBucket>()
            .Where(mb => mb.Year == 2025 && mb.Month == 1)
            .ToListAsync();
        
        Assert.NotEmpty(monthlyBuckets);
    }

    [Fact]
    public async Task Handle_GenerateMonthlyDataCommand_ShouldFailWhenDataExists()
    {
        // Arrange
        await SeedTestDataAsync();
        await SeedExistingMonthlyDataAsync();
        var command = new GenerateMonthlyDataCommand(2025, 1);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Errors?.First().Message);
    }

    private async Task SeedTestDataAsync()
    {
        // Create test buckets
        var bucket1 = Bucket.Create("Test Bucket 1", "Description 1", 1000m).Value!;
        var bucket2 = Bucket.Create("Test Bucket 2", "Description 2", 2000m).Value!;
        
        _context.Set<Bucket>().AddRange(bucket1, bucket2);
        await _context.SaveChangesAsync();

        // Create test spendings
        var spending1 = Spending.Create(
            new DateOnly(2025, 1, 15),
            "Test Spending 1",
            100m,
            "Owner 1",
            Array.Empty<Tag>(),
            bucket1).Value!;

        var spending2 = Spending.Create(
            new DateOnly(2025, 1, 20),
            "Test Spending 2",
            200m,
            "Owner 2",
            Array.Empty<Tag>(),
            bucket2).Value!;

        _context.Set<Spending>().AddRange(spending1, spending2);
        await _context.SaveChangesAsync();
    }

    private async Task SeedExistingMonthlyDataAsync()
    {
        var bucket = _context.Set<Bucket>().First();
        var monthlyBucket = bucket.CreateMonthly(2025, 1);
        
        _context.Set<MonthlyBucket>().Add(monthlyBucket);
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }
}

/// <summary>
/// Example showing how to create a test base class for repository testing with IQueryable
/// </summary>
public abstract class RepositoryTestBase<TEntity, TRepository> : IDisposable
    where TEntity : class
    where TRepository : class
{
    protected readonly ServiceProvider ServiceProvider;
    protected readonly ZBDbContext Context;
    protected readonly TRepository Repository;

    protected RepositoryTestBase()
    {
        var services = new ServiceCollection();
        
        services.AddDbContext<ZBDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{typeof(TEntity).Name}_{Guid.NewGuid()}"));
        
        // Register repository
        services.AddScoped<TRepository>();
        
        ServiceProvider = services.BuildServiceProvider();
        Context = ServiceProvider.GetRequiredService<ZBDbContext>();
        Repository = ServiceProvider.GetRequiredService<TRepository>();
    }

    protected async Task<TEntity> AddEntityAsync(TEntity entity)
    {
        Context.Set<TEntity>().Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    protected IQueryable<TEntity> GetEntities() => Context.Set<TEntity>();

    public virtual void Dispose()
    {
        Context?.Dispose();
        ServiceProvider?.Dispose();
    }
}

/// <summary>
/// Example of testing specific repository functionality with LINQ
/// </summary>
public class BucketRepositoryTests : RepositoryTestBase<Bucket, IBucketRepository>
{
    [Fact]
    public async Task Query_ShouldReturnEnabledBucketsOnly()
    {
        // Arrange
        var enabledBucket = await AddEntityAsync(
            Bucket.Create("Enabled", "Description", 1000m).Value!);
        
        var disabledBucket = Bucket.Create("Disabled", "Description", 1000m).Value!;
        disabledBucket.Disable();
        await AddEntityAsync(disabledBucket);

        // Act
        var enabledBuckets = await Repository.Query()
            .Where(b => b.Enabled)
            .ToListAsync();

        // Assert
        Assert.Single(enabledBuckets);
        Assert.Equal(enabledBucket.Identity, enabledBuckets.First().Identity);
    }
}

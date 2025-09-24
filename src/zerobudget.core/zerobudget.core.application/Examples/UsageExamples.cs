using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using Resulz;

namespace zerobudget.core.application.Examples;

/// <summary>
/// Usage examples for the ZeroBudget application layer
/// </summary>
public class UsageExamples
{
    /// <summary>
    /// Example of how to use the application layer with Wolverine
    /// </summary>
    public async Task<int> ExampleUsage(IMessageBus messageBus)
    {
        // Create a new bucket
        var createBucketCommand = new CreateBucketCommand(
            "Groceries", 
            "Monthly grocery budget", 
            800m);
        
        var bucketResult = await messageBus.InvokeAsync<OperationResult<BucketDto>>(createBucketCommand);
        
        if (!bucketResult.IsSuccess)
            throw new InvalidOperationException("Failed to create bucket");

        var bucketId = bucketResult.Value!.Id;

        // Create a monthly bucket for the current month
        var createMonthlyBucketCommand = new CreateMonthlyBucketCommand(
            2025, 
            1, 
            bucketId);
        
        await messageBus.InvokeAsync(createMonthlyBucketCommand);

        // Create a spending entry
        var createSpendingCommand = new CreateSpendingCommand(
            DateOnly.FromDateTime(DateTime.Today),
            bucketId,
            "Weekly groceries at Supermarket",
            156.78m,
            "John Doe",
            new int[] { }); // No tags for this example
        
        await messageBus.InvokeAsync(createSpendingCommand);

        // Query all buckets
        var getAllBucketsQuery = new GetAllBucketsQuery();
        var allBuckets = await messageBus.InvokeAsync<IEnumerable<BucketDto>>(getAllBucketsQuery);

        // Query spendings by bucket
        var getSpendingsByBucketQuery = new GetSpendingsByBucketIdQuery(bucketId);
        var bucketSpendings = await messageBus.InvokeAsync<IEnumerable<SpendingDto>>(getSpendingsByBucketQuery);

        return bucketId;
    }

    /// <summary>
    /// Example of service registration in a DI container
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {
        // Register the application layer
        services.AddZeroBudgetApplication();

        // Register domain repositories (these would be implemented in infrastructure layer)
        // services.AddScoped<IBucketRepository, BucketRepository>();
        // services.AddScoped<ITagRepository, TagRepository>();
        // services.AddScoped<ISpendingRepository, SpendingRepository>();
        // services.AddScoped<IMonthlyBucketRepository, MonthlyBucketRepository>();
        // services.AddScoped<IMonthlySpendingRepository, MonthlySpendingRepository>();
    }
}
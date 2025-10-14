using System.Transactions;
using Microsoft.Extensions.Logging;
using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Commands;

public class MonthlyBucketCommandHandlers(
    IMonthlyBucketRepository monthlyBucketRepository,
    IBucketRepository bucketRepository,
    ISpendingRepository spendingRepository,
    IMonthlySpendingRepository monthlySpendingRepository,
    ILogger<MonthlyBucketCommandHandlers>? logger = null)
{
    #region Fields
    private readonly IMonthlyBucketRepository _monthlyBucketRepository = monthlyBucketRepository;
    private readonly IBucketRepository _bucketRepository = bucketRepository;
    private readonly ISpendingRepository _spendingRepository = spendingRepository;
    private readonly IMonthlySpendingRepository _monthlySpendingRepository = monthlySpendingRepository;
    private readonly ILogger<MonthlyBucketCommandHandlers>? _logger = logger;
    #endregion

    public async Task<OperationResult<GenerateMonthlyDataResult>> Handle(GenerateMonthlyDataCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        _logger?.LogInformation("Starting monthly data generation for {Year}-{Month}", command.Year, command.Month);

        // Check if monthly buckets already exist for this period
        var existingMonthlyBuckets = _monthlyBucketRepository
            .Any(mb => mb.Year == command.Year && mb.Month == command.Month);
        if (existingMonthlyBuckets)
        {
            return OperationResult.MakeFailure(ErrorMessage.Create("Period", "Monthly data already exists for the specified period"));
        }

        // Get all enabled buckets
        var bucketList = _bucketRepository
            .Where(b => b.Enabled)
            .ToList();

        if (bucketList.Count == 0)
        {
            _logger?.LogWarning("No enabled buckets found for monthly data generation");
            return OperationResult.MakeFailure(ErrorMessage.Create("Bucket", "No enabled buckets found"));
        }

        _logger?.LogInformation("Found {Count} enabled buckets", bucketList.Count);

        // Create monthly buckets from enabled buckets
        var monthlyBuckets = new List<MonthlyBucket>();
        foreach (var bucket in bucketList)
        {
            var monthlyBucket = bucket.CreateMonthly(command.Year, command.Month);
            await _monthlyBucketRepository.AddAsync(monthlyBucket);
            monthlyBuckets.Add(monthlyBucket);
        }

        _logger?.LogInformation("Created {Count} monthly buckets", monthlyBuckets.Count);

        // Generate monthly spendings for each bucket
        var monthlySpendings = new List<MonthlySpending>();

        foreach (var monthlyBucket in monthlyBuckets)
        {
            // Get all enabled spendings for this bucket within the month
            var spendingsList = _spendingRepository
                .Where(s => s.BucketId == monthlyBucket.Bucket.Identity && s.Enabled)
                .ToList();

            _logger?.LogInformation("Found {Count} spendings for bucket {BucketId} in {Year}-{Month}",
                spendingsList.Count, monthlyBucket.Bucket.Identity, command.Year, command.Month);

            // Create monthly spendings from the spendings
            foreach (var spending in spendingsList)
            {
                var monthlySpending = spending.CreateMonthly(monthlyBucket);
                await _monthlySpendingRepository.AddAsync(monthlySpending);
                monthlySpendings.Add(monthlySpending);
            }
        }

        _logger?.LogInformation("Created {Count} monthly spendings", monthlySpendings.Count);

        scope.Complete();

        var result = new GenerateMonthlyDataResult(
            command.Year,
            command.Month,
            monthlyBuckets.Count,
            monthlySpendings.Count
        );

        _logger?.LogInformation("Successfully completed monthly data generation for {Year}-{Month}. " +
            "Created {MonthlyBuckets} monthly buckets and {MonthlySpendings} monthly spendings",
            command.Year, command.Month, monthlyBuckets.Count, monthlySpendings.Count);

        return OperationResult<GenerateMonthlyDataResult>.MakeSuccess(result);

    }
}
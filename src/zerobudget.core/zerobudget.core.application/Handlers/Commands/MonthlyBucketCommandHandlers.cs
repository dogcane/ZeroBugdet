using System.Transactions;
using Microsoft.Extensions.Logging;
using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;

namespace zerobudget.core.application.Handlers.Commands;

public class GenerateMonthlyDataCommandHandler(
    IMonthlyBucketRepository monthlyBucketRepository,
    IBucketRepository bucketRepository,
    ILogger<GenerateMonthlyDataCommandHandler>? logger = null)
{
    private readonly IMonthlyBucketRepository _monthlyBucketRepository = monthlyBucketRepository;
    private readonly IBucketRepository _bucketRepository = bucketRepository;
    private readonly ILogger<GenerateMonthlyDataCommandHandler>? _logger = logger;

    public async Task<OperationResult<bool>> Handle(GenerateMonthlyDataCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var buckets = _bucketRepository.AsQueryable().ToList();

        foreach (var bucket in buckets)
        {
            var monthlyBucketResult = bucket.CreateMonthly(command.Year, command.Month);
            if (!monthlyBucketResult.Success)
            {
                return OperationResult<bool>.MakeFailure(monthlyBucketResult.Errors);
            }

            await _monthlyBucketRepository.AddAsync(monthlyBucketResult.Value!);
        }

        scope.Complete();
        return OperationResult<bool>.MakeSuccess(true);
    }
}

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
    public async Task<OperationResult<bool>> Handle(GenerateMonthlyDataCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var buckets = bucketRepository.AsQueryable().ToList();

        foreach (var bucket in buckets)
        {
            var monthlyBucketResult = bucket.CreateMonthly(command.Year, command.Month);
            if (!monthlyBucketResult.Success)
            {
                return OperationResult<bool>.MakeFailure(monthlyBucketResult.Errors);
            }

            await monthlyBucketRepository.AddAsync(monthlyBucketResult.Value!);
        }

        scope.Complete();
        return OperationResult<bool>.MakeSuccess(true);
    }
}

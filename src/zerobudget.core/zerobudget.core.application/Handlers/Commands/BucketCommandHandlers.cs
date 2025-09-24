using ECO.Data;
using System.Transactions;
using Microsoft.Extensions.Logging;
using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Commands;

public class BucketCommandHandlers(IBucketRepository bucketRepository, IMonthlyBucketRepository monthlyBucketRepository, ILogger<BucketCommandHandlers>? logger = null)
{
    private readonly IBucketRepository _bucketRepository = bucketRepository;
    private readonly IMonthlyBucketRepository _monthlyBucketRepository = monthlyBucketRepository;
    private readonly ILogger<BucketCommandHandlers>? _logger = logger;


    public async Task<OperationResult<BucketDto>> Handle(CreateBucketCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var bucketResult = Bucket.Create(command.Name, command.Description, command.DefaultLimit);
            if (!bucketResult.Success)
            {
                return OperationResult<BucketDto>.MakeFailure(bucketResult.Errors);
            }

            var bucket = bucketResult.Value!;
            await _bucketRepository.AddAsync(bucket);

            scope.Complete();
            return OperationResult<BucketDto>.MakeSuccess(new BucketDto(
                bucket.Identity,
                bucket.Name,
                bucket.Description,
                bucket.DefaultLimit,
                bucket.DefaultBalance,
                bucket.Enabled
            ));
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in CreateBucketCommand handler");
            return OperationResult<BucketDto>.MakeFailure(ErrorMessage.Create("Exception", ex.Message));
        }
    }


    public async Task<OperationResult<BucketDto>> Handle(UpdateBucketCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var bucket = await _bucketRepository.LoadAsync(command.Id);
            if (bucket == null)
            {
                return OperationResult<BucketDto>.MakeFailure(ErrorMessage.Create(nameof(command.Id), "BUCKET_NOT_FOUND"));
            }

            bucket.Update(command.Name, command.Description, command.DefaultLimit);
            await _bucketRepository.UpdateAsync(bucket);

            scope.Complete();
            return OperationResult<BucketDto>.MakeSuccess(new BucketDto(
                bucket.Identity,
                bucket.Name,
                bucket.Description,
                bucket.DefaultLimit,
                bucket.DefaultBalance,
                bucket.Enabled
            ));
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in UpdateBucketCommand handler");
            return OperationResult<BucketDto>.MakeFailure(ErrorMessage.Create("Exception", ex.Message));
        }
    }

    public async Task<OperationResult> Handle(DeleteBucketCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var bucket = await _bucketRepository.LoadAsync(command.Id);
            if (bucket == null)
            {
                return OperationResult.MakeFailure(ErrorMessage.Create(nameof(command.Id), "BUCKET_NOT_FOUND"));
            }

            var hasRelatedMonthlyBuckets = _monthlyBucketRepository.Any(mb => mb.Bucket.Identity == bucket.Identity);
            if (hasRelatedMonthlyBuckets)
            {
                bucket.Disable();
                await _bucketRepository.UpdateAsync(bucket);
                scope.Complete();
                return OperationResult.MakeSuccess();
            }

            await _bucketRepository.RemoveAsync(bucket);
            scope.Complete();
            return OperationResult.MakeSuccess();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in DeleteBucketCommand handler");
            return OperationResult.MakeFailure(ErrorMessage.Create("Exception", ex.Message));
        }
    }

    public async Task<OperationResult> Handle(EnableBucketCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var bucket = await _bucketRepository.LoadAsync(command.Id);
            if (bucket == null)
            {
                return OperationResult.MakeFailure(ErrorMessage.Create(nameof(command.Id), "BUCKET_NOT_FOUND"));
            }

            bucket.Enable();
            await _bucketRepository.UpdateAsync(bucket);
            scope.Complete();
            return OperationResult.MakeSuccess();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in EnableBucketCommand handler");
            return OperationResult.MakeFailure(ErrorMessage.Create("Exception", ex.Message));
        }
    }
}
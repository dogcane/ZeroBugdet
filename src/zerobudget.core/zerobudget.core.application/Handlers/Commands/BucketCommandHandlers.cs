using ECO.Data;
using System.Transactions;
using Microsoft.Extensions.Logging;
using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;

namespace zerobudget.core.application.Handlers.Commands;

public class CreateBucketCommandHandler(IBucketRepository bucketRepository, ILogger<CreateBucketCommandHandler>? logger = null)
{
    private readonly BucketMapper _mapper = new();

    public async Task<OperationResult<BucketDto>> Handle(CreateBucketCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var bucketResult = Bucket.Create(command.Name, command.Description, command.DefaultLimit);
        if (!bucketResult.Success)
        {
            return OperationResult<BucketDto>.MakeFailure(bucketResult.Errors);
        }

        var bucket = bucketResult.Value!;
        await bucketRepository.AddAsync(bucket);

        scope.Complete();
        return OperationResult<BucketDto>.MakeSuccess(_mapper.ToDto(bucket));
    }
}

public class UpdateBucketCommandHandler(IBucketRepository bucketRepository, ILogger<UpdateBucketCommandHandler>? logger = null)
{
    private readonly BucketMapper _mapper = new();

    public async Task<OperationResult<BucketDto>> Handle(UpdateBucketCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var bucket = await bucketRepository.LoadAsync(command.Id);
        if (bucket == null)
        {
            return OperationResult<BucketDto>.MakeFailure(ErrorMessage.Create("UPDATE_BUCKET", "Bucket not found"));
        }

        var updateResult = bucket.Update(command.Name, command.Description, command.DefaultLimit);
        if (!updateResult.Success)
        {
            return OperationResult<BucketDto>.MakeFailure(updateResult.Errors);
        }

        await bucketRepository.UpdateAsync(bucket);

        scope.Complete();
        return OperationResult<BucketDto>.MakeSuccess(_mapper.ToDto(bucket));
    }
}

public class DeleteBucketCommandHandler(IBucketRepository bucketRepository, IMonthlyBucketRepository monthlyBucketRepository, ILogger<DeleteBucketCommandHandler>? logger = null)
{
    public async Task<OperationResult> Handle(DeleteBucketCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var bucket = await bucketRepository.LoadAsync(command.Id);
        if (bucket == null)
        {
            return OperationResult.MakeFailure(ErrorMessage.Create("DELETE_BUCKET", "Bucket not found"));
        }

        var hasRelatedMonthlyBuckets = monthlyBucketRepository.Any(mb => mb.Bucket.Identity == bucket.Identity);
        if (hasRelatedMonthlyBuckets)
        {
            var disableResult = bucket.Disable();
            if (!disableResult.Success)
            {
                return OperationResult.MakeFailure(disableResult.Errors);
            }
            await bucketRepository.UpdateAsync(bucket);
        }
        else
        {
            await bucketRepository.RemoveAsync(bucket);
        }

        scope.Complete();
        return OperationResult.MakeSuccess();
    }
}

public class EnableBucketCommandHandler(IBucketRepository bucketRepository, ILogger<EnableBucketCommandHandler>? logger = null)
{
    private readonly BucketMapper _mapper = new();

    public async Task<OperationResult<BucketDto>> Handle(EnableBucketCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var bucket = await bucketRepository.LoadAsync(command.Id);
        if (bucket == null)
        {
            return OperationResult<BucketDto>.MakeFailure(ErrorMessage.Create("ENABLE_BUCKET", "Bucket not found"));
        }

        var enableResult = bucket.Enable();
        if (!enableResult.Success)
        {
            return OperationResult<BucketDto>.MakeFailure(enableResult.Errors);
        }

        await bucketRepository.UpdateAsync(bucket);
        scope.Complete();
        return OperationResult<BucketDto>.MakeSuccess(_mapper.ToDto(bucket));
    }
}

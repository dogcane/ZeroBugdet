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
    private readonly IBucketRepository _bucketRepository = bucketRepository;
    private readonly ILogger<CreateBucketCommandHandler>? _logger = logger;
    private readonly BucketMapper _mapper = new BucketMapper();

    public async Task<OperationResult<BucketDto>> Handle(CreateBucketCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var bucketResult = Bucket.Create(command.Name, command.Description, command.DefaultLimit);
        if (!bucketResult.Success)
        {
            return OperationResult<BucketDto>.MakeFailure(bucketResult.Errors);
        }

        var bucket = bucketResult.Value!;
        await _bucketRepository.AddAsync(bucket);

        scope.Complete();
        return OperationResult<BucketDto>.MakeSuccess(_mapper.ToDto(bucket));
    }
}

public class UpdateBucketCommandHandler(IBucketRepository bucketRepository, ILogger<UpdateBucketCommandHandler>? logger = null)
{
    private readonly IBucketRepository _bucketRepository = bucketRepository;
    private readonly ILogger<UpdateBucketCommandHandler>? _logger = logger;
    private readonly BucketMapper _mapper = new BucketMapper();

    public async Task<OperationResult<BucketDto>> Handle(UpdateBucketCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var bucket = await _bucketRepository.LoadAsync(command.Id);
        if (bucket == null)
        {
            return OperationResult<BucketDto>.MakeFailure(ErrorMessage.Create("UPDATE_BUCKET", "Bucket not found"));
        }

        var updateResult = bucket.Update(command.Name, command.Description, command.DefaultLimit);
        if (!updateResult.Success)
        {
            return OperationResult<BucketDto>.MakeFailure(updateResult.Errors);
        }

        await _bucketRepository.UpdateAsync(bucket);

        scope.Complete();
        return OperationResult<BucketDto>.MakeSuccess(_mapper.ToDto(bucket));
    }
}

public class DeleteBucketCommandHandler(IBucketRepository bucketRepository, IMonthlyBucketRepository monthlyBucketRepository, ILogger<DeleteBucketCommandHandler>? logger = null)
{
    private readonly IBucketRepository _bucketRepository = bucketRepository;
    private readonly IMonthlyBucketRepository _monthlyBucketRepository = monthlyBucketRepository;
    private readonly ILogger<DeleteBucketCommandHandler>? _logger = logger;

    public async Task Handle(DeleteBucketCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var bucket = await _bucketRepository.LoadAsync(command.Id);
        if (bucket == null)
        {
            throw new InvalidOperationException("Bucket not found");
        }

        var hasRelatedMonthlyBuckets = _monthlyBucketRepository.Any(mb => mb.Bucket.Identity == bucket.Identity);
        if (hasRelatedMonthlyBuckets)
        {
            var disableResult = bucket.Disable();
            if (!disableResult.Success)
            {
                throw new InvalidOperationException(string.Join(", ", disableResult.Errors.Select(e => e.Description)));
            }
            await _bucketRepository.UpdateAsync(bucket);
        }
        else
        {
            await _bucketRepository.RemoveAsync(bucket);
        }

        scope.Complete();
    }
}

public class EnableBucketCommandHandler(IBucketRepository bucketRepository, ILogger<EnableBucketCommandHandler>? logger = null)
{
    private readonly IBucketRepository _bucketRepository = bucketRepository;
    private readonly ILogger<EnableBucketCommandHandler>? _logger = logger;
    private readonly BucketMapper _mapper = new BucketMapper();

    public async Task<BucketDto> Handle(EnableBucketCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var bucket = await _bucketRepository.LoadAsync(command.Id);
        if (bucket == null)
        {
            throw new InvalidOperationException("Bucket not found");
        }

        var enableResult = bucket.Enable();
        if (!enableResult.Success)
        {
            throw new InvalidOperationException(string.Join(", ", enableResult.Errors.Select(e => e.Description)));
        }

        await _bucketRepository.UpdateAsync(bucket);
        scope.Complete();
        return _mapper.ToDto(bucket);
    }
}

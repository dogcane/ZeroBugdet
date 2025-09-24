using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Commands;

public class BucketCommandHandlers
{
    private readonly IBucketRepository _bucketRepository;

    public BucketCommandHandlers(IBucketRepository bucketRepository)
    {
        _bucketRepository = bucketRepository;
    }

    public async Task<OperationResult<BucketDto>> Handle(CreateBucketCommand command)
    {
        var bucketResult = Bucket.Create(command.Name, command.Description, command.DefaultLimit);
        if (!bucketResult.IsSuccess)
            return OperationResult<BucketDto>.MakeFailure(bucketResult.Errors);

        var bucket = bucketResult.Value!;
        await _bucketRepository.AddAsync(bucket);

        return OperationResult<BucketDto>.MakeSuccess(new BucketDto(
            bucket.Identity,
            bucket.Name,
            bucket.Description,
            bucket.DefaultLimit,
            bucket.DefaultBalance
        ));
    }

    public async Task<OperationResult<BucketDto>> Handle(UpdateBucketCommand command)
    {
        var bucket = await _bucketRepository.GetByIdAsync(command.Id);
        if (bucket == null)
            return OperationResult<BucketDto>.MakeFailure("Bucket not found");

        bucket.Update(command.Name, command.Description, command.DefaultLimit);
        await _bucketRepository.UpdateAsync(bucket);

        return OperationResult<BucketDto>.MakeSuccess(new BucketDto(
            bucket.Identity,
            bucket.Name,
            bucket.Description,
            bucket.DefaultLimit,
            bucket.DefaultBalance
        ));
    }

    public async Task<OperationResult> Handle(DeleteBucketCommand command)
    {
        var bucket = await _bucketRepository.GetByIdAsync(command.Id);
        if (bucket == null)
            return OperationResult.MakeFailure("Bucket not found");

        await _bucketRepository.DeleteAsync(bucket);
        return OperationResult.MakeSuccess();
    }
}
using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Commands;

public class MonthlyBucketCommandHandlers
{
    private readonly IMonthlyBucketRepository _monthlyBucketRepository;
    private readonly IBucketRepository _bucketRepository;

    public MonthlyBucketCommandHandlers(
        IMonthlyBucketRepository monthlyBucketRepository,
        IBucketRepository bucketRepository)
    {
        _monthlyBucketRepository = monthlyBucketRepository;
        _bucketRepository = bucketRepository;
    }

    public async Task<OperationResult<MonthlyBucketDto>> Handle(CreateMonthlyBucketCommand command)
    {
        var bucket = await _bucketRepository.GetByIdAsync(command.BucketId);
        if (bucket == null)
            return OperationResult<MonthlyBucketDto>.MakeFailure("Bucket not found");

        var monthlyBucket = bucket.CreateMonthly(command.Year, command.Month);
        await _monthlyBucketRepository.AddAsync(monthlyBucket);

        return OperationResult<MonthlyBucketDto>.MakeSuccess(new MonthlyBucketDto(
            monthlyBucket.Identity,
            monthlyBucket.Year,
            monthlyBucket.Month,
            monthlyBucket.Balance,
            monthlyBucket.Description,
            monthlyBucket.Limit,
            monthlyBucket.Bucket.Identity
        ));
    }

    public async Task<OperationResult<MonthlyBucketDto>> Handle(UpdateMonthlyBucketCommand command)
    {
        var monthlyBucket = await _monthlyBucketRepository.GetByIdAsync(command.Id);
        if (monthlyBucket == null)
            return OperationResult<MonthlyBucketDto>.MakeFailure("Monthly bucket not found");

        // Update logic would need to be implemented in the domain model
        // For now, we'll assume these properties can be updated directly
        // This may need adjustment based on business rules
        
        await _monthlyBucketRepository.UpdateAsync(monthlyBucket);

        return OperationResult<MonthlyBucketDto>.MakeSuccess(new MonthlyBucketDto(
            monthlyBucket.Identity,
            monthlyBucket.Year,
            monthlyBucket.Month,
            monthlyBucket.Balance,
            monthlyBucket.Description,
            monthlyBucket.Limit,
            monthlyBucket.Bucket.Identity
        ));
    }

    public async Task<OperationResult> Handle(DeleteMonthlyBucketCommand command)
    {
        var monthlyBucket = await _monthlyBucketRepository.GetByIdAsync(command.Id);
        if (monthlyBucket == null)
            return OperationResult.MakeFailure("Monthly bucket not found");

        await _monthlyBucketRepository.DeleteAsync(monthlyBucket);
        return OperationResult.MakeSuccess();
    }
}
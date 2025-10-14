using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Commands;

public class MonthlySpendingCommandHandlers(
    IMonthlySpendingRepository monthlySpendingRepository,
    IMonthlyBucketRepository monthlyBucketRepository,
    ITagService tagService)
{
    private readonly IMonthlySpendingRepository _monthlySpendingRepository = monthlySpendingRepository;
    private readonly IMonthlyBucketRepository _monthlyBucketRepository = monthlyBucketRepository;
    private readonly ITagService _tagService = tagService;


    public async Task<OperationResult<MonthlySpendingDto>> Handle(CreateMonthlySpendingCommand command)
    {
        var monthlyBucket = await _monthlyBucketRepository.LoadAsync(command.MonthlyBucketId);
        if (monthlyBucket == null)
            return OperationResult<MonthlySpendingDto>.MakeFailure(ErrorMessage.Create(nameof(command.MonthlyBucketId), "MONTHLY_BUCKET_NOT_FOUND"));

        // Ensure all tags exist by name, creating new ones if needed
        var tags = await _tagService.EnsureTagsByNameAsync(command.TagNames);

        var monthlySpendingResult = MonthlySpending.Create(
            command.Date,
            command.Description,
            command.Amount,
            command.Owner,
            tags.ToTagNames(),
            monthlyBucket);

        if (!monthlySpendingResult.Success)
            return OperationResult<MonthlySpendingDto>.MakeFailure(monthlySpendingResult.Errors);

        var monthlySpending = monthlySpendingResult.Value!;
        await _monthlySpendingRepository.AddAsync(monthlySpending);

        return OperationResult<MonthlySpendingDto>.MakeSuccess(new MonthlySpendingDto(
            monthlySpending.Identity,
            monthlySpending.Date,
            monthlySpending.MonthlyBucketId,
            monthlySpending.Description,
            monthlySpending.Amount,
            monthlySpending.Owner,
            monthlySpending.Tags
        ));
    }

    public async Task<OperationResult<MonthlySpendingDto>> Handle(UpdateMonthlySpendingCommand command)
    {
        var monthlySpending = await _monthlySpendingRepository.LoadAsync(command.Id);
        if (monthlySpending == null)
            return OperationResult<MonthlySpendingDto>.MakeFailure(ErrorMessage.Create(nameof(command.Id), "MONTHLY_SPENDING_NOT_FOUND"));

        // Ensure all tags exist by name, creating new ones if needed
        var tags = await _tagService.EnsureTagsByNameAsync(command.TagNames);

        monthlySpending.Update(
            command.Date,
            command.Description,
            command.Amount,
            command.Owner,
            tags.ToTagNames());

        await _monthlySpendingRepository.UpdateAsync(monthlySpending);

        return OperationResult<MonthlySpendingDto>.MakeSuccess(new MonthlySpendingDto(
            monthlySpending.Identity,
            monthlySpending.Date,
            monthlySpending.MonthlyBucketId,
            monthlySpending.Description,
            monthlySpending.Amount,
            monthlySpending.Owner,
            monthlySpending.Tags
        ));
    }

    public async Task<OperationResult> Handle(DeleteMonthlySpendingCommand command)
    {
        var monthlySpending = await _monthlySpendingRepository.LoadAsync(command.Id);
        if (monthlySpending == null)
            return OperationResult.MakeFailure(ErrorMessage.Create(nameof(command.Id), "MONTHLY_SPENDING_NOT_FOUND"));

        await _monthlySpendingRepository.RemoveAsync(monthlySpending);
        return OperationResult.MakeSuccess();
    }
}
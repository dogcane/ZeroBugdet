using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Commands;

public class MonthlySpendingCommandHandlers
{
    private readonly IMonthlySpendingRepository _monthlySpendingRepository;
    private readonly IMonthlyBucketRepository _monthlyBucketRepository;
    private readonly ITagRepository _tagRepository;

    public MonthlySpendingCommandHandlers(
        IMonthlySpendingRepository monthlySpendingRepository,
        IMonthlyBucketRepository monthlyBucketRepository,
        ITagRepository tagRepository)
    {
        _monthlySpendingRepository = monthlySpendingRepository;
        _monthlyBucketRepository = monthlyBucketRepository;
        _tagRepository = tagRepository;
    }

    #region Helper Methods
    /// <summary>
    /// Ensures all tag names exist in the repository by name, creating new ones if needed.
    /// This is used when tags are referenced by name rather than ID.
    /// </summary>
    private async Task<List<Tag>> EnsureTagsByNameAsync(string[] tagNames)
    {
        var tags = new List<Tag>();
        
        foreach (var tagName in tagNames)
        {
            var tag = await _tagRepository.GetByNameAsync(tagName);
            if (tag == null)
            {
                // Create new tag if it doesn't exist
                var tagResult = Tag.Create(tagName, $"Auto-created tag: {tagName}");
                if (tagResult.Success)
                {
                    tag = tagResult.Value!;
                    await _tagRepository.AddAsync(tag);
                    tags.Add(tag);
                }
            }
            else
            {
                tags.Add(tag);
            }
        }
        
        return tags;
    }
    #endregion

    public async Task<OperationResult<MonthlySpendingDto>> Handle(CreateMonthlySpendingCommand command)
    {
        var monthlyBucket = await _monthlyBucketRepository.GetByIdAsync(command.MonthlyBucketId);
        if (monthlyBucket == null)
            return OperationResult<MonthlySpendingDto>.MakeFailure("Monthly bucket not found");

        // Ensure all tags exist by name, creating new ones if needed
        var tags = await EnsureTagsByNameAsync(command.TagNames);

        var monthlySpendingResult = Spending.Create(
            command.Date,
            command.Description,
            command.Amount,
            command.Owner,
            tags.ToArray(),
            monthlyBucket);

        if (!monthlySpendingResult.IsSuccess)
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
        var monthlySpending = await _monthlySpendingRepository.GetByIdAsync(command.Id);
        if (monthlySpending == null)
            return OperationResult<MonthlySpendingDto>.MakeFailure("Monthly spending not found");

        // Ensure all tags exist by name, creating new ones if needed
        var tags = await EnsureTagsByNameAsync(command.TagNames);

        monthlySpending.Update(
            command.Date,
            command.Description,
            command.Amount,
            command.Owner,
            tags.ToArray());

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
        var monthlySpending = await _monthlySpendingRepository.GetByIdAsync(command.Id);
        if (monthlySpending == null)
            return OperationResult.MakeFailure("Monthly spending not found");

        await _monthlySpendingRepository.DeleteAsync(monthlySpending);
        return OperationResult.MakeSuccess();
    }
}
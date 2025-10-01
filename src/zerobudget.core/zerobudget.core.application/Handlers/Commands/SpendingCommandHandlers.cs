using System.Transactions;
using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Commands;

/// <summary>
/// Command handlers for Spending operations.
/// These handlers are now protected by the GlobalExceptionMiddleware,
/// so unhandled exceptions will be automatically caught and converted to OperationResult failures.
/// </summary>
public class SpendingCommandHandlers(
    ISpendingRepository spendingRepository,
    IBucketRepository bucketRepository,
    ITagRepository tagRepository,
    IMonthlySpendingRepository monthlySpendingRepository)
{
    #region Fields
    private readonly ISpendingRepository _spendingRepository = spendingRepository;
    private readonly IBucketRepository _bucketRepository = bucketRepository;
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly IMonthlySpendingRepository _monthlySpendingRepository = monthlySpendingRepository;
    #endregion

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
                var tagResult = Tag.Create(tagName);
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

    public async Task<OperationResult<SpendingDto>> Handle(CreateSpendingCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var bucket = await _bucketRepository.LoadAsync(command.BucketId);
        if (bucket == null)
            return OperationResult.MakeFailure(ErrorMessage.Create("Bucket", "Bucket not found"));

        // Ensure all tags exist by name, creating new ones if needed
        var tags = await EnsureTagsByNameAsync(command.TagNames);

        var spendingResult = Spending.Create(
            command.Description,
            command.Amount,
            command.Owner,
            [.. tags],
            bucket);

        if (!spendingResult.Success)
            return OperationResult<SpendingDto>.MakeFailure(spendingResult.Errors);

        var spending = spendingResult.Value!;
        await _spendingRepository.AddAsync(spending);

        scope.Complete();
        return OperationResult<SpendingDto>.MakeSuccess(new SpendingDto(
            spending.Identity,
            spending.BucketId,
            spending.Description,
            spending.Amount,
            spending.Owner,
            spending.Tags
        ));
    }

    public async Task<OperationResult<SpendingDto>> Handle(UpdateSpendingCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        var spending = await _spendingRepository.LoadAsync(command.Id);
        if (spending == null)
            return OperationResult<SpendingDto>.MakeFailure(ErrorMessage.Create("Spending", "Spending not found"));

        // Ensure all tags exist by name, creating new ones if needed
        var tags = await EnsureTagsByNameAsync(command.TagNames);

        var updateResult = spending.Update(
            command.Description,
            command.Amount,
            command.Owner,
            [.. tags]);

        if (!updateResult.Success)
            return OperationResult<SpendingDto>.MakeFailure(updateResult.Errors);

        await _spendingRepository.UpdateAsync(spending);

        scope.Complete();
        return OperationResult<SpendingDto>.MakeSuccess(new SpendingDto(
            spending.Identity,
            spending.BucketId,
            spending.Description,
            spending.Amount,
            spending.Owner,
            spending.Tags
        ));
    }

    public async Task<OperationResult> Handle(DeleteSpendingCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var spending = await _spendingRepository.LoadAsync(command.Id);
        if (spending == null)
            return OperationResult.MakeFailure(ErrorMessage.Create("Spending", "Spending not found"));

        // Check if there are related MonthlySpending records based on matching criteria
        var hasRelatedMonthlySpendings = _monthlySpendingRepository.Any(ms => 
            ms.Description == spending.Description &&
            ms.Amount == spending.Amount &&
            ms.Owner == spending.Owner);

        if (hasRelatedMonthlySpendings)
        {
            spending.Disable();
            await _spendingRepository.UpdateAsync(spending);
        }
        else
        {
            await _spendingRepository.RemoveAsync(spending);
        }

        scope.Complete();
        return OperationResult.MakeSuccess();
    }

    public async Task<OperationResult> Handle(EnableSpendingCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var spending = await _spendingRepository.LoadAsync(command.Id);
        if (spending == null)
            return OperationResult.MakeFailure(ErrorMessage.Create("Spending", "Spending not found"));

        spending.Enable();
        await _spendingRepository.UpdateAsync(spending);
        scope.Complete();
        return OperationResult.MakeSuccess();
    }
}
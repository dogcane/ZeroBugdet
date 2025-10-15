using System.Transactions;
using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;

namespace zerobudget.core.application.Handlers.Commands;

/// <summary>
/// Command handlers for Spending operations.
/// These handlers are now protected by the GlobalExceptionMiddleware,
/// so unhandled exceptions will be automatically caught and converted to OperationResult failures.
/// </summary>
public class SpendingCommandHandlers(
    ISpendingRepository spendingRepository,
    IBucketRepository bucketRepository,
    ITagService tagService,
    IMonthlySpendingRepository monthlySpendingRepository)
{
    #region Fields
    private readonly ISpendingRepository _spendingRepository = spendingRepository;
    private readonly IBucketRepository _bucketRepository = bucketRepository;
    private readonly ITagService _tagService = tagService;
    private readonly IMonthlySpendingRepository _monthlySpendingRepository = monthlySpendingRepository;
    private readonly SpendingMapper _mapper = new SpendingMapper();
    #endregion

    public async Task<OperationResult<SpendingDto>> Handle(CreateSpendingCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var bucket = await _bucketRepository.LoadAsync(command.BucketId);
        if (bucket == null)
            return OperationResult.MakeFailure(ErrorMessage.Create("Bucket", "Bucket not found"));

    // Ensure all tags exist by name, creating new ones if needed
    var tags = await _tagService.EnsureTagsByNameAsync(command.TagNames);

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
        return OperationResult<SpendingDto>.MakeSuccess(_mapper.ToDto(spending));
    }

    public async Task<OperationResult<SpendingDto>> Handle(UpdateSpendingCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        var spending = await _spendingRepository.LoadAsync(command.Id);
        if (spending == null)
            return OperationResult<SpendingDto>.MakeFailure(ErrorMessage.Create("Spending", "Spending not found"));

    // Ensure all tags exist by name, creating new ones if needed
    var tags = await _tagService.EnsureTagsByNameAsync(command.TagNames);

        var updateResult = spending.Update(
            command.Description,
            command.Amount,
            command.Owner,
            [.. tags]);

        if (!updateResult.Success)
            return OperationResult<SpendingDto>.MakeFailure(updateResult.Errors);

        await _spendingRepository.UpdateAsync(spending);

        scope.Complete();
        return OperationResult<SpendingDto>.MakeSuccess(_mapper.ToDto(spending));
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
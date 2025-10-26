using System.Transactions;
using Microsoft.Extensions.Logging;
using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;

namespace zerobudget.core.application.Handlers.Commands;

public class CreateSpendingCommandHandler(
    ISpendingRepository spendingRepository,
    IBucketRepository bucketRepository,
    ITagService tagService,
    ILogger<CreateSpendingCommandHandler>? logger = null)
{
    private readonly SpendingMapper _mapper = new();

    public async Task<OperationResult<SpendingDto>> Handle(CreateSpendingCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var bucket = await bucketRepository.LoadAsync(command.BucketId);
        if (bucket == null)
            return OperationResult<SpendingDto>.MakeFailure(ErrorMessage.Create("CREATE_SPENDING", "Bucket not found"));

        var tags = await tagService.EnsureTagsByNameAsync(command.TagNames);

        var spendingResult = Spending.Create(
            command.Description,
            command.Amount,
            command.Owner,
            [.. tags],
            bucket);

        if (!spendingResult.Success)
            return OperationResult<SpendingDto>.MakeFailure(spendingResult.Errors);

        var spending = spendingResult.Value!;
        await spendingRepository.AddAsync(spending);

        scope.Complete();
        return OperationResult<SpendingDto>.MakeSuccess(_mapper.ToDto(spending));
    }
}

public class UpdateSpendingCommandHandler(
    ISpendingRepository spendingRepository,
    ITagService tagService,
    ILogger<UpdateSpendingCommandHandler>? logger = null)
{
    private readonly SpendingMapper _mapper = new();

    public async Task<OperationResult<SpendingDto>> Handle(UpdateSpendingCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var spending = await spendingRepository.LoadAsync(command.Id);
        if (spending == null)
            return OperationResult<SpendingDto>.MakeFailure(ErrorMessage.Create("UPDATE_SPENDING", "Spending not found"));

        var tags = await tagService.EnsureTagsByNameAsync(command.TagNames);

        var updateResult = spending.Update(
            command.Description,
            command.Amount,
            command.Owner,
            [.. tags]);

        if (!updateResult.Success)
            return OperationResult<SpendingDto>.MakeFailure(updateResult.Errors);

        await spendingRepository.UpdateAsync(spending);

        scope.Complete();
        return OperationResult<SpendingDto>.MakeSuccess(_mapper.ToDto(spending));
    }
}

public class DeleteSpendingCommandHandler(
    ISpendingRepository spendingRepository,
    IMonthlySpendingRepository monthlySpendingRepository,
    ILogger<DeleteSpendingCommandHandler>? logger = null)
{
    public async Task<OperationResult> Handle(DeleteSpendingCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var spending = await spendingRepository.LoadAsync(command.Id);
        if (spending == null)
            return OperationResult.MakeFailure(ErrorMessage.Create("DELETE_SPENDING", "Spending not found"));

        var hasRelatedMonthlySpendings = monthlySpendingRepository.Any(ms =>
            ms.Description == spending.Description &&
            ms.Amount == spending.Amount &&
            ms.Owner == spending.Owner);

        if (hasRelatedMonthlySpendings)
        {
            var disableResult = spending.Disable();
            if (!disableResult.Success)
            {
                return OperationResult.MakeFailure(disableResult.Errors);
            }
            await spendingRepository.UpdateAsync(spending);
        }
        else
        {
            await spendingRepository.RemoveAsync(spending);
        }

        scope.Complete();
        return OperationResult.MakeSuccess();
    }
}

public class EnableSpendingCommandHandler(
    ISpendingRepository spendingRepository,
    ILogger<EnableSpendingCommandHandler>? logger = null)
{
    private readonly SpendingMapper _mapper = new();

    public async Task<OperationResult<SpendingDto>> Handle(EnableSpendingCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var spending = await spendingRepository.LoadAsync(command.Id);
        if (spending == null)
            return OperationResult<SpendingDto>.MakeFailure(ErrorMessage.Create("ENABLE_SPENDING", "Spending not found"));

        var enableResult = spending.Enable();
        if (!enableResult.Success)
        {
            return OperationResult<SpendingDto>.MakeFailure(enableResult.Errors);
        }

        await spendingRepository.UpdateAsync(spending);

        scope.Complete();
        return OperationResult<SpendingDto>.MakeSuccess(_mapper.ToDto(spending));
    }
}

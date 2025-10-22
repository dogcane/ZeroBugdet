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
    private readonly ISpendingRepository _spendingRepository = spendingRepository;
    private readonly IBucketRepository _bucketRepository = bucketRepository;
    private readonly ITagService _tagService = tagService;
    private readonly ILogger<CreateSpendingCommandHandler>? _logger = logger;
    private readonly SpendingMapper _mapper = new SpendingMapper();

    public async Task<SpendingDto> Handle(CreateSpendingCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var bucket = await _bucketRepository.LoadAsync(command.BucketId);
        if (bucket == null)
            throw new InvalidOperationException("Bucket not found");

        var tags = await _tagService.EnsureTagsByNameAsync(command.TagNames);

        var spendingResult = Spending.Create(
            command.Description,
            command.Amount,
            command.Owner,
            [.. tags],
            bucket);

        if (!spendingResult.Success)
            throw new InvalidOperationException(string.Join(", ", spendingResult.Errors.Select(e => e.Description)));

        var spending = spendingResult.Value!;
        await _spendingRepository.AddAsync(spending);

        scope.Complete();
        return _mapper.ToDto(spending);
    }
}

public class UpdateSpendingCommandHandler(
    ISpendingRepository spendingRepository,
    ITagService tagService,
    ILogger<UpdateSpendingCommandHandler>? logger = null)
{
    private readonly ISpendingRepository _spendingRepository = spendingRepository;
    private readonly ITagService _tagService = tagService;
    private readonly ILogger<UpdateSpendingCommandHandler>? _logger = logger;
    private readonly SpendingMapper _mapper = new SpendingMapper();

    public async Task<SpendingDto> Handle(UpdateSpendingCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var spending = await _spendingRepository.LoadAsync(command.Id);
        if (spending == null)
            throw new InvalidOperationException("Spending not found");

        var tags = await _tagService.EnsureTagsByNameAsync(command.TagNames);

        var updateResult = spending.Update(
            command.Description,
            command.Amount,
            command.Owner,
            [.. tags]);

        if (!updateResult.Success)
            throw new InvalidOperationException(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

        await _spendingRepository.UpdateAsync(spending);

        scope.Complete();
        return _mapper.ToDto(spending);
    }
}

public class DeleteSpendingCommandHandler(
    ISpendingRepository spendingRepository,
    IMonthlySpendingRepository monthlySpendingRepository,
    ILogger<DeleteSpendingCommandHandler>? logger = null)
{
    private readonly ISpendingRepository _spendingRepository = spendingRepository;
    private readonly IMonthlySpendingRepository _monthlySpendingRepository = monthlySpendingRepository;
    private readonly ILogger<DeleteSpendingCommandHandler>? _logger = logger;

    public async Task Handle(DeleteSpendingCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var spending = await _spendingRepository.LoadAsync(command.Id);
        if (spending == null)
            throw new InvalidOperationException("Spending not found");

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
    }
}

public class EnableSpendingCommandHandler(
    ISpendingRepository spendingRepository,
    ILogger<EnableSpendingCommandHandler>? logger = null)
{
    private readonly ISpendingRepository _spendingRepository = spendingRepository;
    private readonly ILogger<EnableSpendingCommandHandler>? _logger = logger;
    private readonly SpendingMapper _mapper = new SpendingMapper();

    public async Task<SpendingDto> Handle(EnableSpendingCommand command)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var spending = await _spendingRepository.LoadAsync(command.Id);
        if (spending == null)
            throw new InvalidOperationException("Spending not found");

        spending.Enable();
        await _spendingRepository.UpdateAsync(spending);

        scope.Complete();
        return _mapper.ToDto(spending);
    }
}

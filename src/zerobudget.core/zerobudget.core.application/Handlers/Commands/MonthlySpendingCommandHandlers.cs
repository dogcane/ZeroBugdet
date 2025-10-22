using Microsoft.Extensions.Logging;
using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;

namespace zerobudget.core.application.Handlers.Commands;

public class CreateMonthlySpendingCommandHandler(
    IMonthlySpendingRepository monthlySpendingRepository,
    IMonthlyBucketRepository monthlyBucketRepository,
    ITagService tagService,
    ILogger<CreateMonthlySpendingCommandHandler>? logger = null)
{
    private readonly IMonthlySpendingRepository _monthlySpendingRepository = monthlySpendingRepository;
    private readonly IMonthlyBucketRepository _monthlyBucketRepository = monthlyBucketRepository;
    private readonly ITagService _tagService = tagService;
    private readonly ILogger<CreateMonthlySpendingCommandHandler>? _logger = logger;
    private readonly MonthlySpendingMapper _mapper = new MonthlySpendingMapper();

    public async Task<MonthlySpendingDto> Handle(CreateMonthlySpendingCommand command)
    {
        var monthlyBucket = await _monthlyBucketRepository.LoadAsync(command.MonthlyBucketId);
        if (monthlyBucket == null)
            throw new InvalidOperationException("Monthly bucket not found");

        var tags = await _tagService.EnsureTagsByNameAsync(command.TagNames);

        var monthlySpendingResult = MonthlySpending.Create(
            command.Date,
            command.Description,
            command.Amount,
            command.Owner,
            tags.ToTagNames(),
            monthlyBucket);

        if (!monthlySpendingResult.Success)
            throw new InvalidOperationException(string.Join(", ", monthlySpendingResult.Errors.Select(e => e.Description)));

        var monthlySpending = monthlySpendingResult.Value!;
        await _monthlySpendingRepository.AddAsync(monthlySpending);

        return _mapper.ToDto(monthlySpending);
    }
}

public class UpdateMonthlySpendingCommandHandler(
    IMonthlySpendingRepository monthlySpendingRepository,
    ITagService tagService,
    ILogger<UpdateMonthlySpendingCommandHandler>? logger = null)
{
    private readonly IMonthlySpendingRepository _monthlySpendingRepository = monthlySpendingRepository;
    private readonly ITagService _tagService = tagService;
    private readonly ILogger<UpdateMonthlySpendingCommandHandler>? _logger = logger;
    private readonly MonthlySpendingMapper _mapper = new MonthlySpendingMapper();

    public async Task<MonthlySpendingDto> Handle(UpdateMonthlySpendingCommand command)
    {
        var monthlySpending = await _monthlySpendingRepository.LoadAsync(command.Id);
        if (monthlySpending == null)
            throw new InvalidOperationException("Monthly spending not found");

        var tags = await _tagService.EnsureTagsByNameAsync(command.TagNames);

        monthlySpending.Update(
            command.Date,
            command.Description,
            command.Amount,
            command.Owner,
            tags.ToTagNames());

        await _monthlySpendingRepository.UpdateAsync(monthlySpending);

        return _mapper.ToDto(monthlySpending);
    }
}

public class DeleteMonthlySpendingCommandHandler(
    IMonthlySpendingRepository monthlySpendingRepository,
    ILogger<DeleteMonthlySpendingCommandHandler>? logger = null)
{
    private readonly IMonthlySpendingRepository _monthlySpendingRepository = monthlySpendingRepository;
    private readonly ILogger<DeleteMonthlySpendingCommandHandler>? _logger = logger;

    public async Task Handle(DeleteMonthlySpendingCommand command)
    {
        var monthlySpending = await _monthlySpendingRepository.LoadAsync(command.Id);
        if (monthlySpending == null)
            throw new InvalidOperationException("Monthly spending not found");

        await _monthlySpendingRepository.RemoveAsync(monthlySpending);
    }
}

using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Commands;

public class SpendingCommandHandlers
{
    private readonly ISpendingRepository _spendingRepository;
    private readonly IBucketRepository _bucketRepository;
    private readonly ITagRepository _tagRepository;

    public SpendingCommandHandlers(
        ISpendingRepository spendingRepository,
        IBucketRepository bucketRepository,
        ITagRepository tagRepository)
    {
        _spendingRepository = spendingRepository;
        _bucketRepository = bucketRepository;
        _tagRepository = tagRepository;
    }

    public async Task<OperationResult<SpendingDto>> Handle(CreateSpendingCommand command)
    {
        var bucket = await _bucketRepository.GetByIdAsync(command.BucketId);
        if (bucket == null)
            return OperationResult<SpendingDto>.MakeFailure("Bucket not found");

        var tags = new List<Tag>();
        foreach (var tagId in command.TagIds)
        {
            var tag = await _tagRepository.GetByIdAsync(tagId);
            if (tag != null)
                tags.Add(tag);
        }

        var spendingResult = Spending.Create(
            command.Date,
            command.Description,
            command.Amount,
            command.Owner,
            tags.ToArray(),
            bucket);

        if (!spendingResult.IsSuccess)
            return OperationResult<SpendingDto>.MakeFailure(spendingResult.Errors);

        var spending = spendingResult.Value!;
        await _spendingRepository.AddAsync(spending);

        return OperationResult<SpendingDto>.MakeSuccess(new SpendingDto(
            spending.Identity,
            spending.Date,
            spending.BucketId,
            spending.Description,
            spending.Amount,
            spending.Owner,
            spending.Tags.Select(t => t.Identity).ToArray()
        ));
    }

    public async Task<OperationResult<SpendingDto>> Handle(UpdateSpendingCommand command)
    {
        var spending = await _spendingRepository.GetByIdAsync(command.Id);
        if (spending == null)
            return OperationResult<SpendingDto>.MakeFailure("Spending not found");

        var tags = new List<Tag>();
        foreach (var tagId in command.TagIds)
        {
            var tag = await _tagRepository.GetByIdAsync(tagId);
            if (tag != null)
                tags.Add(tag);
        }

        var updateResult = spending.Update(
            command.Date,
            command.Description,
            command.Amount,
            command.Owner,
            tags.ToArray());

        if (!updateResult.IsSuccess)
            return OperationResult<SpendingDto>.MakeFailure(updateResult.Errors);

        await _spendingRepository.UpdateAsync(spending);

        return OperationResult<SpendingDto>.MakeSuccess(new SpendingDto(
            spending.Identity,
            spending.Date,
            spending.BucketId,
            spending.Description,
            spending.Amount,
            spending.Owner,
            spending.Tags.Select(t => t.Identity).ToArray()
        ));
    }

    public async Task<OperationResult> Handle(DeleteSpendingCommand command)
    {
        var spending = await _spendingRepository.GetByIdAsync(command.Id);
        if (spending == null)
            return OperationResult.MakeFailure("Spending not found");

        await _spendingRepository.DeleteAsync(spending);
        return OperationResult.MakeSuccess();
    }
}
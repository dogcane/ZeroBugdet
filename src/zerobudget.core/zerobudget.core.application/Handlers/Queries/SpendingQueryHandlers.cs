using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Queries;

public class SpendingQueryHandlers
{
    private readonly ISpendingRepository _spendingRepository;

    public SpendingQueryHandlers(ISpendingRepository spendingRepository)
    {
        _spendingRepository = spendingRepository;
    }

    public async Task<SpendingDto?> Handle(GetSpendingByIdQuery query)
    {
        var spending = await _spendingRepository.LoadAsync(query.Id);
        if (spending == null)
            return null;

        return new SpendingDto(
            spending.Identity,
            spending.Date,
            spending.BucketId,
            spending.Description,
            spending.Amount,
            spending.Owner,
            spending.Tags.Select(t => t.Identity).ToArray()
        );
    }

    public async Task<IEnumerable<SpendingDto>> Handle(GetAllSpendingsQuery query)
    {
        var spendings = await _spendingRepository.GetAllAsync();
        return spendings.Select(spending => new SpendingDto(
            spending.Identity,
            spending.Date,
            spending.BucketId,
            spending.Description,
            spending.Amount,
            spending.Owner,
            spending.Tags.Select(t => t.Identity).ToArray()
        ));
    }

    public async Task<IEnumerable<SpendingDto>> Handle(GetSpendingsByBucketIdQuery query)
    {
        var spendings = await _spendingRepository.GetAllAsync();
        var filteredSpendings = spendings.Where(s => s.BucketId == query.BucketId);
        
        return filteredSpendings.Select(spending => new SpendingDto(
            spending.Identity,
            spending.Date,
            spending.BucketId,
            spending.Description,
            spending.Amount,
            spending.Owner,
            spending.Tags.Select(t => t.Identity).ToArray()
        ));
    }

    public async Task<IEnumerable<SpendingDto>> Handle(GetSpendingsByDateRangeQuery query)
    {
        var spendings = await _spendingRepository.GetAllAsync();
        var filteredSpendings = spendings.Where(s => s.Date >= query.StartDate && s.Date <= query.EndDate);
        
        return filteredSpendings.Select(spending => new SpendingDto(
            spending.Identity,
            spending.Date,
            spending.BucketId,
            spending.Description,
            spending.Amount,
            spending.Owner,
            spending.Tags.Select(t => t.Identity).ToArray()
        ));
    }

    public async Task<IEnumerable<SpendingDto>> Handle(GetSpendingsByOwnerQuery query)
    {
        var spendings = await _spendingRepository.GetAllAsync();
        var filteredSpendings = spendings.Where(s => s.Owner.Equals(query.Owner, StringComparison.OrdinalIgnoreCase));
        
        return filteredSpendings.Select(spending => new SpendingDto(
            spending.Identity,
            spending.Date,
            spending.BucketId,
            spending.Description,
            spending.Amount,
            spending.Owner,
            spending.Tags.Select(t => t.Identity).ToArray()
        ));
    }
}
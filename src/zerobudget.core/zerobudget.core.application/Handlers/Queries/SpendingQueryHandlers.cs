using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;
using Microsoft.Extensions.Logging;

namespace zerobudget.core.application.Handlers.Queries;

public class SpendingQueryHandlers(ISpendingRepository spendingRepository, ILogger<SpendingQueryHandlers>? logger = null)
{
    private readonly ISpendingRepository _spendingRepository = spendingRepository;
    private readonly ILogger<SpendingQueryHandlers>? _logger = logger;
    private readonly SpendingMapper _mapper = new();

    public async Task<SpendingDto?> Handle(GetSpendingByIdQuery query)
    {
        var spending = await _spendingRepository.LoadAsync(query.Id);
        if (spending == null)
            return null;
        return _mapper.ToDto(spending);
    }

    public async Task<IEnumerable<SpendingDto>> Handle(GetAllSpendingsQuery query)
    {
        var spendings = _spendingRepository.AsQueryable();
        return await Task.FromResult(spendings.Select(_mapper.ToDto));
    }

    public async Task<IEnumerable<SpendingDto>> Handle(GetSpendingsByBucketIdQuery query)
    {
        var spendings = _spendingRepository.AsQueryable();
        var filteredSpendings = spendings.Where(s => s.BucketId == query.BucketId);
        return await Task.FromResult(filteredSpendings.Select(_mapper.ToDto));
    }

    public async Task<IEnumerable<SpendingDto>> Handle(GetSpendingsByOwnerQuery query)
    {
        var spendings = _spendingRepository.AsQueryable();
        var filteredSpendings = spendings.Where(s => s.Owner.Equals(query.Owner, StringComparison.OrdinalIgnoreCase));
        return await Task.FromResult(filteredSpendings.Select(_mapper.ToDto));
    }
}
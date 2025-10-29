using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;
using Microsoft.Extensions.Logging;

namespace zerobudget.core.application.Handlers.Queries;

public class GetSpendingByIdQueryHandler(ISpendingRepository spendingRepository, ILogger<GetSpendingByIdQueryHandler>? logger = null)
{
    private readonly SpendingMapper _mapper = new();

    public async Task<SpendingDto?> Handle(GetSpendingByIdQuery query)
    {
        var spending = await spendingRepository.LoadAsync(query.Id);
        if (spending == null)
            return null;
        return _mapper.ToDto(spending);
    }
}

public class GetAllSpendingsQueryHandler(ISpendingRepository spendingRepository, ILogger<GetAllSpendingsQueryHandler>? logger = null)
{
    private readonly SpendingMapper _mapper = new();

    public async Task<IEnumerable<SpendingDto>> Handle(GetAllSpendingsQuery query)
    {
        var spendings = spendingRepository.AsQueryable();
        var result = spendings.Select(_mapper.ToDto).ToArray();
        return await Task.FromResult(result);
    }
}

public class GetSpendingsByBucketIdQueryHandler(ISpendingRepository spendingRepository, ILogger<GetSpendingsByBucketIdQueryHandler>? logger = null)
{
    private readonly SpendingMapper _mapper = new();

    public async Task<IEnumerable<SpendingDto>> Handle(GetSpendingsByBucketIdQuery query)
    {
        var spendings = spendingRepository.AsQueryable();
        var filteredSpendings = spendings.Where(s => s.BucketId == query.BucketId);
        var result = filteredSpendings.Select(_mapper.ToDto).ToArray();
        return await Task.FromResult(result);
    }
}

public class GetSpendingsByOwnerQueryHandler(ISpendingRepository spendingRepository, ILogger<GetSpendingsByOwnerQueryHandler>? logger = null)
{
    private readonly SpendingMapper _mapper = new();

    public async Task<IEnumerable<SpendingDto>> Handle(GetSpendingsByOwnerQuery query)
    {
        var spendings = spendingRepository.AsQueryable();
        var filteredSpendings = spendings.Where(s => s.Owner.Equals(query.Owner, StringComparison.OrdinalIgnoreCase));
        var result = filteredSpendings.Select(_mapper.ToDto).ToArray();
        return await Task.FromResult(result);
    }
}

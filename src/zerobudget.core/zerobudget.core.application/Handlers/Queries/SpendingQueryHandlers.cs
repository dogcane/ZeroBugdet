using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;
using Microsoft.Extensions.Logging;

namespace zerobudget.core.application.Handlers.Queries;

public class GetSpendingByIdQueryHandler(ISpendingRepository spendingRepository, ILogger<GetSpendingByIdQueryHandler>? logger = null)
{
    private readonly ISpendingRepository _spendingRepository = spendingRepository;
    private readonly ILogger<GetSpendingByIdQueryHandler>? _logger = logger;
    private readonly SpendingMapper _mapper = new();

    public async Task<SpendingDto?> Handle(GetSpendingByIdQuery query)
    {
        var spending = await _spendingRepository.LoadAsync(query.Id);
        if (spending == null)
            return null;
        return _mapper.ToDto(spending);
    }
}

public class GetAllSpendingsQueryHandler(ISpendingRepository spendingRepository, ILogger<GetAllSpendingsQueryHandler>? logger = null)
{
    private readonly ISpendingRepository _spendingRepository = spendingRepository;
    private readonly ILogger<GetAllSpendingsQueryHandler>? _logger = logger;
    private readonly SpendingMapper _mapper = new();

    public async Task<IEnumerable<SpendingDto>> Handle(GetAllSpendingsQuery query)
    {
        var spendings = _spendingRepository.AsQueryable();
        return await Task.FromResult(spendings.Select(_mapper.ToDto));
    }
}

public class GetSpendingsByBucketIdQueryHandler(ISpendingRepository spendingRepository, ILogger<GetSpendingsByBucketIdQueryHandler>? logger = null)
{
    private readonly ISpendingRepository _spendingRepository = spendingRepository;
    private readonly ILogger<GetSpendingsByBucketIdQueryHandler>? _logger = logger;
    private readonly SpendingMapper _mapper = new();

    public async Task<IEnumerable<SpendingDto>> Handle(GetSpendingsByBucketIdQuery query)
    {
        var spendings = _spendingRepository.AsQueryable();
        var filteredSpendings = spendings.Where(s => s.BucketId == query.BucketId);
        return await Task.FromResult(filteredSpendings.Select(_mapper.ToDto));
    }
}

public class GetSpendingsByOwnerQueryHandler(ISpendingRepository spendingRepository, ILogger<GetSpendingsByOwnerQueryHandler>? logger = null)
{
    private readonly ISpendingRepository _spendingRepository = spendingRepository;
    private readonly ILogger<GetSpendingsByOwnerQueryHandler>? _logger = logger;
    private readonly SpendingMapper _mapper = new();

    public async Task<IEnumerable<SpendingDto>> Handle(GetSpendingsByOwnerQuery query)
    {
        var spendings = _spendingRepository.AsQueryable();
        var filteredSpendings = spendings.Where(s => s.Owner.Equals(query.Owner, StringComparison.OrdinalIgnoreCase));
        return await Task.FromResult(filteredSpendings.Select(_mapper.ToDto));
    }
}

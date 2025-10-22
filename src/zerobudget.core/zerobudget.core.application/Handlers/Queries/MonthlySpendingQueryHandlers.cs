using JasperFx.Core.Reflection;
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;
using Microsoft.Extensions.Logging;

namespace zerobudget.core.application.Handlers.Queries;

public class GetMonthlySpendingByIdQueryHandler(IMonthlySpendingRepository monthlySpendingRepository, ILogger<GetMonthlySpendingByIdQueryHandler>? logger = null)
{
    private readonly IMonthlySpendingRepository _monthlySpendingRepository = monthlySpendingRepository;
    private readonly ILogger<GetMonthlySpendingByIdQueryHandler>? _logger = logger;
    private readonly MonthlySpendingMapper _mapper = new();

    public async Task<MonthlySpendingDto?> Handle(GetMonthlySpendingByIdQuery query)
    {
        var monthlySpending = await _monthlySpendingRepository.LoadAsync(query.Id);
        if (monthlySpending == null)
            return null;
        return _mapper.ToDto(monthlySpending);
    }
}

public class GetAllMonthlySpendingsQueryHandler(IMonthlySpendingRepository monthlySpendingRepository, ILogger<GetAllMonthlySpendingsQueryHandler>? logger = null)
{
    private readonly IMonthlySpendingRepository _monthlySpendingRepository = monthlySpendingRepository;
    private readonly ILogger<GetAllMonthlySpendingsQueryHandler>? _logger = logger;
    private readonly MonthlySpendingMapper _mapper = new();

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetAllMonthlySpendingsQuery query)
    {
        var monthlySpendings = _monthlySpendingRepository.AsQueryable();
        return await Task.FromResult(monthlySpendings.Select(_mapper.ToDto));
    }
}

public class GetMonthlySpendingsByMonthlyBucketIdQueryHandler(IMonthlySpendingRepository monthlySpendingRepository, ILogger<GetMonthlySpendingsByMonthlyBucketIdQueryHandler>? logger = null)
{
    private readonly IMonthlySpendingRepository _monthlySpendingRepository = monthlySpendingRepository;
    private readonly ILogger<GetMonthlySpendingsByMonthlyBucketIdQueryHandler>? _logger = logger;
    private readonly MonthlySpendingMapper _mapper = new();

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetMonthlySpendingsByMonthlyBucketIdQuery query)
    {
        var monthlySpendings = _monthlySpendingRepository.AsQueryable();
        var filteredSpendings = monthlySpendings.Where(ms => ms.MonthlyBucketId == query.MonthlyBucketId);
        return await Task.FromResult(filteredSpendings.Select(_mapper.ToDto));
    }
}

public class GetMonthlySpendingsByDateRangeQueryHandler(IMonthlySpendingRepository monthlySpendingRepository, ILogger<GetMonthlySpendingsByDateRangeQueryHandler>? logger = null)
{
    private readonly IMonthlySpendingRepository _monthlySpendingRepository = monthlySpendingRepository;
    private readonly ILogger<GetMonthlySpendingsByDateRangeQueryHandler>? _logger = logger;
    private readonly MonthlySpendingMapper _mapper = new();

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetMonthlySpendingsByDateRangeQuery query)
    {
        var monthlySpendings = _monthlySpendingRepository.AsQueryable();
        var filteredSpendings = monthlySpendings.Where(ms => ms.Date >= query.StartDate && ms.Date <= query.EndDate);
        return await Task.FromResult(filteredSpendings.Select(_mapper.ToDto));
    }
}

public class GetMonthlySpendingsByOwnerQueryHandler(IMonthlySpendingRepository monthlySpendingRepository, ILogger<GetMonthlySpendingsByOwnerQueryHandler>? logger = null)
{
    private readonly IMonthlySpendingRepository _monthlySpendingRepository = monthlySpendingRepository;
    private readonly ILogger<GetMonthlySpendingsByOwnerQueryHandler>? _logger = logger;
    private readonly MonthlySpendingMapper _mapper = new();

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetMonthlySpendingsByOwnerQuery query)
    {
        var monthlySpendings = _monthlySpendingRepository.AsQueryable();
        var filteredSpendings = monthlySpendings.Where(ms => ms.Owner.Equals(query.Owner, StringComparison.OrdinalIgnoreCase));
        return await Task.FromResult(filteredSpendings.Select(_mapper.ToDto));
    }
}

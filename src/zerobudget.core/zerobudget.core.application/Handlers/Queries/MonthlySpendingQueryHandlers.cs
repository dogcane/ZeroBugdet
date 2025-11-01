using JasperFx.Core.Reflection;
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;
using Microsoft.Extensions.Logging;

namespace zerobudget.core.application.Handlers.Queries;

public class GetMonthlySpendingByIdQueryHandler(IMonthlySpendingRepository monthlySpendingRepository, ILogger<GetMonthlySpendingByIdQueryHandler>? logger = null)
{
    private readonly MonthlySpendingMapper _mapper = new();

    public async Task<MonthlySpendingDto?> Handle(GetMonthlySpendingByIdQuery query)
    {
        var monthlySpending = await monthlySpendingRepository.LoadAsync(query.Id);
        if (monthlySpending == null)
            return null;
        return _mapper.ToDto(monthlySpending);
    }
}

public class GetAllMonthlySpendingsQueryHandler(IMonthlySpendingRepository monthlySpendingRepository, ILogger<GetAllMonthlySpendingsQueryHandler>? logger = null)
{
    private readonly MonthlySpendingMapper _mapper = new();

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetAllMonthlySpendingsQuery query)
    {
        var monthlySpendings = monthlySpendingRepository.AsQueryable();
        monthlySpendings = monthlySpendings.OrderByDescending(ms => ms.Date);
        var result = monthlySpendings.Select(_mapper.ToDto).ToArray();
        return await Task.FromResult(result);
    }
}

public class GetMonthlySpendingsByMonthlyBucketIdQueryHandler(IMonthlySpendingRepository monthlySpendingRepository, ILogger<GetMonthlySpendingsByMonthlyBucketIdQueryHandler>? logger = null)
{
    private readonly MonthlySpendingMapper _mapper = new();

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetMonthlySpendingsByMonthlyBucketIdQuery query)
    {
        var monthlySpendings = monthlySpendingRepository.AsQueryable();
        var filteredSpendings = monthlySpendings.Where(ms => ms.MonthlyBucketId == query.MonthlyBucketId);
        filteredSpendings = filteredSpendings.OrderByDescending(ms => ms.Date);
        var result = filteredSpendings.Select(_mapper.ToDto).ToArray();
        return await Task.FromResult(result);
    }
}

public class GetMonthlySpendingsByDateRangeQueryHandler(IMonthlySpendingRepository monthlySpendingRepository, ILogger<GetMonthlySpendingsByDateRangeQueryHandler>? logger = null)
{
    private readonly MonthlySpendingMapper _mapper = new();

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetMonthlySpendingsByDateRangeQuery query)
    {
        var monthlySpendings = monthlySpendingRepository.AsQueryable();
        var filteredSpendings = monthlySpendings.Where(ms => ms.Date >= query.StartDate && ms.Date <= query.EndDate);
        filteredSpendings = filteredSpendings.OrderByDescending(ms => ms.Date);
        var result = filteredSpendings.Select(_mapper.ToDto).ToArray();
        return await Task.FromResult(result);
    }
}

public class GetMonthlySpendingsByOwnerQueryHandler(IMonthlySpendingRepository monthlySpendingRepository, ILogger<GetMonthlySpendingsByOwnerQueryHandler>? logger = null)
{
    private readonly MonthlySpendingMapper _mapper = new();

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetMonthlySpendingsByOwnerQuery query)
    {
        var monthlySpendings = monthlySpendingRepository.AsQueryable();
        var filteredSpendings = monthlySpendings.Where(ms => ms.Owner.Equals(query.Owner, StringComparison.OrdinalIgnoreCase));
        filteredSpendings = filteredSpendings.OrderByDescending(ms => ms.Date);
        var result = filteredSpendings.Select(_mapper.ToDto).ToArray();
        return await Task.FromResult(result);
    }
}

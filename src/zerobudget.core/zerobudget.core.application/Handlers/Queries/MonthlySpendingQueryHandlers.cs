using JasperFx.Core.Reflection;
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;
using Microsoft.Extensions.Logging;

namespace zerobudget.core.application.Handlers.Queries;

public class MonthlySpendingQueryHandlers(IMonthlySpendingRepository monthlySpendingRepository, ILogger<MonthlySpendingQueryHandlers>? logger = null)
{
    private readonly IMonthlySpendingRepository _monthlySpendingRepository = monthlySpendingRepository;
    private readonly ILogger<MonthlySpendingQueryHandlers>? _logger = logger;
    private readonly MonthlySpendingMapper _mapper = new();    

    public async Task<MonthlySpendingDto?> Handle(GetMonthlySpendingByIdQuery query)
    {
        var monthlySpending = await _monthlySpendingRepository.LoadAsync(query.Id);
        if (monthlySpending == null)
            return null;
        return _mapper.ToDto(monthlySpending);
    }

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetAllMonthlySpendingsQuery query)
    {
        var monthlySpendings = _monthlySpendingRepository.AsQueryable();
        return await Task.FromResult(monthlySpendings.Select(_mapper.ToDto));
    }

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetMonthlySpendingsByMonthlyBucketIdQuery query)
    {
        var monthlySpendings =  _monthlySpendingRepository.AsQueryable();
        var filteredSpendings = monthlySpendings.Where(ms => ms.MonthlyBucketId == query.MonthlyBucketId);
        return await Task.FromResult(filteredSpendings.Select(_mapper.ToDto));
    }

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetMonthlySpendingsByDateRangeQuery query)
    {
        var monthlySpendings = _monthlySpendingRepository.AsQueryable();
        var filteredSpendings = monthlySpendings.Where(ms => ms.Date >= query.StartDate && ms.Date <= query.EndDate);
        return await Task.FromResult(filteredSpendings.Select(_mapper.ToDto));
    }

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetMonthlySpendingsByOwnerQuery query)
    {
        var monthlySpendings = _monthlySpendingRepository.AsQueryable();
        var filteredSpendings = monthlySpendings.Where(ms => ms.Owner.Equals(query.Owner, StringComparison.OrdinalIgnoreCase));
        return await Task.FromResult(filteredSpendings.Select(_mapper.ToDto));
    }
}
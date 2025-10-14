using JasperFx.Core.Reflection;
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Queries;

public class MonthlySpendingQueryHandlers
{
    private readonly IMonthlySpendingRepository _monthlySpendingRepository;

    public MonthlySpendingQueryHandlers(IMonthlySpendingRepository monthlySpendingRepository)
    {
        _monthlySpendingRepository = monthlySpendingRepository;
    }

    public async Task<MonthlySpendingDto?> Handle(GetMonthlySpendingByIdQuery query)
    {
        var monthlySpending = await _monthlySpendingRepository.LoadAsync(query.Id);
        if (monthlySpending == null)
            return null;

        return new MonthlySpendingDto(
            monthlySpending.Identity,
            monthlySpending.Date,
            monthlySpending.MonthlyBucketId,
            monthlySpending.Description,
            monthlySpending.Amount,
            monthlySpending.Owner,
            monthlySpending.Tags
        );
    }

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetAllMonthlySpendingsQuery query)
    {
        var monthlySpendings = _monthlySpendingRepository.AsQueryable();
        return await Task.FromResult(monthlySpendings.Select(monthlySpending => new MonthlySpendingDto(
            monthlySpending.Identity,
            monthlySpending.Date,
            monthlySpending.MonthlyBucketId,
            monthlySpending.Description,
            monthlySpending.Amount,
            monthlySpending.Owner,
            monthlySpending.Tags
        )));
    }

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetMonthlySpendingsByMonthlyBucketIdQuery query)
    {
        var monthlySpendings =  _monthlySpendingRepository.AsQueryable();
        var filteredSpendings = monthlySpendings.Where(ms => ms.MonthlyBucketId == query.MonthlyBucketId);

        return await Task.FromResult(filteredSpendings.Select(monthlySpending => new MonthlySpendingDto(
            monthlySpending.Identity,
            monthlySpending.Date,
            monthlySpending.MonthlyBucketId,
            monthlySpending.Description,
            monthlySpending.Amount,
            monthlySpending.Owner,
            monthlySpending.Tags
        )));
    }

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetMonthlySpendingsByDateRangeQuery query)
    {
        var monthlySpendings = _monthlySpendingRepository.AsQueryable();
        var filteredSpendings = monthlySpendings.Where(ms => ms.Date >= query.StartDate && ms.Date <= query.EndDate);

        return await Task.FromResult(filteredSpendings.Select(monthlySpending => new MonthlySpendingDto(
            monthlySpending.Identity,
            monthlySpending.Date,
            monthlySpending.MonthlyBucketId,
            monthlySpending.Description,
            monthlySpending.Amount,
            monthlySpending.Owner,
            monthlySpending.Tags
        )));
    }

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetMonthlySpendingsByOwnerQuery query)
    {
        var monthlySpendings = _monthlySpendingRepository.AsQueryable();
        var filteredSpendings = monthlySpendings.Where(ms => ms.Owner.Equals(query.Owner, StringComparison.OrdinalIgnoreCase));

        return await Task.FromResult(filteredSpendings.Select(monthlySpending => new MonthlySpendingDto(
            monthlySpending.Identity,
            monthlySpending.Date,
            monthlySpending.MonthlyBucketId,
            monthlySpending.Description,
            monthlySpending.Amount,
            monthlySpending.Owner,
            monthlySpending.Tags
        )));
    }
}
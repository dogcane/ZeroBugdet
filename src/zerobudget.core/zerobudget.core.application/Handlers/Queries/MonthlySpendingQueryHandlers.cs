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

/// <summary>
/// Query handler for GetMonthlySpendingsQuery
/// Applies strongly-typed LINQ filters without reflection
/// Orders by Date descending (most recent first)
/// </summary>
public class MonthlySpendingCollectionQueryHandler(IMonthlySpendingRepository repository, ILogger<MonthlySpendingCollectionQueryHandler>? logger = null)
{
    private readonly Mappers.MonthlySpendingMapper _mapper = new();

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(GetMonthlySpendingsQuery query)
    {
        var queryable = repository.AsQueryable();
        queryable = ApplyFilters(queryable, query);
        queryable = ApplyOrdering(queryable);
        var result = queryable.Select(MapToDto).ToArray();
        return await Task.FromResult(result);
    }

    /// <summary>
    /// Apply LINQ filters directly based on GetMonthlySpendingsQuery properties
    /// </summary>
    private IQueryable<MonthlySpending> ApplyFilters(IQueryable<MonthlySpending> queryable, GetMonthlySpendingsQuery query)
    {
        // Filter by MonthlyBucketId (exact match)
        if (query.MonthlyBucketId.HasValue)
        {
            queryable = queryable.Where(ms => ms.MonthlyBucketId == query.MonthlyBucketId.Value);
        }

        // Filter by Description (case-insensitive contains search)
        if (!string.IsNullOrWhiteSpace(query.Description))
        {
            queryable = queryable.Where(ms => ms.Description.Contains(query.Description, StringComparison.OrdinalIgnoreCase));
        }

        // Filter by Owner (case-insensitive exact match)
        if (!string.IsNullOrWhiteSpace(query.Owner))
        {
            queryable = queryable.Where(ms => ms.Owner.Equals(query.Owner, StringComparison.OrdinalIgnoreCase));
        }

        // Filter by StartDate (greater than or equal)
        if (query.StartDate.HasValue)
        {
            queryable = queryable.Where(ms => ms.Date >= query.StartDate.Value);
        }

        // Filter by EndDate (less than or equal)
        if (query.EndDate.HasValue)
        {
            queryable = queryable.Where(ms => ms.Date <= query.EndDate.Value);
        }

        return queryable;
    }

    /// <summary>
    /// Apply ordering by Date descending (most recent first)
    /// </summary>
    private IQueryable<MonthlySpending> ApplyOrdering(IQueryable<MonthlySpending> query)
    {
        return query.OrderByDescending(ms => ms.Date);
    }

    /// <summary>
    /// Convert MonthlySpending entity to MonthlySpendingDto
    /// </summary>
    private MonthlySpendingDto MapToDto(MonthlySpending entity)
    {
        return _mapper.ToDto(entity);
    }
}

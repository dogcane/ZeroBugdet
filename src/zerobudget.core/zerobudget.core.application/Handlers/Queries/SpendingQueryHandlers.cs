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

/// <summary>
/// Query handler for GetSpendingsQuery
/// Applies strongly-typed LINQ filters without reflection
/// Orders by Description ascending
/// </summary>
public class SpendingCollectionQueryHandler(ISpendingRepository repository, ILogger<SpendingCollectionQueryHandler>? logger = null)
{
    private readonly Mappers.SpendingMapper _mapper = new();

    public async Task<IEnumerable<SpendingDto>> Handle(GetSpendingsQuery query)
    {
        var queryable = repository.AsQueryable();
        queryable = ApplyFilters(queryable, query);
        queryable = ApplyOrdering(queryable);
        var result = queryable.Select(MapToDto).ToArray();
        return await Task.FromResult(result);
    }

    /// <summary>
    /// Apply LINQ filters directly based on GetSpendingsQuery properties
    /// </summary>
    private IQueryable<Spending> ApplyFilters(IQueryable<Spending> queryable, GetSpendingsQuery query)
    {
        // Filter by BucketId (exact match)
        if (query.BucketId.HasValue)
        {
            queryable = queryable.Where(s => s.BucketId == query.BucketId.Value);
        }

        // Filter by Description (case-insensitive contains search)
        if (!string.IsNullOrWhiteSpace(query.Description))
        {
            queryable = queryable.Where(s => s.Description.Contains(query.Description, StringComparison.OrdinalIgnoreCase));
        }

        // Filter by Owner (case-insensitive exact match)
        if (!string.IsNullOrWhiteSpace(query.Owner))
        {
            queryable = queryable.Where(s => s.Owner.Equals(query.Owner, StringComparison.OrdinalIgnoreCase));
        }

        // Filter by Enabled (exact match)
        if (query.Enabled.HasValue)
        {
            queryable = queryable.Where(s => s.Enabled == query.Enabled.Value);
        }

        return queryable;
    }

    /// <summary>
    /// Apply ordering by Description ascending
    /// </summary>
    private IQueryable<Spending> ApplyOrdering(IQueryable<Spending> query)
    {
        return query.OrderBy(s => s.Description);
    }

    /// <summary>
    /// Convert Spending entity to SpendingDto
    /// </summary>
    private SpendingDto MapToDto(Spending entity)
    {
        return _mapper.ToDto(entity);
    }
}

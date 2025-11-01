using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;
using Microsoft.Extensions.Logging;

namespace zerobudget.core.application.Handlers.Queries;

public class GetMonthlyBucketByIdQueryHandler(IMonthlyBucketRepository monthlyBucketRepository, ILogger<GetMonthlyBucketByIdQueryHandler>? logger = null)
{
    private readonly MonthlyBucketMapper _mapper = new();

    public async Task<MonthlyBucketDto?> Handle(GetMonthlyBucketByIdQuery query)
    {
        var monthlyBucket = await monthlyBucketRepository.LoadAsync(query.Id);
        if (monthlyBucket == null)
            return null;
        return _mapper.ToDto(monthlyBucket);
    }
}

/// <summary>
/// Query handler for GetMonthlyBucketsQuery
/// Applies strongly-typed LINQ filters without reflection
/// Orders by Description ascending
/// </summary>
public class MonthlyBucketCollectionQueryHandler(IMonthlyBucketRepository repository, ILogger<MonthlyBucketCollectionQueryHandler>? logger = null)
{
    private readonly Mappers.MonthlyBucketMapper _mapper = new();

    public async Task<IEnumerable<MonthlyBucketDto>> Handle(GetMonthlyBucketsQuery query)
    {
        var queryable = repository.AsQueryable();
        queryable = ApplyFilters(queryable, query);
        queryable = ApplyOrdering(queryable);
        var result = queryable.Select(MapToDto).ToArray();
        return await Task.FromResult(result);
    }

    /// <summary>
    /// Apply LINQ filters directly based on GetMonthlyBucketsQuery properties
    /// </summary>
    private IQueryable<MonthlyBucket> ApplyFilters(IQueryable<MonthlyBucket> queryable, GetMonthlyBucketsQuery query)
    {
        // Filter by Year (exact match)
        if (query.Year.HasValue)
        {
            queryable = queryable.Where(mb => mb.Year == query.Year.Value);
        }

        // Filter by Month (exact match)
        if (query.Month.HasValue)
        {
            queryable = queryable.Where(mb => mb.Month == query.Month.Value);
        }

        // Filter by BucketId (exact match)
        if (query.BucketId.HasValue)
        {
            queryable = queryable.Where(mb => mb.BucketId == query.BucketId.Value);
        }

        // Filter by Description (case-insensitive contains search)
        if (!string.IsNullOrWhiteSpace(query.Description))
        {
            queryable = queryable.Where(mb => mb.Description.Contains(query.Description, StringComparison.OrdinalIgnoreCase));
        }

        return queryable;
    }

    /// <summary>
    /// Apply ordering by Description ascending
    /// </summary>
    private IQueryable<MonthlyBucket> ApplyOrdering(IQueryable<MonthlyBucket> query)
    {
        return query.OrderBy(mb => mb.Description);
    }

    /// <summary>
    /// Convert MonthlyBucket entity to MonthlyBucketDto
    /// </summary>
    private MonthlyBucketDto MapToDto(MonthlyBucket entity)
    {
        return _mapper.ToDto(entity);
    }
}

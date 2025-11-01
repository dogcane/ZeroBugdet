using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;
using System.Transactions;
using Microsoft.Extensions.Logging;

namespace zerobudget.core.application.Handlers.Queries;

/// <summary>
/// Handler for GetBucketByIdQuery
/// Each handler class has only ONE Handle method for Wolverine compatibility
/// </summary>
public class GetBucketByIdQueryHandler(IBucketRepository bucketRepository, ILogger<GetBucketByIdQueryHandler>? logger = null)
{
    private readonly BucketMapper _mapper = new();

    public async Task<BucketDto?> Handle(GetBucketByIdQuery query)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var bucket = await bucketRepository.LoadAsync(query.Id);
        if (bucket == null)
            return null;
        scope.Complete();
        return _mapper.ToDto(bucket);
    }
}
/// <summary>
/// Query handler for GetBucketsQuery
/// Applies strongly-typed LINQ filters without reflection
/// </summary>
public class GetBucketsQueryHandler(IBucketRepository repository, ILogger<GetBucketsQueryHandler>? logger = null)
{
    private readonly Mappers.BucketMapper _mapper = new();

    public async Task<IEnumerable<BucketDto>> Handle(GetBucketsQuery query)
    {
        var queryable = repository.AsQueryable();
        queryable = ApplyFilters(queryable, query);
        queryable = ApplyOrdering(queryable);
        var result = queryable.Select(MapToDto).ToArray();
        return await Task.FromResult(result);
    }

    /// <summary>
    /// Apply LINQ filters directly based on GetBucketsQuery properties
    /// </summary>
    private IQueryable<Bucket> ApplyFilters(IQueryable<Bucket> queryable, GetBucketsQuery query)
    {
        // Filter by Name (case-insensitive contains search)
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            queryable = queryable.Where(b => b.Name.Contains(query.Name, StringComparison.OrdinalIgnoreCase));
        }

        // Filter by Description (case-insensitive contains search)
        if (!string.IsNullOrWhiteSpace(query.Description))
        {
            queryable = queryable.Where(b => b.Description.Contains(query.Description, StringComparison.OrdinalIgnoreCase));
        }

        // Filter by Enabled (exact match)
        queryable = queryable.Where(b => b.Enabled == query.Enabled);

        return queryable;
    }

    /// <summary>
    /// Apply ordering by Name ascending
    /// </summary>
    private IQueryable<Bucket> ApplyOrdering(IQueryable<Bucket> query)
    {
        return query.OrderBy(b => b.Name);
    }

    /// <summary>
    /// Convert Bucket entity to BucketDto
    /// </summary>
    private BucketDto MapToDto(Bucket entity)
    {
        return _mapper.ToDto(entity);
    }
}

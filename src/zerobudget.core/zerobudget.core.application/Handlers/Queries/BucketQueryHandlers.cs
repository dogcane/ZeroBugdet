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
/// Handler for GetBucketsByNameQuery
/// </summary>
public class GetBucketsByNameQueryHandler(IBucketRepository bucketRepository, ILogger<GetBucketsByNameQueryHandler>? logger = null)
{
    private readonly BucketMapper _mapper = new();

    public async Task<IEnumerable<BucketDto>> Handle(GetBucketsByNameQuery query)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var buckets = bucketRepository.AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            buckets = buckets.Where(b => b.Name.Contains(query.Name, StringComparison.OrdinalIgnoreCase));
        }
        if (!string.IsNullOrWhiteSpace(query.Description))
        {
            buckets = buckets.Where(b => b.Description.Contains(query.Description, StringComparison.OrdinalIgnoreCase));
        }
        if (query.Enabled)
        {
            buckets = buckets.Where(b => b.Enabled);
        }
        var result = buckets.Select(_mapper.ToDto).ToArray();
        scope.Complete();
        return await Task.FromResult(result);
    }
}

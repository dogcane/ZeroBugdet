using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;

namespace zerobudget.core.application.Handlers.Queries;

using System.Transactions;
using Microsoft.Extensions.Logging;

public class BucketQueryHandlers(IBucketRepository bucketRepository, ILogger<BucketQueryHandlers>? logger = null)
{
    private readonly IBucketRepository _bucketRepository = bucketRepository;
    private readonly ILogger<BucketQueryHandlers>? _logger = logger;
    private readonly BucketMapper _mapper = new();

    public async Task<BucketDto?> Handle(GetBucketByIdQuery query)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var bucket = await _bucketRepository.LoadAsync(query.Id);
        if (bucket == null)
            return null;
        scope.Complete();
        return _mapper.ToDto(bucket);
    }

    public async Task<IEnumerable<BucketDto>> Handle(GetBucketsByNameQuery query)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var buckets = _bucketRepository.AsQueryable();
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
        scope.Complete();
        return await Task.FromResult(buckets.Select(_mapper.ToDto));
    }
}
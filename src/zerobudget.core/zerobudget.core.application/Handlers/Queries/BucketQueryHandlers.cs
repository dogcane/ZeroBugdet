using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Queries;

public class BucketQueryHandlers
{
    private readonly IBucketRepository _bucketRepository;

    public BucketQueryHandlers(IBucketRepository bucketRepository)
    {
        _bucketRepository = bucketRepository;
    }

    public async Task<BucketDto?> Handle(GetBucketByIdQuery query)
    {
        var bucket = await _bucketRepository.GetByIdAsync(query.Id);
        if (bucket == null)
            return null;

        return new BucketDto(
            bucket.Identity,
            bucket.Name,
            bucket.Description,
            bucket.DefaultLimit,
            bucket.DefaultBalance
        );
    }

    public async Task<IEnumerable<BucketDto>> Handle(GetAllBucketsQuery query)
    {
        var buckets = await _bucketRepository.GetAllAsync();
        return buckets.Select(bucket => new BucketDto(
            bucket.Identity,
            bucket.Name,
            bucket.Description,
            bucket.DefaultLimit,
            bucket.DefaultBalance
        ));
    }

    public async Task<IEnumerable<BucketDto>> Handle(GetBucketsByNameQuery query)
    {
        // This would need a custom repository method or filtering implementation
        var buckets = await _bucketRepository.GetAllAsync();
        var filteredBuckets = buckets.Where(b => b.Name.Contains(query.Name, StringComparison.OrdinalIgnoreCase));
        
        return filteredBuckets.Select(bucket => new BucketDto(
            bucket.Identity,
            bucket.Name,
            bucket.Description,
            bucket.DefaultLimit,
            bucket.DefaultBalance
        ));
    }
}
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Queries;

public class MonthlyBucketQueryHandlers
{
    private readonly IMonthlyBucketRepository _monthlyBucketRepository;

    public MonthlyBucketQueryHandlers(IMonthlyBucketRepository monthlyBucketRepository)
    {
        _monthlyBucketRepository = monthlyBucketRepository;
    }

    public async Task<MonthlyBucketDto?> Handle(GetMonthlyBucketByIdQuery query)
    {
        var monthlyBucket = await _monthlyBucketRepository.GetByIdAsync(query.Id);
        if (monthlyBucket == null)
            return null;

        return new MonthlyBucketDto(
            monthlyBucket.Identity,
            monthlyBucket.Year,
            monthlyBucket.Month,
            monthlyBucket.Balance,
            monthlyBucket.Description,
            monthlyBucket.Limit,
            monthlyBucket.Bucket.Identity
        );
    }

    public async Task<IEnumerable<MonthlyBucketDto>> Handle(GetAllMonthlyBucketsQuery query)
    {
        var monthlyBuckets = await _monthlyBucketRepository.GetAllAsync();
        return monthlyBuckets.Select(monthlyBucket => new MonthlyBucketDto(
            monthlyBucket.Identity,
            monthlyBucket.Year,
            monthlyBucket.Month,
            monthlyBucket.Balance,
            monthlyBucket.Description,
            monthlyBucket.Limit,
            monthlyBucket.Bucket.Identity
        ));
    }

    public async Task<IEnumerable<MonthlyBucketDto>> Handle(GetMonthlyBucketsByYearMonthQuery query)
    {
        var monthlyBuckets = await _monthlyBucketRepository.GetAllAsync();
        var filteredBuckets = monthlyBuckets.Where(mb => mb.Year == query.Year && mb.Month == query.Month);
        
        return filteredBuckets.Select(monthlyBucket => new MonthlyBucketDto(
            monthlyBucket.Identity,
            monthlyBucket.Year,
            monthlyBucket.Month,
            monthlyBucket.Balance,
            monthlyBucket.Description,
            monthlyBucket.Limit,
            monthlyBucket.Bucket.Identity
        ));
    }

    public async Task<IEnumerable<MonthlyBucketDto>> Handle(GetMonthlyBucketsByBucketIdQuery query)
    {
        var monthlyBuckets = await _monthlyBucketRepository.GetAllAsync();
        var filteredBuckets = monthlyBuckets.Where(mb => mb.Bucket.Identity == query.BucketId);
        
        return filteredBuckets.Select(monthlyBucket => new MonthlyBucketDto(
            monthlyBucket.Identity,
            monthlyBucket.Year,
            monthlyBucket.Month,
            monthlyBucket.Balance,
            monthlyBucket.Description,
            monthlyBucket.Limit,
            monthlyBucket.Bucket.Identity
        ));
    }
}
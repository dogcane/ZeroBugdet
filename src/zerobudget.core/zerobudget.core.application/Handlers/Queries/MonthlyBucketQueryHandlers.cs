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

public class GetAllMonthlyBucketsQueryHandler(IMonthlyBucketRepository monthlyBucketRepository, ILogger<GetAllMonthlyBucketsQueryHandler>? logger = null)
{
    private readonly MonthlyBucketMapper _mapper = new();

    public async Task<IEnumerable<MonthlyBucketDto>> Handle(GetAllMonthlyBucketsQuery query)
    {
        var monthlyBuckets = monthlyBucketRepository.AsQueryable();
        monthlyBuckets = monthlyBuckets.OrderBy(mb => mb.Description);
        var result = monthlyBuckets.Select(_mapper.ToDto).ToArray();
        return await Task.FromResult(result);
    }
}

public class GetMonthlyBucketsByYearMonthQueryHandler(IMonthlyBucketRepository monthlyBucketRepository, ILogger<GetMonthlyBucketsByYearMonthQueryHandler>? logger = null)
{
    private readonly MonthlyBucketMapper _mapper = new();

    public async Task<IEnumerable<MonthlyBucketDto>> Handle(GetMonthlyBucketsByYearMonthQuery query)
    {
        var monthlyBuckets = monthlyBucketRepository.AsQueryable();
        var filteredBuckets = monthlyBuckets.Where(mb => mb.Year == query.Year && mb.Month == query.Month);
        filteredBuckets = filteredBuckets.OrderBy(mb => mb.Description);
        var result = filteredBuckets.Select(_mapper.ToDto).ToArray();
        return await Task.FromResult(result);
    }
}

public class GetMonthlyBucketsByBucketIdQueryHandler(IMonthlyBucketRepository monthlyBucketRepository, ILogger<GetMonthlyBucketsByBucketIdQueryHandler>? logger = null)
{
    private readonly MonthlyBucketMapper _mapper = new();

    public async Task<IEnumerable<MonthlyBucketDto>> Handle(GetMonthlyBucketsByBucketIdQuery query)
    {
        var monthlyBuckets = monthlyBucketRepository.AsQueryable();
        var filteredBuckets = monthlyBuckets.Where(mb => mb.BucketId == query.BucketId);
        filteredBuckets = filteredBuckets.OrderBy(mb => mb.Description);
        var result = filteredBuckets.Select(_mapper.ToDto).ToArray();
        return await Task.FromResult(result);
    }
}

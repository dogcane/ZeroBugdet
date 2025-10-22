using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;
using Microsoft.Extensions.Logging;

namespace zerobudget.core.application.Handlers.Queries;

public class GetMonthlyBucketByIdQueryHandler(IMonthlyBucketRepository monthlyBucketRepository, ILogger<GetMonthlyBucketByIdQueryHandler>? logger = null)
{
    private readonly IMonthlyBucketRepository _monthlyBucketRepository = monthlyBucketRepository;
    private readonly ILogger<GetMonthlyBucketByIdQueryHandler>? _logger = logger;
    private readonly MonthlyBucketMapper _mapper = new();

    public async Task<MonthlyBucketDto?> Handle(GetMonthlyBucketByIdQuery query)
    {
        var monthlyBucket = await _monthlyBucketRepository.LoadAsync(query.Id);
        if (monthlyBucket == null)
            return null;
        return _mapper.ToDto(monthlyBucket);
    }
}

public class GetAllMonthlyBucketsQueryHandler(IMonthlyBucketRepository monthlyBucketRepository, ILogger<GetAllMonthlyBucketsQueryHandler>? logger = null)
{
    private readonly IMonthlyBucketRepository _monthlyBucketRepository = monthlyBucketRepository;
    private readonly ILogger<GetAllMonthlyBucketsQueryHandler>? _logger = logger;
    private readonly MonthlyBucketMapper _mapper = new();

    public async Task<IEnumerable<MonthlyBucketDto>> Handle(GetAllMonthlyBucketsQuery query)
    {
        var monthlyBuckets = _monthlyBucketRepository.AsQueryable();
        return await Task.FromResult(monthlyBuckets.Select(_mapper.ToDto));
    }
}

public class GetMonthlyBucketsByYearMonthQueryHandler(IMonthlyBucketRepository monthlyBucketRepository, ILogger<GetMonthlyBucketsByYearMonthQueryHandler>? logger = null)
{
    private readonly IMonthlyBucketRepository _monthlyBucketRepository = monthlyBucketRepository;
    private readonly ILogger<GetMonthlyBucketsByYearMonthQueryHandler>? _logger = logger;
    private readonly MonthlyBucketMapper _mapper = new();

    public async Task<IEnumerable<MonthlyBucketDto>> Handle(GetMonthlyBucketsByYearMonthQuery query)
    {
        var monthlyBuckets = _monthlyBucketRepository.AsQueryable();
        var filteredBuckets = monthlyBuckets.Where(mb => mb.Year == query.Year && mb.Month == query.Month);
        return await Task.FromResult(filteredBuckets.Select(_mapper.ToDto));
    }
}

public class GetMonthlyBucketsByBucketIdQueryHandler(IMonthlyBucketRepository monthlyBucketRepository, ILogger<GetMonthlyBucketsByBucketIdQueryHandler>? logger = null)
{
    private readonly IMonthlyBucketRepository _monthlyBucketRepository = monthlyBucketRepository;
    private readonly ILogger<GetMonthlyBucketsByBucketIdQueryHandler>? _logger = logger;
    private readonly MonthlyBucketMapper _mapper = new();

    public async Task<IEnumerable<MonthlyBucketDto>> Handle(GetMonthlyBucketsByBucketIdQuery query)
    {
        var monthlyBuckets = _monthlyBucketRepository.AsQueryable();
        var filteredBuckets = monthlyBuckets.Where(mb => mb.Bucket.Identity == query.BucketId);
        return await Task.FromResult(filteredBuckets.Select(_mapper.ToDto));
    }
}

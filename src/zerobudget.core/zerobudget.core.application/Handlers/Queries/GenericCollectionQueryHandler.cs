using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Queries;

/// <summary>
/// Base generic query handler for querying collections of entities
/// </summary>
public abstract class GenericCollectionQueryHandler<TEntity, TDto, TQuery, TRepository>
    where TEntity : class
    where TRepository : class
{
    protected readonly TRepository Repository;
    protected readonly ILogger? Logger;

    protected GenericCollectionQueryHandler(TRepository repository, ILogger? logger = null)
    {
        Repository = repository;
        Logger = logger;
    }

    /// <summary>
    /// Apply filters to the queryable based on the query object properties
    /// </summary>
    protected IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> queryable, TQuery query)
    {
        var queryType = typeof(TQuery);
        var properties = queryType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var queryProperty in properties)
        {
            var filterValue = queryProperty.GetValue(query);
            
            // Skip null values (optional parameters not provided)
            if (filterValue == null)
                continue;

            var propertyName = queryProperty.Name;

            // Special handling for date range filters (StartDate, EndDate)
            if (propertyName.Equals("StartDate", StringComparison.OrdinalIgnoreCase) || 
                propertyName.Equals("EndDate", StringComparison.OrdinalIgnoreCase))
            {
                var actualPropertyName = "Date";
                var actualProperty = typeof(TEntity).GetProperty(actualPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                
                if (actualProperty != null && actualProperty.PropertyType == typeof(DateOnly))
                {
                    var dateParameter = Expression.Parameter(typeof(TEntity), "x");
                    var actualPropertyAccess = Expression.Property(dateParameter, actualProperty);
                    var dateValue = (DateOnly)filterValue;
                    Expression dateComparison;
                    
                    if (propertyName.Equals("StartDate", StringComparison.OrdinalIgnoreCase))
                    {
                        dateComparison = Expression.GreaterThanOrEqual(actualPropertyAccess, Expression.Constant(dateValue));
                    }
                    else
                    {
                        dateComparison = Expression.LessThanOrEqual(actualPropertyAccess, Expression.Constant(dateValue));
                    }
                    
                    var dateLambda = Expression.Lambda<Func<TEntity, bool>>(dateComparison, dateParameter);
                    queryable = queryable.Where(dateLambda);
                }
                continue;
            }

            // Find matching property in entity
            var entityProperty = typeof(TEntity).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            
            if (entityProperty == null)
                continue;

            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var propertyAccess = Expression.Property(parameter, entityProperty);
            Expression comparison;

            // Handle different types
            if (entityProperty.PropertyType == typeof(string))
            {
                // String contains (case-insensitive)
                var stringValue = filterValue.ToString();
                if (!string.IsNullOrWhiteSpace(stringValue))
                {
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) })!;
                    var valueExpression = Expression.Constant(stringValue, typeof(string));
                    var comparisonExpression = Expression.Constant(StringComparison.OrdinalIgnoreCase);
                    comparison = Expression.Call(propertyAccess, containsMethod, valueExpression, comparisonExpression);
                    
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);
                    queryable = queryable.Where(lambda);
                }
            }
            else if (entityProperty.PropertyType == typeof(bool))
            {
                // Boolean equals
                var value = Convert.ToBoolean(filterValue);
                comparison = Expression.Equal(propertyAccess, Expression.Constant(value));
                var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);
                queryable = queryable.Where(lambda);
            }
            else if (entityProperty.PropertyType == typeof(int) || entityProperty.PropertyType == typeof(short))
            {
                // Numeric equals
                var value = Convert.ChangeType(filterValue, entityProperty.PropertyType);
                comparison = Expression.Equal(propertyAccess, Expression.Constant(value, entityProperty.PropertyType));
                var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);
                queryable = queryable.Where(lambda);
            }
        }

        return queryable;
    }

    /// <summary>
    /// Apply entity-specific ordering to the query
    /// </summary>
    protected abstract IQueryable<TEntity> ApplyOrdering(IQueryable<TEntity> query);

    /// <summary>
    /// Convert entity to DTO
    /// </summary>
    protected abstract TDto MapToDto(TEntity entity);
}

/// <summary>
/// Generic query handler for Bucket entities
/// Orders by Name ascending
/// </summary>
public class BucketCollectionQueryHandler : GenericCollectionQueryHandler<Bucket, BucketDto, BucketCollectionQuery, IBucketRepository>
{
    private readonly Mappers.BucketMapper _mapper = new();

    public BucketCollectionQueryHandler(IBucketRepository repository, ILogger<BucketCollectionQueryHandler>? logger = null)
        : base(repository, logger)
    {
    }

    public async Task<IEnumerable<BucketDto>> Handle(BucketCollectionQuery query)
    {
        var queryable = Repository.AsQueryable();
        queryable = ApplyFilters(queryable, query);
        queryable = ApplyOrdering(queryable);
        var result = queryable.Select(MapToDto).ToArray();
        return await Task.FromResult(result);
    }

    protected override IQueryable<Bucket> ApplyOrdering(IQueryable<Bucket> query)
    {
        return query.OrderBy(b => b.Name);
    }

    protected override BucketDto MapToDto(Bucket entity)
    {
        return _mapper.ToDto(entity);
    }
}

/// <summary>
/// Generic query handler for MonthlyBucket entities
/// Orders by Name ascending (via Bucket relationship, but we'll use Year/Month as proxy)
/// </summary>
public class MonthlyBucketCollectionQueryHandler : GenericCollectionQueryHandler<MonthlyBucket, MonthlyBucketDto, MonthlyBucketCollectionQuery, IMonthlyBucketRepository>
{
    private readonly Mappers.MonthlyBucketMapper _mapper = new();

    public MonthlyBucketCollectionQueryHandler(IMonthlyBucketRepository repository, ILogger<MonthlyBucketCollectionQueryHandler>? logger = null)
        : base(repository, logger)
    {
    }

    public async Task<IEnumerable<MonthlyBucketDto>> Handle(MonthlyBucketCollectionQuery query)
    {
        var queryable = Repository.AsQueryable();
        queryable = ApplyFilters(queryable, query);
        queryable = ApplyOrdering(queryable);
        var result = queryable.Select(MapToDto).ToArray();
        return await Task.FromResult(result);
    }

    protected override IQueryable<MonthlyBucket> ApplyOrdering(IQueryable<MonthlyBucket> query)
    {
        // MonthlyBucket doesn't have Name property, order by Description which comes from Bucket
        return query.OrderBy(mb => mb.Description);
    }

    protected override MonthlyBucketDto MapToDto(MonthlyBucket entity)
    {
        return _mapper.ToDto(entity);
    }
}

/// <summary>
/// Generic query handler for Spending entities
/// Orders by Description ascending
/// </summary>
public class SpendingCollectionQueryHandler : GenericCollectionQueryHandler<Spending, SpendingDto, SpendingCollectionQuery, ISpendingRepository>
{
    private readonly Mappers.SpendingMapper _mapper = new();

    public SpendingCollectionQueryHandler(ISpendingRepository repository, ILogger<SpendingCollectionQueryHandler>? logger = null)
        : base(repository, logger)
    {
    }

    public async Task<IEnumerable<SpendingDto>> Handle(SpendingCollectionQuery query)
    {
        var queryable = Repository.AsQueryable();
        queryable = ApplyFilters(queryable, query);
        queryable = ApplyOrdering(queryable);
        var result = queryable.Select(MapToDto).ToArray();
        return await Task.FromResult(result);
    }

    protected override IQueryable<Spending> ApplyOrdering(IQueryable<Spending> query)
    {
        return query.OrderBy(s => s.Description);
    }

    protected override SpendingDto MapToDto(Spending entity)
    {
        return _mapper.ToDto(entity);
    }
}

/// <summary>
/// Generic query handler for MonthlySpending entities
/// Orders by Date descending
/// </summary>
public class MonthlySpendingCollectionQueryHandler : GenericCollectionQueryHandler<MonthlySpending, MonthlySpendingDto, MonthlySpendingCollectionQuery, IMonthlySpendingRepository>
{
    private readonly Mappers.MonthlySpendingMapper _mapper = new();

    public MonthlySpendingCollectionQueryHandler(IMonthlySpendingRepository repository, ILogger<MonthlySpendingCollectionQueryHandler>? logger = null)
        : base(repository, logger)
    {
    }

    public async Task<IEnumerable<MonthlySpendingDto>> Handle(MonthlySpendingCollectionQuery query)
    {
        var queryable = Repository.AsQueryable();
        queryable = ApplyFilters(queryable, query);
        queryable = ApplyOrdering(queryable);
        var result = queryable.Select(MapToDto).ToArray();
        return await Task.FromResult(result);
    }

    protected override IQueryable<MonthlySpending> ApplyOrdering(IQueryable<MonthlySpending> query)
    {
        return query.OrderByDescending(ms => ms.Date);
    }

    protected override MonthlySpendingDto MapToDto(MonthlySpending entity)
    {
        return _mapper.ToDto(entity);
    }
}

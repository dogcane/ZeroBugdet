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


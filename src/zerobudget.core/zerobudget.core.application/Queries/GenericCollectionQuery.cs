namespace zerobudget.core.application.Queries;

/// <summary>
/// Generic query for filtering and retrieving collections of entities.
/// Excludes Identity property as per requirement.
/// </summary>
/// <typeparam name="TDto">The DTO type to return</typeparam>
public record GenericCollectionQuery<TDto>
{
    public Dictionary<string, object?> Filters { get; init; } = new();
}

// Specific query types for each entity
public record BucketCollectionQuery : GenericCollectionQuery<DTOs.BucketDto>;
public record MonthlyBucketCollectionQuery : GenericCollectionQuery<DTOs.MonthlyBucketDto>;
public record SpendingCollectionQuery : GenericCollectionQuery<DTOs.SpendingDto>;
public record MonthlySpendingCollectionQuery : GenericCollectionQuery<DTOs.MonthlySpendingDto>;

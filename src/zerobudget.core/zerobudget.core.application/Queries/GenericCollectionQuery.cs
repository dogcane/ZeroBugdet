namespace zerobudget.core.application.Queries;

/// <summary>
/// Query for filtering and retrieving Bucket collections.
/// Excludes Identity property as per requirement.
/// </summary>
public record BucketCollectionQuery(
    string? Name = null,
    string? Description = null,
    bool? Enabled = null
);

/// <summary>
/// Query for filtering and retrieving MonthlyBucket collections.
/// Excludes Identity property as per requirement.
/// </summary>
public record MonthlyBucketCollectionQuery(
    short? Year = null,
    short? Month = null,
    int? BucketId = null,
    string? Description = null
);

/// <summary>
/// Query for filtering and retrieving Spending collections.
/// Excludes Identity property as per requirement.
/// </summary>
public record SpendingCollectionQuery(
    int? BucketId = null,
    string? Description = null,
    string? Owner = null,
    bool? Enabled = null
);

/// <summary>
/// Query for filtering and retrieving MonthlySpending collections.
/// Excludes Identity property as per requirement.
/// </summary>
public record MonthlySpendingCollectionQuery(
    int? MonthlyBucketId = null,
    string? Description = null,
    string? Owner = null,
    DateOnly? StartDate = null,
    DateOnly? EndDate = null
);

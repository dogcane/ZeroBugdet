namespace zerobudget.core.application.DTOs;

public record MonthlyBucketDto(
    int Id,
    short Year,
    short Month,
    decimal Balance,
    string Description,
    decimal Limit,
    int BucketId
);

public record CreateMonthlyBucketDto(
    short Year,
    short Month,
    int BucketId
);

public record UpdateMonthlyBucketDto(
    int Id,
    short Year,
    short Month,
    decimal Balance,
    decimal Limit
);

public record GenerateMonthlyDataResult(
    short Year,
    short Month,
    int MonthlyBucketsCreated,
    int MonthlySpendingsCreated
);
namespace zerobudget.core.application.DTOs;

public record MonthlySpendingDto(
    int Id,
    DateOnly Date,
    int MonthlyBucketId,
    string Description,
    decimal Amount,
    string Owner,
    string[] Tags
);

public record CreateMonthlySpendingDto(
    DateOnly Date,
    int MonthlyBucketId,
    string Description,
    decimal Amount,
    string Owner,
    string[] Tags
);

public record UpdateMonthlySpendingDto(
    int Id,
    DateOnly Date,
    string Description,
    decimal Amount,
    string Owner,
    string[] Tags
);
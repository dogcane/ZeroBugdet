namespace zerobudget.core.application.DTOs;

public record SpendingDto(
    int Id,
    DateOnly Date,
    int BucketId,
    string Description,
    decimal Amount,
    string Owner,
    int[] TagIds
);

public record CreateSpendingDto(
    DateOnly Date,
    int BucketId,
    string Description,
    decimal Amount,
    string Owner,
    int[] TagIds
);

public record UpdateSpendingDto(
    int Id,
    DateOnly Date,
    string Description,
    decimal Amount,
    string Owner,
    int[] TagIds
);
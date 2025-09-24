namespace zerobudget.core.application.Commands;

public record CreateMonthlySpendingCommand(
    DateOnly Date,
    int MonthlyBucketId,
    string Description,
    decimal Amount,
    string Owner,
    int[] TagIds
);

public record UpdateMonthlySpendingCommand(
    int Id,
    DateOnly Date,
    string Description,
    decimal Amount,
    string Owner,
    int[] TagIds
);

public record DeleteMonthlySpendingCommand(
    int Id
);
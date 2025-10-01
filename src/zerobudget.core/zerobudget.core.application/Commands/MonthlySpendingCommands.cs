namespace zerobudget.core.application.Commands;

public record CreateMonthlySpendingCommand(
    DateOnly Date,
    int MonthlyBucketId,
    string Description,
    decimal Amount,
    string Owner,
    string[] TagNames
);

public record UpdateMonthlySpendingCommand(
    int Id,
    DateOnly Date,
    string Description,
    decimal Amount,
    string Owner,
    string[] TagNames
);

public record DeleteMonthlySpendingCommand(
    int Id
);
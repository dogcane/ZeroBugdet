namespace zerobudget.core.application.Commands;

public record CreateSpendingCommand(
    DateOnly Date,
    int BucketId,
    string Description,
    decimal Amount,
    string Owner,
    string[] TagNames
);

public record UpdateSpendingCommand(
    int Id,
    DateOnly Date,
    string Description,
    decimal Amount,
    string Owner,
    string[] TagNames
);

public record DeleteSpendingCommand(
    int Id
);

public record EnableSpendingCommand(
    int Id
);
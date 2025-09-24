namespace zerobudget.core.application.Commands;

public record CreateSpendingCommand(
    DateOnly Date,
    int BucketId,
    string Description,
    decimal Amount,
    string Owner,
    int[] TagIds
);

public record UpdateSpendingCommand(
    int Id,
    DateOnly Date,
    string Description,
    decimal Amount,
    string Owner,
    int[] TagIds
);

public record DeleteSpendingCommand(
    int Id
);
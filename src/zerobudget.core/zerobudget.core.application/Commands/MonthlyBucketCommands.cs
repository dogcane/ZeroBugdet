namespace zerobudget.core.application.Commands;

public record CreateMonthlyBucketCommand(
    short Year,
    short Month,
    int BucketId
);

public record UpdateMonthlyBucketCommand(
    int Id,
    short Year,
    short Month,
    decimal Balance,
    decimal Limit
);

public record DeleteMonthlyBucketCommand(
    int Id
);
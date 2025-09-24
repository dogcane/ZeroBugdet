namespace zerobudget.core.application.Commands;

public record CreateBucketCommand(
    string Name,
    string Description,
    decimal DefaultLimit
);

public record UpdateBucketCommand(
    int Id,
    string Name,
    string Description,
    decimal DefaultLimit
);

public record DeleteBucketCommand(
    int Id
);

public record EnableBucketCommand(
    int Id
);
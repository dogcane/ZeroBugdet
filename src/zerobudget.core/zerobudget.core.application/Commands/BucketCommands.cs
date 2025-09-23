using zerobudget.core.application.DTOs;

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
namespace zerobudget.core.application.DTOs;

public record BucketDto(
    int Id,
    string Name,
    string Description,
    decimal DefaultLimit,
    decimal DefaultBalance
);

public record CreateBucketDto(
    string Name,
    string Description,
    decimal DefaultLimit
);

public record UpdateBucketDto(
    int Id,
    string Name,
    string Description,
    decimal DefaultLimit
);
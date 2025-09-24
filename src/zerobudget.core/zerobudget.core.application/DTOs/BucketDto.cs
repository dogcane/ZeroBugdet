namespace zerobudget.core.application.DTOs;

public record BucketDto(
    int Id,
    string Name,
    string Description,
    decimal DefaultLimit,
    decimal DefaultBalance,
    bool Enabled
);
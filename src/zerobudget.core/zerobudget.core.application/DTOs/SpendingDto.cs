namespace zerobudget.core.application.DTOs;

public record SpendingDto(
    int Id,
    int BucketId,
    string Description,
    decimal Amount,
    string Owner,
    string[] Tags
);
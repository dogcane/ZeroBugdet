namespace zerobudget.core.application.Queries;

public record GetMonthlySpendingByIdQuery(int Id);
public record GetMonthlySpendingsQuery(int? MonthlyBucketId = null, string? Description = null, string? Owner = null, DateOnly? StartDate = null, DateOnly? EndDate = null);

namespace zerobudget.core.application.Queries;

public record GetMonthlySpendingByIdQuery(int Id);

public record GetAllMonthlySpendingsQuery();

public record GetMonthlySpendingsByMonthlyBucketIdQuery(int MonthlyBucketId);

public record GetMonthlySpendingsByDateRangeQuery(DateOnly StartDate, DateOnly EndDate);

public record GetMonthlySpendingsByOwnerQuery(string Owner);
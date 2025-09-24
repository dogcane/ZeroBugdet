namespace zerobudget.core.application.Queries;

public record GetSpendingByIdQuery(int Id);

public record GetAllSpendingsQuery();

public record GetSpendingsByBucketIdQuery(int BucketId);

public record GetSpendingsByDateRangeQuery(DateOnly StartDate, DateOnly EndDate);

public record GetSpendingsByOwnerQuery(string Owner);
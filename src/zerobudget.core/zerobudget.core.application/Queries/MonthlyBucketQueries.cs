namespace zerobudget.core.application.Queries;

public record GetMonthlyBucketByIdQuery(int Id);

public record GetAllMonthlyBucketsQuery();

public record GetMonthlyBucketsByYearMonthQuery(short Year, short Month);

public record GetMonthlyBucketsByBucketIdQuery(int BucketId);
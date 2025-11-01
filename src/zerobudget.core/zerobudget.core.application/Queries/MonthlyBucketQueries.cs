namespace zerobudget.core.application.Queries;

public record GetMonthlyBucketByIdQuery(int Id);
public record GetMonthlyBucketsQuery(short? Year = null, short? Month = null, int? BucketId = null, string? Description = null);

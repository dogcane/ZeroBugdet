namespace zerobudget.core.application.Queries;

public record GetBucketByIdQuery(int Id);
public record GetBucketsQuery(string? Name = null, string? Description = null, bool Enabled = true);

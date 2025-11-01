namespace zerobudget.core.application.Queries;

public record GetSpendingByIdQuery(int Id);
public record GetSpendingsQuery(int? BucketId = null, string? Description = null, string? Owner = null, bool? Enabled = null);

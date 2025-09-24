using zerobudget.core.application.DTOs;

namespace zerobudget.core.application.Queries;

public record GetBucketByIdQuery(int Id);

public record GetBucketsByNameQuery(string Name, string Description, bool Enabled = true);
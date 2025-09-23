using zerobudget.core.application.DTOs;

namespace zerobudget.core.application.Queries;

public record GetBucketByIdQuery(int Id);

public record GetAllBucketsQuery();

public record GetBucketsByNameQuery(string Name);
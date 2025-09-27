using ECO;

namespace zerobudget.core.domain;

public interface IBucketRepository : IRepository<Bucket, int>
{
    // Use LINQ queries with the base IRepository.Query() method
}

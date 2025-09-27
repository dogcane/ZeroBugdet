using ECO;

namespace zerobudget.core.domain;

public interface IMonthlyBucketRepository : IRepository<MonthlyBucket, int>
{
    // Use LINQ queries with the base IRepository.Query() method
}

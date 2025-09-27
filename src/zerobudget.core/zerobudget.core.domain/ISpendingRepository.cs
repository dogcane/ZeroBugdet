using ECO;

namespace zerobudget.core.domain;

public interface ISpendingRepository : IRepository<Spending, int>
{
    // Use LINQ queries with the base IRepository.Query() method
}

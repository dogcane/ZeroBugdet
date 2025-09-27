using ECO;

namespace zerobudget.core.domain;

public interface IMonthlySpendingRepository : IRepository<MonthlySpending, int>
{
    // Use LINQ queries with the base IRepository.Query() method
}

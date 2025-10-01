using ECO.Data;
using ECO.Providers.EntityFramework;
using zerobudget.core.domain;

namespace zerobudget.core.infrastructure.data;

public class MonthlySpendingEFRepository(IDataContext dataContext) : EntityFrameworkRepository<MonthlySpending, int>(dataContext), IMonthlySpendingRepository
{
    // Use LINQ queries with Query() method from base class
}

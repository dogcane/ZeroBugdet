using ECO.Data;
using ECO.Providers.EntityFramework;

namespace zerobudget.core.infrastructure.data;

public class MonthlySpendingEFRepository(IDataContext dataContext) : EntityFrameworkRepository<zerobudget.core.domain.Spending, int>(dataContext), zerobudget.core.domain.IMonthlySpendingRepository
{
    // Use LINQ queries with Query() method from base class
}

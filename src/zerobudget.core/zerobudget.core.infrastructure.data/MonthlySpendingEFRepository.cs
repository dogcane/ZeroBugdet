using ECO.Data;
using ECO.Providers.EntityFramework;

namespace zerobudget.core.infrastructure.data;

public class MonthlySpendingEFRepository(IDataContext dataContext) : EntityFrameworkRepository<zerobudget.core.domain.MonthlySpending, int>(dataContext), zerobudget.core.domain.IMonthlySpendingRepository
{
    // Add custom methods if needed
}

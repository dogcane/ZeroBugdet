using ECO.Data;
using ECO.Providers.EntityFramework;

namespace zerobudget.core.infrastructure.data;

public class SpendingEFRepository(IDataContext dataContext) : EntityFrameworkRepository<zerobudget.core.domain.Spending, int>(dataContext), zerobudget.core.domain.ISpendingRepository
{
    // Add custom methods if needed
}

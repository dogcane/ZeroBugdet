using ECO.Data;
using ECO.Providers.EntityFramework;

namespace zerobudget.core.infrastructure.data;

public class MonthlyBucketEFRepository(IDataContext dataContext) : EntityFrameworkRepository<zerobudget.core.domain.MonthlyBucket, int>(dataContext), zerobudget.core.domain.IMonthlyBucketRepository
{
    // Add custom methods if needed
}

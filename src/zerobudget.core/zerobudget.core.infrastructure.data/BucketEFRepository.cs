using ECO.Data;
using ECO.Providers.EntityFramework;

namespace zerobudget.core.infrastructure.data;

public class BucketEFRepository(IDataContext dataContext) : EntityFrameworkRepository<zerobudget.core.domain.Bucket, int>(dataContext), zerobudget.core.domain.IBucketRepository
{
    // Add custom methods if needed
}

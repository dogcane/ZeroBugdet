using ECO.Data;
using ECO.Providers.EntityFramework;

namespace zerobudget.core.infrastructure.data;

public class TagEFRepository(IDataContext dataContext) : EntityFrameworkRepository<zerobudget.core.domain.Tag, int>(dataContext), zerobudget.core.domain.ITagRepository
{
    // Add custom methods if needed
}

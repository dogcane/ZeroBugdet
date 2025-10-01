using ECO.Data;
using ECO.Providers.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace zerobudget.core.infrastructure.data;

public class TagEFRepository(IDataContext dataContext) : EntityFrameworkRepository<zerobudget.core.domain.Tag, int>(dataContext), zerobudget.core.domain.ITagRepository
{
    private new ZBDbContext DbContext => (ZBDbContext)dataContext.GetContext();

    public async Task<zerobudget.core.domain.Tag?> GetByNameAsync(string name)
    {
        return await Query().FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<HashSet<string>> GetUsedTagNamesAsync()
    {
        // Use PostgreSQL-optimized query to get distinct tag names from Spendings
        // This uses JSONB array operations which are efficient in PostgreSQL
        var usedTagNames = await DbContext.Spendings
            .Where(s => s.Tags.Length > 0)
            .SelectMany(s => s.Tags)
            .Distinct()
            .ToListAsync();
        
        return new HashSet<string>(usedTagNames);
    }

    public async Task<int> RemoveUnusedTagsAsync()
    {
        // Get all used tag names
        var usedTagNames = await GetUsedTagNamesAsync();
        
        // Find tags that are not in the used list
        // Using raw SQL for better performance with PostgreSQL
        var unusedTags = await Query()
            .Where(t => !usedTagNames.Contains(t.Name))
            .ToListAsync();
        
        // Remove unused tags
        foreach (var tag in unusedTags)
        {
            await RemoveAsync(tag);
        }
        
        return unusedTags.Count;
    }
}

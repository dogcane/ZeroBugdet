using ECO.Data;
using ECO.Providers.EntityFramework;
using Microsoft.EntityFrameworkCore;
using zerobudget.core.domain;

namespace zerobudget.core.infrastructure.data;

public class TagEFRepository(IDataContext dataContext) : EntityFrameworkRepository<Tag, int>(dataContext), ITagRepository
{
    public async Task<HashSet<string>> GetUsedTagNamesAsync()
    {
        // Use PostgreSQL-optimized query to get distinct tag names from Spendings
        // This uses JSONB array operations which are efficient in PostgreSQL
        var usedTagNames = await DbContext.Set<Spending>()
            .Where(s => s.Tags.Length > 0)
            .SelectMany(s => s.Tags)
            .Distinct()
            .ToListAsync();
        
        return [.. usedTagNames];
    }

    public async Task<int> RemoveUnusedTagsAsync()
    {
        // Get all used tag names
        var usedTagNames = await GetUsedTagNamesAsync();
        
        // Find tags that are not in the used list
        // Using raw SQL for better performance with PostgreSQL
        var unusedTags = await this.AsQueryable<Tag>()
            .Where(t => !usedTagNames.Contains(t.Name, StringComparer.OrdinalIgnoreCase))
            .ToListAsync();
        
        // Remove unused tags
        foreach (var tag in unusedTags)
        {
            await RemoveAsync(tag);
        }
        
        return unusedTags.Count;
    }
}

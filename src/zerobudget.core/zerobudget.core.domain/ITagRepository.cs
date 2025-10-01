using ECO;

namespace zerobudget.core.domain;

public interface ITagRepository : IRepository<Tag, int>
{
    /// <summary>
    /// Find a tag by its name
    /// </summary>
    Task<Tag?> GetByNameAsync(string name);
    
    /// <summary>
    /// Get all tag names that are currently in use by spendings
    /// </summary>
    Task<HashSet<string>> GetUsedTagNamesAsync();
    
    /// <summary>
    /// Remove tags that are not associated with any spending
    /// </summary>
    Task<int> RemoveUnusedTagsAsync();
}

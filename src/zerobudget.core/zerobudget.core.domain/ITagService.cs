using System.Collections.Generic;
using System.Threading.Tasks;

namespace zerobudget.core.domain
{
    public interface ITagService
    {
        /// <summary>
        /// Ensures all tag names exist in the repository by name, creating new ones if needed.
        /// Returns the list of Tag entities corresponding to the provided names.
        /// </summary>
        /// <param name="tagNames">Array of tag names to ensure.</param>
        /// <returns>List of Tag entities.</returns>
        Task<List<Tag>> EnsureTagsByNameAsync(string[] tagNames);
    }
}

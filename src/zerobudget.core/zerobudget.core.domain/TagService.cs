using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zerobudget.core.domain
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        /// <inheritdoc />
        public async Task<List<Tag>> EnsureTagsByNameAsync(string[] tagNames)
        {
            var tags = new List<Tag>();

            foreach (var tagName in tagNames.NormalizeTagNames())
            {
                var tag = _tagRepository.Where(t => t.Name == tagName).FirstOrDefault();
                if (tag == null)
                {
                    var tagResult = Tag.Create(tagName);
                    if (tagResult.Success)
                    {
                        tag = tagResult.Value!;
                        await _tagRepository.AddAsync(tag);
                        tags.Add(tag);
                    }
                }
                else
                {
                    tags.Add(tag);
                }
            }

            return tags;
        }
    }
}

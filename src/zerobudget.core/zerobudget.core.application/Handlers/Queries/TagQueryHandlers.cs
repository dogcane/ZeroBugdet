using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Queries;

public class TagQueryHandlers
{
    private readonly ITagRepository _tagRepository;

    public TagQueryHandlers(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<TagDto?> Handle(GetTagByIdQuery query)
    {
        var tag = await _tagRepository.GetByIdAsync(query.Id);
        if (tag == null)
            return null;

        return new TagDto(
            tag.Identity,
            tag.Name
        );
    }

    public async Task<IEnumerable<TagDto>> Handle(GetAllTagsQuery query)
    {
        var tags = await _tagRepository.GetAllAsync();
        return tags.Select(tag => new TagDto(
            tag.Identity,
            tag.Name
        ));
    }

    public async Task<IEnumerable<TagDto>> Handle(GetTagsByNameQuery query)
    {
        var tags = await _tagRepository.GetAllAsync();
        var filteredTags = tags.Where(t => t.Name.Contains(query.Name, StringComparison.OrdinalIgnoreCase));
        
        return filteredTags.Select(tag => new TagDto(
            tag.Identity,
            tag.Name
        ));
    }
}
using JasperFx.Core.Reflection;
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Queries;

public class TagQueryHandlers(ITagRepository tagRepository)
{
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<TagDto?> Handle(GetTagByIdQuery query)
    {
        var tag = await _tagRepository.LoadAsync(query.Id);
        if (tag == null)
            return null;

        return new TagDto(
            tag.Identity,
            tag.Name
        );
    }

    public async Task<IEnumerable<TagDto>> Handle(GetAllTagsQuery query)
    {
        var tags = _tagRepository.AsQueryable();
        return await Task.FromResult(tags.Select(tag => new TagDto(
            tag.Identity,
            tag.Name
        )));
    }

    public async Task<IEnumerable<TagDto>> Handle(GetTagsByNameQuery query)
    {
        var tags = _tagRepository.AsQueryable   ();
        var filteredTags = tags.Where(t => t.Name.Contains(query.Name, StringComparison.OrdinalIgnoreCase));

        return await Task.FromResult(filteredTags.Select(tag => new TagDto(
            tag.Identity,
            tag.Name
        )));
    }
}
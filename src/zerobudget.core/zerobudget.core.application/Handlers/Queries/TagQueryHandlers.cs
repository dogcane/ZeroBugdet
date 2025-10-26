using JasperFx.Core.Reflection;
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;
using Microsoft.Extensions.Logging;

namespace zerobudget.core.application.Handlers.Queries;

public class GetTagByIdQueryHandler(ITagRepository tagRepository, ILogger<GetTagByIdQueryHandler>? logger = null)
{
    private readonly TagMapper _mapper = new();

    public async Task<TagDto?> Handle(GetTagByIdQuery query)
    {
        var tag = await tagRepository.LoadAsync(query.Id);
        if (tag == null)
            return null;
        return _mapper.ToDto(tag);
    }
}

public class GetAllTagsQueryHandler(ITagRepository tagRepository, ILogger<GetAllTagsQueryHandler>? logger = null)
{
    private readonly TagMapper _mapper = new();

    public async Task<IEnumerable<TagDto>> Handle(GetAllTagsQuery query)
    {
        var tags = tagRepository.AsQueryable();
        return await Task.FromResult(tags.Select(_mapper.ToDto));
    }
}

public class GetTagsByNameQueryHandler(ITagRepository tagRepository, ILogger<GetTagsByNameQueryHandler>? logger = null)
{
    private readonly TagMapper _mapper = new();

    public async Task<IEnumerable<TagDto>> Handle(GetTagsByNameQuery query)
    {
        var tags = tagRepository.AsQueryable();
        var filteredTags = tags.Where(t => t.Name.Contains(query.Name, StringComparison.OrdinalIgnoreCase));
        return await Task.FromResult(filteredTags.Select(_mapper.ToDto));
    }
}

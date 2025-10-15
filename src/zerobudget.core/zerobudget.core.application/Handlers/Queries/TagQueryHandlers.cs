using JasperFx.Core.Reflection;
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;
using Microsoft.Extensions.Logging;

namespace zerobudget.core.application.Handlers.Queries;

public class TagQueryHandlers(ITagRepository tagRepository, ILogger<TagQueryHandlers>? logger = null)
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly ILogger<TagQueryHandlers>? _logger = logger;
    private readonly TagMapper _mapper = new();

    public async Task<TagDto?> Handle(GetTagByIdQuery query)
    {
        var tag = await _tagRepository.LoadAsync(query.Id);
        if (tag == null)
            return null;
        return _mapper.ToDto(tag);
    }

    public async Task<IEnumerable<TagDto>> Handle(GetAllTagsQuery query)
    {
        var tags = _tagRepository.AsQueryable();
        return await Task.FromResult(tags.Select(_mapper.ToDto));
    }

    public async Task<IEnumerable<TagDto>> Handle(GetTagsByNameQuery query)
    {
        var tags = _tagRepository.AsQueryable();
        var filteredTags = tags.Where(t => t.Name.Contains(query.Name, StringComparison.OrdinalIgnoreCase));
        return await Task.FromResult(filteredTags.Select(_mapper.ToDto));
    }
}
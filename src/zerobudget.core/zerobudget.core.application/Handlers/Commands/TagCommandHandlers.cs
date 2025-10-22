using Microsoft.Extensions.Logging;
using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;

namespace zerobudget.core.application.Handlers.Commands;

public class CreateTagCommandHandler(ITagRepository tagRepository, ILogger<CreateTagCommandHandler>? logger = null)
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly ILogger<CreateTagCommandHandler>? _logger = logger;
    private readonly TagMapper _mapper = new TagMapper();

    public async Task<TagDto> Handle(CreateTagCommand command)
    {
        var tagResult = Tag.Create(command.Name);
        if (!tagResult.Success)
            throw new InvalidOperationException(string.Join(", ", tagResult.Errors.Select(e => e.Description)));

        var tag = tagResult.Value!;
        await _tagRepository.AddAsync(tag);

        return _mapper.ToDto(tag);
    }
}

public class DeleteTagCommandHandler(ITagRepository tagRepository, ILogger<DeleteTagCommandHandler>? logger = null)
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly ILogger<DeleteTagCommandHandler>? _logger = logger;

    public async Task Handle(DeleteTagCommand command)
    {
        var tag = await _tagRepository.LoadAsync(command.Id);
        if (tag == null)
            throw new InvalidOperationException("Tag not found");

        await _tagRepository.RemoveAsync(tag);
    }
}

public class CleanupUnusedTagsCommandHandler(ITagService tagService, ILogger<CleanupUnusedTagsCommandHandler>? logger = null)
{
    private readonly ITagService _tagService = tagService;
    private readonly ILogger<CleanupUnusedTagsCommandHandler>? _logger = logger;

    public async Task<int> Handle(CleanupUnusedTagsCommand command)
    {
        return await Task.FromResult(0);
        //return await _tagService.CleanupUnusedTagsAsync();
    }
}

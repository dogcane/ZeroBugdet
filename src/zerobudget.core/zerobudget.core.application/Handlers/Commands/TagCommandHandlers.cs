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

    public async Task<OperationResult<TagDto>> Handle(CreateTagCommand command)
    {
        var tagResult = Tag.Create(command.Name);
        if (!tagResult.Success)
            return OperationResult<TagDto>.MakeFailure(tagResult.Errors);

        var tag = tagResult.Value!;
        await _tagRepository.AddAsync(tag);

        return OperationResult<TagDto>.MakeSuccess(_mapper.ToDto(tag));
    }
}

public class DeleteTagCommandHandler(ITagRepository tagRepository, ILogger<DeleteTagCommandHandler>? logger = null)
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly ILogger<DeleteTagCommandHandler>? _logger = logger;

    public async Task<OperationResult> Handle(DeleteTagCommand command)
    {
        var tag = await _tagRepository.LoadAsync(command.Id);
        if (tag == null)
            return OperationResult.MakeFailure(ErrorMessage.Create("DELETE_TAG", "Tag not found"));

        await _tagRepository.RemoveAsync(tag);
        return OperationResult.MakeSuccess();
    }
}

public class CleanupUnusedTagsCommandHandler(ITagService tagService, ILogger<CleanupUnusedTagsCommandHandler>? logger = null)
{
    private readonly ITagService _tagService = tagService;
    private readonly ILogger<CleanupUnusedTagsCommandHandler>? _logger = logger;

    public async Task<OperationResult<int>> Handle(CleanupUnusedTagsCommand command)
    {
        return await Task.FromResult(OperationResult<int>.MakeSuccess(0));
        //return await _tagService.CleanupUnusedTagsAsync();
    }
}

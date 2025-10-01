using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Commands;

public class TagCommandHandlers
{
    private readonly ITagRepository _tagRepository;

    public TagCommandHandlers(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<OperationResult<TagDto>> Handle(CreateTagCommand command)
    {
        var tagResult = Tag.Create(command.Name, command.Description);
        if (!tagResult.IsSuccess)
            return OperationResult<TagDto>.MakeFailure(tagResult.Errors);

        var tag = tagResult.Value!;
        await _tagRepository.AddAsync(tag);

        return OperationResult<TagDto>.MakeSuccess(new TagDto(
            tag.Identity,
            tag.Name
        ));
    }

    public async Task<OperationResult<TagDto>> Handle(UpdateTagCommand command)
    {
        var tag = await _tagRepository.GetByIdAsync(command.Id);
        if (tag == null)
            return OperationResult<TagDto>.MakeFailure("Tag not found");

        var updateResult = tag.Update(command.Name, command.Description);
        if (!updateResult.IsSuccess)
            return OperationResult<TagDto>.MakeFailure(updateResult.Errors);

        await _tagRepository.UpdateAsync(tag);

        return OperationResult<TagDto>.MakeSuccess(new TagDto(
            tag.Identity,
            tag.Name
        ));
    }

    public async Task<OperationResult> Handle(DeleteTagCommand command)
    {
        var tag = await _tagRepository.GetByIdAsync(command.Id);
        if (tag == null)
            return OperationResult.MakeFailure("Tag not found");

        await _tagRepository.DeleteAsync(tag);
        return OperationResult.MakeSuccess();
    }
}
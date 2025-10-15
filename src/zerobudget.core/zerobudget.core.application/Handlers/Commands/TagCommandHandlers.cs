using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.domain;
using zerobudget.core.application.Mappers;

namespace zerobudget.core.application.Handlers.Commands;

public class TagCommandHandlers(ITagRepository tagRepository)
{
    private readonly ITagRepository _tagRepository = tagRepository;
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

    public async Task<OperationResult> Handle(DeleteTagCommand command)
    {
        var tag = await _tagRepository.LoadAsync(command.Id);
        if (tag == null)
            return OperationResult.MakeFailure(ErrorMessage.Create("TAG", "Tag not found"));

        await _tagRepository.RemoveAsync(tag);
        return OperationResult.MakeSuccess();
    }
}
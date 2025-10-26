using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;

namespace zerobudget.core.webapi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TagController(IMessageBus messageBus) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetById(int id)
    {
        var query = new GetTagByIdQuery(id);
        var result = await messageBus.InvokeAsync<TagDto?>(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
    {
        var query = new GetAllTagsQuery();
        var result = await messageBus.InvokeAsync<IEnumerable<TagDto>>(query);

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetByName([FromQuery] string name)
    {
        var query = new GetTagsByNameQuery(name);
        var result = await messageBus.InvokeAsync<IEnumerable<TagDto>>(query);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> Create([FromBody] CreateTagCommand command)
    {
        var result = await messageBus.InvokeAsync<TagDto>(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteTagCommand(id);
        await messageBus.InvokeAsync(command);

        return NoContent();
    }

    [HttpPost("cleanup")]
    public async Task<ActionResult> CleanupUnused()
    {
        var command = new CleanupUnusedTagsCommand();
        await messageBus.InvokeAsync(command);

        return NoContent();
    }
}

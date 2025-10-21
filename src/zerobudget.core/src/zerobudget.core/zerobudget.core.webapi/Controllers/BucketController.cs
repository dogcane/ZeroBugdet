using Microsoft.AspNetCore.Mvc;
using Wolverine;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;

namespace zerobudget.core.webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BucketController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public BucketController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BucketDto>> GetById(int id)
    {
        var query = new GetBucketByIdQuery(id);
        var result = await _messageBus.InvokeAsync<BucketDto?>(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BucketDto>>> GetByName(
        [FromQuery] string name,
        [FromQuery] string description,
        [FromQuery] bool enabled = true)
    {
        var query = new GetBucketsByNameQuery(name, description, enabled);
        var result = await _messageBus.InvokeAsync<IEnumerable<BucketDto>>(query);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<BucketDto>> Create([FromBody] CreateBucketCommand command)
    {
        var result = await _messageBus.InvokeAsync<BucketDto>(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BucketDto>> Update(int id, [FromBody] UpdateBucketRequest request)
    {
        var command = new UpdateBucketCommand(id, request.Name, request.Description, request.DefaultLimit);
        var result = await _messageBus.InvokeAsync<BucketDto>(command);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteBucketCommand(id);
        await _messageBus.InvokeAsync(command);

        return NoContent();
    }

    [HttpPatch("{id}/enable")]
    public async Task<ActionResult<BucketDto>> Enable(int id)
    {
        var command = new EnableBucketCommand(id);
        var result = await _messageBus.InvokeAsync<BucketDto>(command);

        return Ok(result);
    }
}

public record UpdateBucketRequest(
    string Name,
    string Description,
    decimal DefaultLimit
);

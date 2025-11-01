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
public class SpendingController(IMessageBus messageBus) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<SpendingDto>> GetById(int id)
    {
        var query = new GetSpendingByIdQuery(id);
        var result = await messageBus.InvokeAsync<SpendingDto?>(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpendingDto>>> GetAll()
    {
        var query = new GetSpendingsQuery();
        var result = await messageBus.InvokeAsync<IEnumerable<SpendingDto>>(query);

        return Ok(result);
    }

    [HttpGet("bucket/{bucketId}")]
    public async Task<ActionResult<IEnumerable<SpendingDto>>> GetByBucketId(int bucketId)
    {
        var query = new GetSpendingsQuery(BucketId: bucketId);
        var result = await messageBus.InvokeAsync<IEnumerable<SpendingDto>>(query);

        return Ok(result);
    }

    [HttpGet("owner/{owner}")]
    public async Task<ActionResult<IEnumerable<SpendingDto>>> GetByOwner(string owner)
    {
        var query = new GetSpendingsQuery(Owner: owner);
        var result = await messageBus.InvokeAsync<IEnumerable<SpendingDto>>(query);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<SpendingDto>> Create([FromBody] CreateSpendingCommand command)
    {
        var result = await messageBus.InvokeAsync<SpendingDto>(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SpendingDto>> Update(int id, [FromBody] UpdateSpendingRequest request)
    {
        var command = new UpdateSpendingCommand(id, request.Date, request.Description, request.Amount, request.Owner, request.TagNames);
        var result = await messageBus.InvokeAsync<SpendingDto>(command);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteSpendingCommand(id);
        await messageBus.InvokeAsync(command);

        return NoContent();
    }

    [HttpPatch("{id}/enable")]
    public async Task<ActionResult<SpendingDto>> Enable(int id)
    {
        var command = new EnableSpendingCommand(id);
        var result = await messageBus.InvokeAsync<SpendingDto>(command);

        return Ok(result);
    }
}

public record UpdateSpendingRequest(
    DateOnly Date,
    string Description,
    decimal Amount,
    string Owner,
    string[] TagNames
);

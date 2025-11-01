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
public class MonthlySpendingController(IMessageBus messageBus) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<MonthlySpendingDto>> GetById(int id)
    {
        var query = new GetMonthlySpendingByIdQuery(id);
        var result = await messageBus.InvokeAsync<MonthlySpendingDto?>(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MonthlySpendingDto>>> GetAll()
    {
        var query = new GetMonthlySpendingsQuery();
        var result = await messageBus.InvokeAsync<IEnumerable<MonthlySpendingDto>>(query);

        return Ok(result);
    }

    [HttpGet("monthly-bucket/{monthlyBucketId}")]
    public async Task<ActionResult<IEnumerable<MonthlySpendingDto>>> GetByMonthlyBucketId(int monthlyBucketId)
    {
        var query = new GetMonthlySpendingsQuery(MonthlyBucketId: monthlyBucketId);
        var result = await messageBus.InvokeAsync<IEnumerable<MonthlySpendingDto>>(query);

        return Ok(result);
    }

    [HttpGet("date-range")]
    public async Task<ActionResult<IEnumerable<MonthlySpendingDto>>> GetByDateRange(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate)
    {
        var query = new GetMonthlySpendingsQuery(StartDate: startDate, EndDate: endDate);
        var result = await messageBus.InvokeAsync<IEnumerable<MonthlySpendingDto>>(query);

        return Ok(result);
    }

    [HttpGet("owner/{owner}")]
    public async Task<ActionResult<IEnumerable<MonthlySpendingDto>>> GetByOwner(string owner)
    {
        var query = new GetMonthlySpendingsQuery(Owner: owner);
        var result = await messageBus.InvokeAsync<IEnumerable<MonthlySpendingDto>>(query);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<MonthlySpendingDto>> Create([FromBody] CreateMonthlySpendingCommand command)
    {
        var result = await messageBus.InvokeAsync<MonthlySpendingDto>(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MonthlySpendingDto>> Update(int id, [FromBody] UpdateMonthlySpendingRequest request)
    {
        var command = new UpdateMonthlySpendingCommand(id, request.Date, request.Description, request.Amount, request.Owner, request.TagNames);
        var result = await messageBus.InvokeAsync<MonthlySpendingDto>(command);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteMonthlySpendingCommand(id);
        await messageBus.InvokeAsync(command);

        return NoContent();
    }
}

public record UpdateMonthlySpendingRequest(
    DateOnly Date,
    string Description,
    decimal Amount,
    string Owner,
    string[] TagNames
);

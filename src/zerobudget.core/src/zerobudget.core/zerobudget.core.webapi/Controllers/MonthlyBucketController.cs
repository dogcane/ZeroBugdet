using Microsoft.AspNetCore.Mvc;
using Wolverine;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Queries;

namespace zerobudget.core.webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonthlyBucketController(IMessageBus messageBus) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<MonthlyBucketDto>> GetById(int id)
    {
        var query = new GetMonthlyBucketByIdQuery(id);
        var result = await messageBus.InvokeAsync<MonthlyBucketDto?>(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MonthlyBucketDto>>> GetAll()
    {
        var query = new GetAllMonthlyBucketsQuery();
        var result = await messageBus.InvokeAsync<IEnumerable<MonthlyBucketDto>>(query);

        return Ok(result);
    }

    [HttpGet("year/{year}/month/{month}")]
    public async Task<ActionResult<IEnumerable<MonthlyBucketDto>>> GetByYearMonth(short year, short month)
    {
        var query = new GetMonthlyBucketsByYearMonthQuery(year, month);
        var result = await messageBus.InvokeAsync<IEnumerable<MonthlyBucketDto>>(query);

        return Ok(result);
    }

    [HttpGet("bucket/{bucketId}")]
    public async Task<ActionResult<IEnumerable<MonthlyBucketDto>>> GetByBucketId(int bucketId)
    {
        var query = new GetMonthlyBucketsByBucketIdQuery(bucketId);
        var result = await messageBus.InvokeAsync<IEnumerable<MonthlyBucketDto>>(query);

        return Ok(result);
    }

    [HttpPost("generate")]
    public async Task<ActionResult> GenerateMonthlyData([FromBody] GenerateMonthlyDataRequest request)
    {
        var command = new GenerateMonthlyDataCommand(request.Year, request.Month);
        await messageBus.InvokeAsync(command);

        return Ok();
    }
}

public record GenerateMonthlyDataRequest(
    short Year,
    short Month
);

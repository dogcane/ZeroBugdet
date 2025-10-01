# Tag Management Scheduled Job - Usage Example

This document provides a step-by-step guide on how to configure and use the tag cleanup scheduled job in your application.

## Step 1: Configure Wolverine with Tag Maintenance

In your application's startup configuration (typically `Program.cs` or `Startup.cs`), add the tag maintenance schedule to your Wolverine configuration:

```csharp
using Wolverine;
using zerobudget.core.application.ScheduledJobs;

var builder = WebApplication.CreateBuilder(args);

// Configure Wolverine
builder.Host.UseWolverine(opts =>
{
    // Your other Wolverine configurations here...
    
    // Add tag maintenance scheduled job
    opts.ConfigureTagMaintenance();
});

var app = builder.Build();
app.Run();
```

## Step 2: Verify the Schedule (Optional)

The default schedule runs the cleanup job daily at 2:00 AM. To customize the schedule, modify the `TagMaintenanceSchedule.cs` file:

```csharp
// Example: Run every Sunday at 3:00 AM
public static void ConfigureTagMaintenance(this WolverineOptions options)
{
    options.Schedules.Schedule<CleanupUnusedTagsCommand>()
        .Weekly(DayOfWeek.Sunday, 3);
}

// Example: Run every 12 hours
public static void ConfigureTagMaintenance(this WolverineOptions options)
{
    options.Schedules.Schedule<CleanupUnusedTagsCommand>()
        .EveryHours(12);
}

// Example: Run at specific times (cron-like)
public static void ConfigureTagMaintenance(this WolverineOptions options)
{
    options.Schedules.Schedule<CleanupUnusedTagsCommand>()
        .Cron("0 2 * * *"); // Daily at 2:00 AM
}
```

## Step 3: Manual Trigger (Optional)

You can also manually trigger the tag cleanup from your code:

```csharp
using Wolverine;
using zerobudget.core.application.Commands;

public class TagMaintenanceController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public TagMaintenanceController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    [HttpPost("api/tags/cleanup")]
    public async Task<IActionResult> CleanupTags()
    {
        var result = await _messageBus.InvokeAsync<OperationResult<int>>(
            new CleanupUnusedTagsCommand());
        
        if (result.Success)
        {
            return Ok(new { RemovedCount = result.Value });
        }
        
        return BadRequest(new { Errors = result.ErrorMessages });
    }
}
```

## Step 4: Monitor Execution

The tag cleanup job logs its execution:

```
[Information] Starting cleanup of unused tags...
[Information] Cleanup completed. Removed 5 unused tags.
```

Check your application logs to monitor the cleanup job execution.

## How It Works

1. **Tag Collection**: The system collects all tag names currently used by any spending
2. **Comparison**: Compares the list of all tags in the database with the used tags
3. **Removal**: Removes tags that are not in the used list
4. **Logging**: Logs the number of removed tags

## Performance Considerations

The implementation uses PostgreSQL-optimized queries:
- **SelectMany + Distinct**: Efficiently extracts unique tag names from spending records
- **Batch Operations**: Tags are identified and removed in efficient batches
- **Minimal Locks**: Operations are designed to minimize database locks

## Safety Features

- **Transactional**: The cleanup is wrapped in a transaction
- **Read-Only Check**: Only tags with zero references are removed
- **Exception Handling**: Any errors are caught and logged
- **No False Positives**: The system double-checks tag usage before removal

## Troubleshooting

### Job Not Running
1. Check that `opts.ConfigureTagMaintenance()` is called in Wolverine configuration
2. Verify the application has scheduling enabled
3. Check application logs for any Wolverine-related errors

### Tags Not Being Removed
1. Verify tags are actually unused (check `Spendings` table)
2. Check application logs for any errors during cleanup
3. Ensure database connection is working properly

### Performance Issues
1. Monitor database query execution time
2. Consider adding indexes on the `Tags` field in Spendings table
3. Adjust the schedule to run during off-peak hours

## Advanced Configuration

### Custom Repository Implementation

If you need custom cleanup logic, you can override the `RemoveUnusedTagsAsync` method in your repository:

```csharp
public class CustomTagEFRepository : TagEFRepository
{
    public override async Task<int> RemoveUnusedTagsAsync()
    {
        // Your custom implementation here
        // For example: Archive instead of delete
        var unusedTags = await GetUnusedTagsAsync();
        foreach (var tag in unusedTags)
        {
            tag.Archive(); // Custom method
            await UpdateAsync(tag);
        }
        return unusedTags.Count;
    }
}
```

## Related Documentation

- See `TAG_MANAGEMENT_README.md` for complete feature overview
- See `ITagRepository.cs` for repository interface details
- See `TagEFRepository.cs` for implementation details

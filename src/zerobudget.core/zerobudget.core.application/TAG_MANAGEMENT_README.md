# Tag Management Features

This document describes the tag management features implemented in the ZeroBudget application.

## Features

### 1. Automatic Tag Creation
When creating or updating a spending, if you reference tags by their IDs, the system ensures they exist in the database. Tags are automatically loaded from the repository.

### 2. Tag Cleanup Scheduled Job
A scheduled job runs daily at 2:00 AM to clean up unused tags from the repository. Tags that are not associated with any spending are automatically removed to keep the database clean.

### 3. Performance Optimizations
The tag cleanup operation uses PostgreSQL-optimized queries for better performance:
- Uses LINQ with Entity Framework for efficient database queries
- Distinct tag names are retrieved using SelectMany and Distinct operations
- Batch operations for removing unused tags

## Usage

### Configuring the Scheduled Job

To enable the tag cleanup scheduled job in your application, add the following configuration to your Wolverine setup:

```csharp
using zerobudget.core.application.ScheduledJobs;

// In your Program.cs or startup configuration
builder.Host.UseWolverine(opts =>
{
    // ... other Wolverine configuration
    
    // Configure tag maintenance schedule
    opts.ConfigureTagMaintenance();
});
```

### Manual Tag Cleanup

You can also manually trigger tag cleanup by publishing the command:

```csharp
using zerobudget.core.application.Commands;

// Using Wolverine's message bus
await messageBus.InvokeAsync(new CleanupUnusedTagsCommand());
```

### Customizing the Schedule

To change when the cleanup job runs, modify the `TagMaintenanceSchedule.cs` file:

```csharp
// Run weekly on Sundays at 3:00 AM
options.Schedules.Schedule<CleanupUnusedTagsCommand>()
    .Weekly(DayOfWeek.Sunday, 3);

// Run every 6 hours
options.Schedules.Schedule<CleanupUnusedTagsCommand>()
    .EveryHours(6);
```

## Repository Methods

The `ITagRepository` interface now includes the following methods:

- `Task<Tag?> GetByNameAsync(string name)` - Find a tag by its name
- `Task<HashSet<string>> GetUsedTagNamesAsync()` - Get all tag names currently in use by spendings
- `Task<int> RemoveUnusedTagsAsync()` - Remove tags not associated with any spending (returns count of removed tags)

## Implementation Details

### SpendingCommandHandlers
The `CreateSpendingCommand` and `UpdateSpendingCommand` handlers use the `EnsureTagsExistAsync` helper method to verify that all referenced tags exist in the repository.

### TagEFRepository
The repository implementation uses Entity Framework with PostgreSQL-specific optimizations:
- Tag names are stored as arrays in the Spending table
- The `GetUsedTagNamesAsync` method uses SelectMany to efficiently extract all used tag names
- The `RemoveUnusedTagsAsync` method performs a set difference operation to find unused tags

## Database Considerations

- Tags are referenced by name in the Spending entity (stored as string arrays)
- The cleanup job is designed to be safe - it only removes tags that have zero references
- The cleanup operation is transactional - either all unused tags are removed or none are

## Monitoring

The tag cleanup job logs its activity:
- Info log when cleanup starts: "Starting cleanup of unused tags..."
- Info log when cleanup completes: "Cleanup completed. Removed {count} unused tags."
- Error log if cleanup fails with exception details

Check your application logs to monitor the cleanup job execution.

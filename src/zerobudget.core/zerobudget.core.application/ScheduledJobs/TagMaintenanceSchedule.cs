using Wolverine;
using zerobudget.core.application.Commands;

namespace zerobudget.core.application.ScheduledJobs;

/// <summary>
/// Scheduled job configuration for tag maintenance.
/// This job runs periodically to clean up unused tags from the repository.
/// </summary>
public static class TagMaintenanceSchedule
{
    /// <summary>
    /// Configure the tag cleanup job to run daily at 2 AM.
    /// This can be customized based on requirements.
    /// </summary>
    public static void ConfigureTagMaintenance(this WolverineOptions options)
    {
        // Schedule the cleanup job to run daily at 2:00 AM
        options.Schedules.Schedule<CleanupUnusedTagsCommand>()
            .Daily(2); // Run at 2:00 AM server time
    }
}

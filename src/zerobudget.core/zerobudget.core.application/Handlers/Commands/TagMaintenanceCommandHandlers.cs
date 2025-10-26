using Microsoft.Extensions.Logging;
using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Commands;

/// <summary>
/// Handler for tag maintenance operations such as cleaning up unused tags.
/// </summary>
public class TagMaintenanceCommandHandlers(
    ITagRepository tagRepository,
    ILogger<TagMaintenanceCommandHandlers> logger)
{
    /// <summary>
    /// Handles the cleanup of unused tags.
    /// This operation uses PostgreSQL-optimized queries for performance.
    /// </summary>
    public async Task<OperationResult<int>> Handle(CleanupUnusedTagsCommand command)
    {
        try
        {
            logger.LogInformation("Starting cleanup of unused tags...");
            
            var removedCount = await tagRepository.RemoveUnusedTagsAsync();
            
            logger.LogInformation("Cleanup completed. Removed {RemovedCount} unused tags.", removedCount);
            
            return OperationResult<int>.MakeSuccess(removedCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during tag cleanup");
            return OperationResult<int>.MakeFailure(ErrorMessage.Create("TAG", $"Error during tag cleanup: {ex.Message}"));
        }
    }
}

using Microsoft.Extensions.Logging;
using Resulz;
using zerobudget.core.application.Commands;
using zerobudget.core.domain;

namespace zerobudget.core.application.Handlers.Commands;

/// <summary>
/// Handler for tag maintenance operations such as cleaning up unused tags.
/// </summary>
public class TagMaintenanceCommandHandlers
{
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<TagMaintenanceCommandHandlers> _logger;

    public TagMaintenanceCommandHandlers(
        ITagRepository tagRepository,
        ILogger<TagMaintenanceCommandHandlers> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the cleanup of unused tags.
    /// This operation uses PostgreSQL-optimized queries for performance.
    /// </summary>
    public async Task<OperationResult<int>> Handle(CleanupUnusedTagsCommand command)
    {
        try
        {
            _logger.LogInformation("Starting cleanup of unused tags...");
            
            var removedCount = await _tagRepository.RemoveUnusedTagsAsync();
            
            _logger.LogInformation("Cleanup completed. Removed {RemovedCount} unused tags.", removedCount);
            
            return OperationResult<int>.MakeSuccess(removedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during tag cleanup");
            return OperationResult<int>.MakeFailure($"Error during tag cleanup: {ex.Message}");
        }
    }
}

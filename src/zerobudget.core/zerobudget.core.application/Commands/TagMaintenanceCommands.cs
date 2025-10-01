namespace zerobudget.core.application.Commands;

/// <summary>
/// Command to clean up unused tags from the repository.
/// This should be scheduled to run periodically.
/// </summary>
public record CleanupUnusedTagsCommand();

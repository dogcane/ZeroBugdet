using Microsoft.Extensions.Logging;
using Resulz;

namespace zerobudget.core.application.Middleware;

/// <summary>
/// Attribute-based exception handling middleware for individual handlers.
/// Use this as an alternative to the global middleware when you want selective error handling.
/// </summary>
/// <example>
/// [ExceptionHandler]
/// public async Task&lt;OperationResult&lt;SpendingDto&gt;&gt; Handle(CreateSpendingCommand command)
/// {
///     // Your handler logic here
/// }
/// </example>
public class ExceptionHandlerAttribute : Attribute
{
    public static async Task<OperationResult<T>> HandleAsync<T>(
        Func<Task<OperationResult<T>>> inner,
        ILogger logger)
    {
        try
        {
            return await inner();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in command handler");
            return OperationResult<T>.MakeFailure($"An unexpected error occurred: {ex.Message}");
        }
    }

    public static async Task<OperationResult> HandleAsync(
        Func<Task<OperationResult>> inner,
        ILogger logger)
    {
        try
        {
            return await inner();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in command handler");
            return OperationResult.MakeFailure($"An unexpected error occurred: {ex.Message}");
        }
    }

    public static async Task<T?> HandleQueryAsync<T>(
        Func<Task<T?>> inner,
        ILogger logger) where T : class
    {
        try
        {
            return await inner();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in query handler");
            return null;
        }
    }

    public static async Task<IEnumerable<T>> HandleQueryAsync<T>(
        Func<Task<IEnumerable<T>>> inner,
        ILogger logger)
    {
        try
        {
            return await inner();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in query handler");
            return Enumerable.Empty<T>();
        }
    }
}

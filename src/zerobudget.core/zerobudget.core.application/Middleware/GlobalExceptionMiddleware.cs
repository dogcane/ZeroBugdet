using Microsoft.Extensions.Logging;
using Resulz;
using Wolverine;

namespace zerobudget.core.application.Middleware;

/// <summary>
/// Wolverine middleware to handle exceptions across all message handlers
/// </summary>
public class GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger = null)
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

    /// <summary>
    /// Generic middleware method that handles exceptions for any handler
    /// This is the main entry point that Wolverine will call
    /// </summary>
    public async Task<T> Before<T>(IMessageContext context, Func<Task<T>> inner)
    {
        try
        {
            return await inner();
        }
        catch (Exception ex)
        {
            var messageType = context.Envelope?.Message?.GetType().Name ?? "Unknown";
            _logger?.LogError(ex, "Unhandled exception in handler for message type: {MessageType}", messageType);

            // Handle different return types appropriately
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(OperationResult<>))
            {
                // For OperationResult<TResult>, create a failure result
                var resultType = typeof(T).GetGenericArguments()[0];
                var makeFailureMethod = typeof(OperationResult<>).MakeGenericType(resultType)
                    .GetMethod("MakeFailure", new[] { typeof(string) });
                    
                return (T)makeFailureMethod!.Invoke(null, 
                    new object[] { $"An unexpected error occurred while processing {messageType}: {ex.Message}" })!;
            }
            
            if (typeof(T) == typeof(OperationResult))
            {
                // For OperationResult, create a failure result
                return (T)(object)OperationResult.MakeFailure(ErrorMessage.Create("Global", $"An unexpected error occurred while processing {messageType}: {ex.Message}"));
            }

            if (typeof(T).IsClass && Nullable.GetUnderlyingType(typeof(T)) == null)
            {
                // For reference types (DTOs, etc.), return null if possible
                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    // For IEnumerable<T>, return empty collection
                    var elementType = typeof(T).GetGenericArguments()[0];
                    var emptyMethod = typeof(Enumerable).GetMethod("Empty")!.MakeGenericMethod(elementType);
                    return (T)emptyMethod.Invoke(null, null)!;
                }

                // For nullable reference types, return null
                if (!typeof(T).IsValueType)
                {
                    return default(T)!;
                }
            }

            // For value types and other cases, re-throw with a more descriptive error
            throw new ApplicationException($"An unexpected error occurred while processing {messageType}: {ex.Message}", ex);
        }
    }
}

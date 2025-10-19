using Microsoft.Extensions.Logging;
using Resulz;
using Wolverine;

namespace zerobudget.core.application.Middleware;

/// <summary>
/// Wolverine middleware to handle exceptions across all message handlers
/// </summary>
public class GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware>? logger = null)
{
    private readonly ILogger<GlobalExceptionMiddleware>? _logger = logger;

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

            return HandleExceptionForType<T>(ex, messageType);
        }
    }

    private T HandleExceptionForType<T>(Exception ex, string messageType)
    {
        if (IsGenericOperationResult<T>())
        {
            return CreateGenericOperationResultFailure<T>(ex, messageType);
        }

        if (typeof(T) == typeof(OperationResult))
        {
            return CreateOperationResultFailure<T>(ex, messageType);
        }

        return HandleReferenceOrValueType<T>(ex, messageType);
    }

    private static bool IsGenericOperationResult<T>()
    {
        return typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(OperationResult<>);
    }

    private static T CreateGenericOperationResultFailure<T>(Exception ex, string messageType)
    {
        var resultType = typeof(T).GetGenericArguments()[0];
        var makeFailureMethod = typeof(OperationResult<>).MakeGenericType(resultType)
            .GetMethod("MakeFailure", new[] { typeof(string) });

        return (T)makeFailureMethod!.Invoke(null,
            [$"An unexpected error occurred while processing {messageType}: {ex.Message}"])!;
    }

    private static T CreateOperationResultFailure<T>(Exception ex, string messageType)
    {
        return (T)(object)OperationResult.MakeFailure(ErrorMessage.Create("Global", $"An unexpected error occurred while processing {messageType}: {ex.Message}"));
    }

    private static T HandleReferenceOrValueType<T>(Exception ex, string messageType)
    {
        if (typeof(T).IsClass && Nullable.GetUnderlyingType(typeof(T)) == null)
        {
            return HandleReferenceType<T>();
        }

        throw new InvalidOperationException($"An unexpected error occurred while processing {messageType}: {ex.Message}", ex);
    }

    private static T HandleReferenceType<T>()
    {
        if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var elementType = typeof(T).GetGenericArguments()[0];
            var emptyMethod = typeof(Enumerable).GetMethod("Empty")!.MakeGenericMethod(elementType);
            return (T)emptyMethod.Invoke(null, null)!;
        }

        if (!typeof(T).IsValueType)
        {
            return default(T)!;
        }

        return default(T)!;
    }
}

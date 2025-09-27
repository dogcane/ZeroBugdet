# Exception Middleware for Wolverine

This folder contains exception handling middleware for Wolverine message handlers in the ZeroBudget application.

## Overview

The exception middleware provides centralized error handling for all Wolverine message handlers, eliminating the need to add try-catch blocks to every handler method.

## Components

### 1. GlobalExceptionMiddleware

A global middleware that automatically wraps all message handlers and catches unhandled exceptions.

**Features:**
- Automatically catches all unhandled exceptions in message handlers
- Converts exceptions to appropriate return types:
  - `OperationResult<T>` → Returns failure with error message
  - `OperationResult` → Returns failure with error message
  - `IEnumerable<T>` → Returns empty collection
  - Nullable reference types → Returns null
  - Other types → Re-throws with descriptive message
- Logs all exceptions with message type context

**Usage:**
The middleware is automatically applied to all handlers when you call `AddZeroBudgetApplication()`. No additional configuration needed.

### 2. ExceptionHandlerAttribute (Alternative Approach)

An attribute-based approach for selective exception handling on individual handlers.

**Usage:**
```csharp
[ExceptionHandler]
public async Task<OperationResult<SpendingDto>> Handle(CreateSpendingCommand command)
{
    // Your handler logic here
    // Exceptions will be automatically caught and converted to OperationResult.MakeFailure()
}
```

## How It Works

### Before (Manual Exception Handling)

```csharp
public async Task<OperationResult<SpendingDto>> Handle(CreateSpendingCommand command)
{
    try
    {
        var bucket = await _bucketRepository.GetByIdAsync(command.BucketId);
        if (bucket == null)
            return OperationResult<SpendingDto>.MakeFailure("Bucket not found");

        // ... rest of handler logic
        
        return OperationResult<SpendingDto>.MakeSuccess(dto);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in CreateSpendingCommand handler");
        return OperationResult<SpendingDto>.MakeFailure($"An error occurred: {ex.Message}");
    }
}
```

### After (With Global Middleware)

```csharp
public async Task<OperationResult<SpendingDto>> Handle(CreateSpendingCommand command)
{
    // No try-catch needed! The middleware handles it automatically
    var bucket = await _bucketRepository.GetByIdAsync(command.BucketId);
    if (bucket == null)
        return OperationResult<SpendingDto>.MakeFailure("Bucket not found");

    // ... rest of handler logic
    
    return OperationResult<SpendingDto>.MakeSuccess(dto);
    
    // If an exception occurs anywhere in this method, it will be:
    // 1. Logged with full context
    // 2. Converted to OperationResult<SpendingDto>.MakeFailure("An unexpected error occurred...")
    // 3. Returned to the caller
}
```

## Configuration

The middleware is automatically configured in `ServiceCollectionExtensions.cs`:

```csharp
services.AddWolverine(opts =>
{
    // Add global exception middleware to all message handlers
    opts.Policies.AddMiddleware(typeof(GlobalExceptionMiddleware));
});
```

## Benefits

1. **Centralized Error Handling**: All exceptions are handled in one place
2. **Consistent Error Responses**: All handlers return consistent error formats
3. **Automatic Logging**: All exceptions are automatically logged with context
4. **Cleaner Code**: Eliminates repetitive try-catch blocks in handlers
5. **Type Safety**: Automatically converts exceptions to appropriate return types
6. **Zero Configuration**: Works automatically with existing handlers

## Error Handling Strategy

### For Commands (OperationResult/OperationResult<T>)
- Exceptions are caught and converted to `OperationResult.MakeFailure()`
- Error message includes the command type and exception message
- Full exception details are logged

### For Queries (DTOs/Collections)
- Single DTOs: Returns `null` on error
- Collections: Returns empty collection on error
- Exceptions are logged but don't break the query flow

### Logging
All exceptions are logged with:
- Full exception details (message, stack trace, inner exceptions)
- Message type context (which command/query failed)
- Log level: Error

## Testing

The middleware preserves the existing behavior for successful operations while adding robust error handling for failures. Your existing unit tests should continue to work without modification.

For testing error scenarios:
```csharp
[Fact]
public async Task Handle_DatabaseException_ShouldReturnFailureResult()
{
    // Arrange - setup mocks to throw exception
    _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                  .ThrowsAsync(new InvalidOperationException("Database error"));

    // Act
    var result = await _handler.Handle(command);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Contains("An unexpected error occurred", result.Errors.First().Message);
}
```

## Notes

- The middleware is applied globally to all handlers
- It does not interfere with business logic error handling (like validation failures)
- It only catches unhandled exceptions that would otherwise crash the handler
- Logging is performed at the Error level with full exception details

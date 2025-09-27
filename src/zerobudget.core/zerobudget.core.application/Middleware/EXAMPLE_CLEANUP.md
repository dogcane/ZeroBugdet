# Example: Cleaning Up BucketCommandHandlers

Here's how you can simplify your existing `BucketCommandHandlers` now that you have the global exception middleware.

## Before (With Manual Exception Handling)

```csharp
public async Task<OperationResult<BucketDto>> Handle(CreateBucketCommand command)
{
    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    try
    {
        var bucketResult = Bucket.Create(command.Name, command.Description, command.DefaultLimit);
        if (!bucketResult.Success)
        {
            return OperationResult<BucketDto>.MakeFailure(bucketResult.Errors);
        }

        var bucket = bucketResult.Value!;
        await _bucketRepository.AddAsync(bucket);

        scope.Complete();
        return OperationResult<BucketDto>.MakeSuccess(new BucketDto(
            bucket.Identity,
            bucket.Name,
            bucket.Description,
            bucket.DefaultLimit,
            bucket.DefaultBalance,
            bucket.Enabled
        ));
    }
    catch (Exception ex)
    {
        _logger?.LogError(ex, "Error in CreateBucketCommand handler");
        return OperationResult<BucketDto>.MakeFailure(ErrorMessage.Create("Exception", ex.Message));
    }
}
```

## After (With Global Exception Middleware)

```csharp
public async Task<OperationResult<BucketDto>> Handle(CreateBucketCommand command)
{
    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    
    var bucketResult = Bucket.Create(command.Name, command.Description, command.DefaultLimit);
    if (!bucketResult.Success)
    {
        return OperationResult<BucketDto>.MakeFailure(bucketResult.Errors);
    }

    var bucket = bucketResult.Value!;
    await _bucketRepository.AddAsync(bucket);

    scope.Complete();
    return OperationResult<BucketDto>.MakeSuccess(new BucketDto(
        bucket.Identity,
        bucket.Name,
        bucket.Description,
        bucket.DefaultLimit,
        bucket.DefaultBalance,
        bucket.Enabled
    ));
    
    // No more try-catch needed! 
    // If any exception occurs, the middleware will:
    // 1. Log it automatically with full context
    // 2. Convert it to OperationResult<BucketDto>.MakeFailure()
    // 3. Include a user-friendly error message
}
```

## Benefits

1. **50% less code** - No more try-catch blocks
2. **Consistent error handling** - All exceptions handled the same way
3. **Better logging** - Includes message type context automatically
4. **Cleaner business logic** - Focus on the happy path
5. **No duplication** - Exception handling logic in one place

## Optional: Remove Manual Exception Handling

If you want to clean up your existing handlers, you can safely remove the try-catch blocks from:
- `BucketCommandHandlers`
- `BucketQueryHandlers` 
- Any other handlers that have manual exception handling

The global middleware will handle all exceptions automatically while preserving the same behavior.

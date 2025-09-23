# ZeroBudget.Core.Application

This project contains the application layer for the ZeroBudget system, implementing CQRS patterns using Wolverine for handling commands and queries.

## Overview

The application layer provides a clean interface for all business operations through command and query handlers. It uses repository abstractions to keep the application layer decoupled from data access concerns.

## Architecture

The project follows the CQRS (Command Query Responsibility Segregation) pattern:

### Commands
Commands represent write operations (Create, Update, Delete) and are handled by dedicated command handlers:
- `BucketCommandHandlers` - Handle bucket operations
- `MonthlyBucketCommandHandlers` - Handle monthly bucket operations  
- `TagCommandHandlers` - Handle tag operations
- `SpendingCommandHandlers` - Handle spending operations
- `MonthlySpendingCommandHandlers` - Handle monthly spending operations

### Queries
Queries represent read operations and are handled by dedicated query handlers:
- `BucketQueryHandlers` - Handle bucket queries
- `MonthlyBucketQueryHandlers` - Handle monthly bucket queries
- `TagQueryHandlers` - Handle tag queries
- `SpendingQueryHandlers` - Handle spending queries
- `MonthlySpendingQueryHandlers` - Handle monthly spending queries

### DTOs
Data Transfer Objects provide a clean interface between the application layer and external consumers:
- `BucketDto`, `CreateBucketDto`, `UpdateBucketDto`
- `MonthlyBucketDto`, `CreateMonthlyBucketDto`, `UpdateMonthlyBucketDto`
- `TagDto`, `CreateTagDto`, `UpdateTagDto`
- `SpendingDto`, `CreateSpendingDto`, `UpdateSpendingDto`
- `MonthlySpendingDto`, `CreateMonthlySpendingDto`, `UpdateMonthlySpendingDto`

## Dependencies

- **Wolverine** - Message handling framework for commands and queries
- **Microsoft.Extensions.DependencyInjection.Abstractions** - Dependency injection support
- **zerobudget.core.domain** - Domain models and repository interfaces

## Usage

Register the application services in your dependency injection container:

```csharp
services.AddZeroBudgetApplication();
```

This will register all command and query handlers along with Wolverine configuration.

## Entity Operations

The application layer provides full CRUD operations for the following entities:

### Bucket
- Create new buckets with name, description, and default limit
- Update bucket properties
- Delete buckets
- Query buckets by ID, all buckets, or filter by name

### MonthlyBucket
- Create monthly buckets from parent buckets
- Update monthly bucket properties
- Delete monthly buckets
- Query by ID, all monthly buckets, by year/month, or by bucket ID

### Spending
- Create new spending entries with date, description, amount, owner, and tags
- Update spending properties
- Delete spending entries
- Query by ID, all spendings, by bucket, by date range, or by owner

### MonthlySpending
- Create monthly spending entries
- Update monthly spending properties
- Delete monthly spending entries
- Query by ID, all monthly spendings, by monthly bucket, by date range, or by owner

### Tag
- Create new tags
- Update tag names
- Delete tags
- Query by ID, all tags, or filter by name

## Implementation Notes

- All operations use domain models for business logic validation
- Repository abstractions are injected for data access
- Error handling uses the `OperationResult` pattern from the Resulz library
- Wolverine handles the command/query dispatching automatically
- The application layer is completely stateless and focused on orchestration
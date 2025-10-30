# Generic Collection Query Handlers

This document describes how to use the generic collection query handlers to filter and retrieve entities.

## Overview

The generic query handlers provide a flexible way to query collections of entities with filtering capabilities. Each entity type has its own specific ordering:
- **Buckets and MonthlyBuckets**: Ordered by Name/Description ascending
- **Spending**: Ordered by Description ascending  
- **MonthlySpending**: Ordered by Date descending

## Query Types

### BucketCollectionQuery
Query and filter Bucket entities.

**Example:**
```csharp
// Get all enabled buckets with "Savings" in the name
var query = new BucketCollectionQuery 
{ 
    Filters = new Dictionary<string, object?> 
    { 
        { "Name", "Savings" },
        { "Enabled", true }
    }
};

var handler = new BucketCollectionQueryHandler(bucketRepository);
var results = await handler.Handle(query);
```

**Available Filters:**
- `Name` (string) - Case-insensitive contains search
- `Description` (string) - Case-insensitive contains search
- `Enabled` (bool) - Exact match
- `DefaultLimit` (decimal) - Exact match
- `DefaultBalance` (decimal) - Exact match

**Ordering:** Results are ordered by `Name` ascending

### MonthlyBucketCollectionQuery
Query and filter MonthlyBucket entities.

**Example:**
```csharp
// Get all monthly buckets for January 2024
var query = new MonthlyBucketCollectionQuery 
{ 
    Filters = new Dictionary<string, object?> 
    { 
        { "Year", (short)2024 },
        { "Month", (short)1 }
    }
};

var handler = new MonthlyBucketCollectionQueryHandler(monthlyBucketRepository);
var results = await handler.Handle(query);
```

**Available Filters:**
- `Year` (short) - Exact match
- `Month` (short) - Exact match
- `BucketId` (int) - Exact match
- `Description` (string) - Case-insensitive contains search
- `Balance` (decimal) - Exact match
- `Limit` (decimal) - Exact match

**Ordering:** Results are ordered by `Description` ascending

### SpendingCollectionQuery
Query and filter Spending entities.

**Example:**
```csharp
// Get all spendings for a specific owner
var query = new SpendingCollectionQuery 
{ 
    Filters = new Dictionary<string, object?> 
    { 
        { "Owner", "john@example.com" }
    }
};

var handler = new SpendingCollectionQueryHandler(spendingRepository);
var results = await handler.Handle(query);
```

**Available Filters:**
- `BucketId` (int) - Exact match
- `Description` (string) - Case-insensitive contains search
- `Owner` (string) - Case-insensitive contains search
- `Amount` (decimal) - Exact match
- `Enabled` (bool) - Exact match

**Ordering:** Results are ordered by `Description` ascending

### MonthlySpendingCollectionQuery
Query and filter MonthlySpending entities.

**Example:**
```csharp
// Get monthly spendings in a date range
var query = new MonthlySpendingCollectionQuery 
{ 
    Filters = new Dictionary<string, object?> 
    { 
        { "StartDate", new DateOnly(2024, 1, 1) },
        { "EndDate", new DateOnly(2024, 1, 31) }
    }
};

var handler = new MonthlySpendingCollectionQueryHandler(monthlySpendingRepository);
var results = await handler.Handle(query);
```

**Available Filters:**
- `MonthlyBucketId` (int) - Exact match
- `Description` (string) - Case-insensitive contains search
- `Owner` (string) - Case-insensitive contains search
- `Amount` (decimal) - Exact match
- `StartDate` (DateOnly) - Greater than or equal to Date
- `EndDate` (DateOnly) - Less than or equal to Date

**Ordering:** Results are ordered by `Date` descending (most recent first)

## Filter Types

### String Filters
String filters perform case-insensitive contains search:
```csharp
{ "Name", "savings" }  // Matches "Savings Account", "My Savings", etc.
```

### Boolean Filters
Boolean filters perform exact match:
```csharp
{ "Enabled", true }  // Only matches entities where Enabled == true
```

### Numeric Filters
Numeric filters (int, short, decimal) perform exact match:
```csharp
{ "Year", (short)2024 }  // Only matches entities where Year == 2024
```

### Date Range Filters
Date range filters use special naming convention:
```csharp
// Get entities where Date >= StartDate AND Date <= EndDate
{ 
    "StartDate", new DateOnly(2024, 1, 1),
    "EndDate", new DateOnly(2024, 1, 31)
}
```

## Multiple Filters

You can combine multiple filters - all filters are applied with AND logic:
```csharp
var query = new BucketCollectionQuery 
{ 
    Filters = new Dictionary<string, object?> 
    { 
        { "Name", "Savings" },      // AND
        { "Enabled", true },         // AND
        { "Description", "Monthly" } // Matches buckets with "Savings" in name, 
                                      // enabled, and "Monthly" in description
    }
};
```

## No Filters

To get all entities (with proper ordering):
```csharp
var query = new BucketCollectionQuery 
{ 
    Filters = new Dictionary<string, object?>() 
};
// Returns all buckets ordered by Name ascending
```

## Notes

- All filters exclude the `Identity` (Id) property as specified in requirements
- String comparisons are case-insensitive
- Numeric and boolean comparisons are exact matches
- Date range filtering supports StartDate/EndDate pattern
- Results always include the entity-specific ordering

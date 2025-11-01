# Generic Collection Query Handlers

This document describes how to use the generic collection query handlers to filter and retrieve entities.

## Overview

The generic query handlers provide a flexible way to query collections of entities with strongly-typed filtering capabilities. Each entity type has its own specific ordering:
- **Buckets and MonthlyBuckets**: Ordered by Name/Description ascending
- **Spending**: Ordered by Description ascending  
- **MonthlySpending**: Ordered by Date descending

## Query Types

### BucketCollectionQuery
Query and filter Bucket entities using strongly-typed optional parameters.

**Example:**
```csharp
// Get all enabled buckets with "Savings" in the name
var query = new BucketCollectionQuery(
    Name: "Savings",
    Enabled: true
);

var handler = new BucketCollectionQueryHandler(bucketRepository);
var results = await handler.Handle(query);
```

**Available Parameters:**
- `Name` (string?, optional) - Case-insensitive contains search
- `Description` (string?, optional) - Case-insensitive contains search
- `Enabled` (bool?, optional) - Exact match

**Ordering:** Results are ordered by `Name` ascending

### MonthlyBucketCollectionQuery
Query and filter MonthlyBucket entities using strongly-typed optional parameters.

**Example:**
```csharp
// Get all monthly buckets for January 2024
var query = new MonthlyBucketCollectionQuery(
    Year: (short)2024,
    Month: (short)1
);

var handler = new MonthlyBucketCollectionQueryHandler(monthlyBucketRepository);
var results = await handler.Handle(query);
```

**Available Parameters:**
- `Year` (short?, optional) - Exact match
- `Month` (short?, optional) - Exact match
- `BucketId` (int?, optional) - Exact match
- `Description` (string?, optional) - Case-insensitive contains search

**Ordering:** Results are ordered by `Description` ascending

### SpendingCollectionQuery
Query and filter Spending entities using strongly-typed optional parameters.

**Example:**
```csharp
// Get all spendings for a specific owner
var query = new SpendingCollectionQuery(
    Owner: "john@example.com"
);

var handler = new SpendingCollectionQueryHandler(spendingRepository);
var results = await handler.Handle(query);
```

**Available Parameters:**
- `BucketId` (int?, optional) - Exact match
- `Description` (string?, optional) - Case-insensitive contains search
- `Owner` (string?, optional) - Case-insensitive contains search
- `Enabled` (bool?, optional) - Exact match

**Ordering:** Results are ordered by `Description` ascending

### MonthlySpendingCollectionQuery
Query and filter MonthlySpending entities using strongly-typed optional parameters.

**Example:**
```csharp
// Get monthly spendings in a date range
var query = new MonthlySpendingCollectionQuery(
    StartDate: new DateOnly(2024, 1, 1),
    EndDate: new DateOnly(2024, 1, 31)
);

var handler = new MonthlySpendingCollectionQueryHandler(monthlySpendingRepository);
var results = await handler.Handle(query);
```

**Available Parameters:**
- `MonthlyBucketId` (int?, optional) - Exact match
- `Description` (string?, optional) - Case-insensitive contains search
- `Owner` (string?, optional) - Case-insensitive contains search
- `StartDate` (DateOnly?, optional) - Greater than or equal to Date
- `EndDate` (DateOnly?, optional) - Less than or equal to Date

**Ordering:** Results are ordered by `Date` descending (most recent first)

## Filter Types

### String Filters
String filters perform case-insensitive contains search:
```csharp
new BucketCollectionQuery(Name: "savings")  // Matches "Savings Account", "My Savings", etc.
```

### Boolean Filters
Boolean filters perform exact match:
```csharp
new BucketCollectionQuery(Enabled: true)  // Only matches entities where Enabled == true
```

### Numeric Filters
Numeric filters (int, short) perform exact match:
```csharp
new MonthlyBucketCollectionQuery(Year: (short)2024)  // Only matches entities where Year == 2024
```

### Date Range Filters
Date range filters use StartDate and EndDate parameters:
```csharp
// Get entities where Date >= StartDate AND Date <= EndDate
new MonthlySpendingCollectionQuery(
    StartDate: new DateOnly(2024, 1, 1),
    EndDate: new DateOnly(2024, 1, 31)
)
```

## Multiple Filters

You can combine multiple filters using named parameters - all filters are applied with AND logic:
```csharp
var query = new BucketCollectionQuery(
    Name: "Savings",       // AND
    Enabled: true,         // AND
    Description: "Monthly" // Matches buckets with "Savings" in name, 
                           // enabled, and "Monthly" in description
);
```

## No Filters

To get all entities (with proper ordering), create a query without parameters:
```csharp
var query = new BucketCollectionQuery();
// Returns all buckets ordered by Name ascending
```

## Notes

- All query parameters are optional (nullable)
- Parameters exclude the `Identity` (Id) property as specified in requirements
- String comparisons are case-insensitive
- Numeric and boolean comparisons are exact matches
- Date range filtering supports StartDate/EndDate parameters
- Results always include the entity-specific ordering
- Strongly-typed parameters provide compile-time safety and IntelliSense support

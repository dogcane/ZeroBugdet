using ECO.Integrations.Moq;
using Moq;
using Xunit;
using zerobudget.core.application.Handlers.Queries;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;

namespace zerobudget.core.application.tests;

public class GenericCollectionQueryHandlerTests
{
    #region BucketCollectionQueryHandler Tests

    [Fact]
    public async Task BucketCollectionQueryHandler_NoFilters_ReturnsAllBucketsOrderedByName()
    {
        // Arrange
        var bucketRepository = new Mock<IBucketRepository>();
        var handler = new BucketCollectionQueryHandler(bucketRepository.Object);

        var bucket1 = Bucket.Create("Zebra Bucket", "Description Z", 1000m).Value!;
        var bucket2 = Bucket.Create("Alpha Bucket", "Description A", 2000m).Value!;
        var bucket3 = Bucket.Create("Beta Bucket", "Description B", 3000m).Value!;
        var buckets = new[] { bucket1, bucket2, bucket3 };

        var query = new BucketCollectionQuery();

        bucketRepository.SetupAsQueryable<IBucketRepository, Bucket, int>(buckets);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        var resultList = result.ToList();
        Assert.Equal("Alpha Bucket", resultList[0].Name);
        Assert.Equal("Beta Bucket", resultList[1].Name);
        Assert.Equal("Zebra Bucket", resultList[2].Name);
    }

    [Fact]
    public async Task BucketCollectionQueryHandler_FilterByName_ReturnsMatchingBucketsOrderedByName()
    {
        // Arrange
        var bucketRepository = new Mock<IBucketRepository>();
        var handler = new BucketCollectionQueryHandler(bucketRepository.Object);

        var bucket1 = Bucket.Create("Test Bucket 1", "Description 1", 1000m).Value!;
        var bucket2 = Bucket.Create("Test Bucket 2", "Description 2", 2000m).Value!;
        var bucket3 = Bucket.Create("Other Bucket", "Description 3", 3000m).Value!;
        var buckets = new[] { bucket1, bucket2, bucket3 };

        var query = new BucketCollectionQuery(Name: "Test");

        bucketRepository.SetupAsQueryable<IBucketRepository, Bucket, int>(buckets);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Contains("Test", b.Name));
    }

    [Fact]
    public async Task BucketCollectionQueryHandler_FilterByEnabled_ReturnsOnlyEnabledBuckets()
    {
        // Arrange
        var bucketRepository = new Mock<IBucketRepository>();
        var handler = new BucketCollectionQueryHandler(bucketRepository.Object);

        var bucket1 = Bucket.Create("Bucket 1", "Description 1", 1000m).Value!;
        var bucket2 = Bucket.Create("Bucket 2", "Description 2", 2000m).Value!;
        bucket2.Disable();
        var buckets = new[] { bucket1, bucket2 };

        var query = new BucketCollectionQuery(Enabled: true);

        bucketRepository.SetupAsQueryable<IBucketRepository, Bucket, int>(buckets);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Bucket 1", result.First().Name);
    }

    #endregion

    #region MonthlyBucketCollectionQueryHandler Tests

    [Fact]
    public async Task MonthlyBucketCollectionQueryHandler_NoFilters_ReturnsAllOrderedByDescription()
    {
        // Arrange
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var handler = new MonthlyBucketCollectionQueryHandler(monthlyBucketRepository.Object);

        var bucket1 = Bucket.Create("Bucket 1", "Zebra Description", 1000m).Value!;
        var bucket2 = Bucket.Create("Bucket 2", "Alpha Description", 2000m).Value!;
        
        var mb1 = bucket1.CreateMonthly(2024, 1).Value!;
        var mb2 = bucket2.CreateMonthly(2024, 1).Value!;
        var monthlyBuckets = new[] { mb1, mb2 };

        var query = new MonthlyBucketCollectionQuery();

        monthlyBucketRepository.SetupAsQueryable<IMonthlyBucketRepository, MonthlyBucket, int>(monthlyBuckets);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        var resultList = result.ToList();
        Assert.Equal("Alpha Description", resultList[0].Description);
        Assert.Equal("Zebra Description", resultList[1].Description);
    }

    [Fact]
    public async Task MonthlyBucketCollectionQueryHandler_FilterByYear_ReturnsMatchingMonthlyBuckets()
    {
        // Arrange
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var handler = new MonthlyBucketCollectionQueryHandler(monthlyBucketRepository.Object);

        var bucket = Bucket.Create("Test Bucket", "Description", 1000m).Value!;
        var mb1 = bucket.CreateMonthly(2024, 1).Value!;
        var mb2 = bucket.CreateMonthly(2023, 1).Value!;
        var monthlyBuckets = new[] { mb1, mb2 };

        var query = new MonthlyBucketCollectionQuery(Year: (short)2024);

        monthlyBucketRepository.SetupAsQueryable<IMonthlyBucketRepository, MonthlyBucket, int>(monthlyBuckets);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(2024, result.First().Year);
    }

    [Fact]
    public async Task MonthlyBucketCollectionQueryHandler_FilterByYearAndMonth_ReturnsMatchingMonthlyBuckets()
    {
        // Arrange
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var handler = new MonthlyBucketCollectionQueryHandler(monthlyBucketRepository.Object);

        var bucket = Bucket.Create("Test Bucket", "Description", 1000m).Value!;
        var mb1 = bucket.CreateMonthly(2024, 1).Value!;
        var mb2 = bucket.CreateMonthly(2024, 2).Value!;
        var mb3 = bucket.CreateMonthly(2023, 1).Value!;
        var monthlyBuckets = new[] { mb1, mb2, mb3 };

        var query = new MonthlyBucketCollectionQuery(Year: (short)2024, Month: (short)1);

        monthlyBucketRepository.SetupAsQueryable<IMonthlyBucketRepository, MonthlyBucket, int>(monthlyBuckets);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var monthlyBucket = result.First();
        Assert.Equal(2024, monthlyBucket.Year);
        Assert.Equal(1, monthlyBucket.Month);
    }

    #endregion

    #region SpendingCollectionQueryHandler Tests

    [Fact]
    public async Task SpendingCollectionQueryHandler_NoFilters_ReturnsAllOrderedByDescription()
    {
        // Arrange
        var spendingRepository = new Mock<ISpendingRepository>();
        var handler = new SpendingCollectionQueryHandler(spendingRepository.Object);

        var bucket = Bucket.Create("Test Bucket", "Description", 1000m).Value!;
        var spending1 = Spending.Create("Zebra Expense", 100m, "Owner1", [], bucket).Value!;
        var spending2 = Spending.Create("Alpha Expense", 200m, "Owner2", [], bucket).Value!;
        var spending3 = Spending.Create("Beta Expense", 300m, "Owner3", [], bucket).Value!;
        var spendings = new[] { spending1, spending2, spending3 };

        var query = new SpendingCollectionQuery();

        spendingRepository.SetupAsQueryable<ISpendingRepository, Spending, int>(spendings);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        var resultList = result.ToList();
        Assert.Equal("Alpha Expense", resultList[0].Description);
        Assert.Equal("Beta Expense", resultList[1].Description);
        Assert.Equal("Zebra Expense", resultList[2].Description);
    }

    [Fact]
    public async Task SpendingCollectionQueryHandler_FilterByDescription_ReturnsMatchingSpendings()
    {
        // Arrange
        var spendingRepository = new Mock<ISpendingRepository>();
        var handler = new SpendingCollectionQueryHandler(spendingRepository.Object);

        var bucket = Bucket.Create("Test Bucket", "Description", 1000m).Value!;
        var spending1 = Spending.Create("Grocery Shopping", 100m, "Owner1", [], bucket).Value!;
        var spending2 = Spending.Create("Gas Station", 50m, "Owner2", [], bucket).Value!;
        var spending3 = Spending.Create("Grocery Store", 75m, "Owner3", [], bucket).Value!;
        var spendings = new[] { spending1, spending2, spending3 };

        var query = new SpendingCollectionQuery(Description: "Grocery");

        spendingRepository.SetupAsQueryable<ISpendingRepository, Spending, int>(spendings);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, s => Assert.Contains("Grocery", s.Description));
    }

    [Fact]
    public async Task SpendingCollectionQueryHandler_FilterByOwner_ReturnsMatchingSpendings()
    {
        // Arrange
        var spendingRepository = new Mock<ISpendingRepository>();
        var handler = new SpendingCollectionQueryHandler(spendingRepository.Object);

        var bucket = Bucket.Create("Test Bucket", "Description", 1000m).Value!;
        var spending1 = Spending.Create("Expense 1", 100m, "john@example.com", [], bucket).Value!;
        var spending2 = Spending.Create("Expense 2", 50m, "jane@example.com", [], bucket).Value!;
        var spending3 = Spending.Create("Expense 3", 75m, "john@example.com", [], bucket).Value!;
        var spendings = new[] { spending1, spending2, spending3 };

        var query = new SpendingCollectionQuery(Owner: "john");

        spendingRepository.SetupAsQueryable<ISpendingRepository, Spending, int>(spendings);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, s => Assert.Contains("john", s.Owner, StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region MonthlySpendingCollectionQueryHandler Tests

    [Fact]
    public async Task MonthlySpendingCollectionQueryHandler_NoFilters_ReturnsAllOrderedByDateDescending()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new MonthlySpendingCollectionQueryHandler(monthlySpendingRepository.Object);

        var bucket = Bucket.Create("Test Bucket", "Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2024, 1).Value!;
        
        var spending1 = MonthlySpending.Create(new DateOnly(2024, 1, 1), "Expense 1", 100m, "Owner", [], monthlyBucket).Value!;
        var spending2 = MonthlySpending.Create(new DateOnly(2024, 1, 15), "Expense 2", 200m, "Owner", [], monthlyBucket).Value!;
        var spending3 = MonthlySpending.Create(new DateOnly(2024, 1, 10), "Expense 3", 300m, "Owner", [], monthlyBucket).Value!;
        var monthlySpendings = new[] { spending1, spending2, spending3 };

        var query = new MonthlySpendingCollectionQuery();

        monthlySpendingRepository.SetupAsQueryable<IMonthlySpendingRepository, MonthlySpending, int>(monthlySpendings);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        var resultList = result.ToList();
        Assert.Equal(new DateOnly(2024, 1, 15), resultList[0].Date);
        Assert.Equal(new DateOnly(2024, 1, 10), resultList[1].Date);
        Assert.Equal(new DateOnly(2024, 1, 1), resultList[2].Date);
    }

    [Fact]
    public async Task MonthlySpendingCollectionQueryHandler_FilterByDateRange_ReturnsMatchingSpendings()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new MonthlySpendingCollectionQueryHandler(monthlySpendingRepository.Object);

        var bucket = Bucket.Create("Test Bucket", "Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2024, 1).Value!;
        
        var spending1 = MonthlySpending.Create(new DateOnly(2024, 1, 5), "Expense 1", 100m, "Owner", [], monthlyBucket).Value!;
        var spending2 = MonthlySpending.Create(new DateOnly(2024, 1, 15), "Expense 2", 200m, "Owner", [], monthlyBucket).Value!;
        var spending3 = MonthlySpending.Create(new DateOnly(2024, 1, 25), "Expense 3", 300m, "Owner", [], monthlyBucket).Value!;
        var monthlySpendings = new[] { spending1, spending2, spending3 };

        var query = new MonthlySpendingCollectionQuery(
            StartDate: new DateOnly(2024, 1, 10),
            EndDate: new DateOnly(2024, 1, 20)
        );

        monthlySpendingRepository.SetupAsQueryable<IMonthlySpendingRepository, MonthlySpending, int>(monthlySpendings);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(new DateOnly(2024, 1, 15), result.First().Date);
    }

    [Fact]
    public async Task MonthlySpendingCollectionQueryHandler_FilterByOwner_ReturnsMatchingSpendings()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new MonthlySpendingCollectionQueryHandler(monthlySpendingRepository.Object);

        var bucket = Bucket.Create("Test Bucket", "Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2024, 1).Value!;
        
        var spending1 = MonthlySpending.Create(new DateOnly(2024, 1, 1), "Expense 1", 100m, "john@example.com", [], monthlyBucket).Value!;
        var spending2 = MonthlySpending.Create(new DateOnly(2024, 1, 2), "Expense 2", 200m, "jane@example.com", [], monthlyBucket).Value!;
        var spending3 = MonthlySpending.Create(new DateOnly(2024, 1, 3), "Expense 3", 300m, "john@example.com", [], monthlyBucket).Value!;
        var monthlySpendings = new[] { spending1, spending2, spending3 };

        var query = new MonthlySpendingCollectionQuery(Owner: "john");

        monthlySpendingRepository.SetupAsQueryable<IMonthlySpendingRepository, MonthlySpending, int>(monthlySpendings);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, s => Assert.Contains("john", s.Owner, StringComparison.OrdinalIgnoreCase));
    }

    #endregion
}

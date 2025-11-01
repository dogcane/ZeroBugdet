using ECO.Data;
using ECO.Integrations.Moq;
using Moq;
using Xunit;
using zerobudget.core.application.Handlers.Queries;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;

namespace zerobudget.core.application.tests;

public class MonthlySpendingQueryHandlerTests
{
    [Fact]
    public async Task Handle_GetMonthlySpendingByIdQuery_ReturnsMonthlySpending()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new GetMonthlySpendingByIdQueryHandler(monthlySpendingRepository.Object);

        var bucket = Bucket.Create("Test", "Description", 1000m).Value!.WithIdentity<Bucket, int>(1);
        var monthlyBucket = bucket.CreateMonthly(2024, 10).Value!.WithIdentity<MonthlyBucket, int>(1);
        var spending = Spending.Create("Test Spending", 100m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        var monthlySpending = spending.CreateMonthly(monthlyBucket).Value!.WithIdentity<MonthlySpending, int>(1);

        monthlySpendingRepository
            .SetupRepository<IMonthlySpendingRepository, MonthlySpending, int>([monthlySpending]);

        var query = new GetMonthlySpendingByIdQuery(1);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Spending", result.Description);
        Assert.Equal(100m, result.Amount);
    }

    [Fact]
    public async Task Handle_GetMonthlySpendingByIdQuery_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new GetMonthlySpendingByIdQueryHandler(monthlySpendingRepository.Object);

        monthlySpendingRepository
            .SetupRepository<IMonthlySpendingRepository, MonthlySpending, int>([]);

        var query = new GetMonthlySpendingByIdQuery(999);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_GetMonthlySpendingsQuery_NoFilters_ReturnsAllMonthlySpendings()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new MonthlySpendingCollectionQueryHandler(monthlySpendingRepository.Object);

        var bucket = Bucket.Create("Test", "Description", 1000m).Value!.WithIdentity<Bucket, int>(1);
        var monthlyBucket = bucket.CreateMonthly(2024, 10).Value!.WithIdentity<MonthlyBucket, int>(1);
        var spending1 = Spending.Create("Spending1", 100m, "Owner1", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("Spending2", 200m, "Owner2", Array.Empty<Tag>(), bucket).Value!;
        var monthlySpending1 = spending1.CreateMonthly(monthlyBucket).Value!;
        var monthlySpending2 = spending2.CreateMonthly(monthlyBucket).Value!;

        monthlySpendingRepository.SetupAsQueryable<IMonthlySpendingRepository, MonthlySpending, int>(new[] { monthlySpending1, monthlySpending2 });

        var query = new GetMonthlySpendingsQuery();

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Handle_GetMonthlySpendingsQuery_WithFilters_ReturnsFilteredMonthlySpendings()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new MonthlySpendingCollectionQueryHandler(monthlySpendingRepository.Object);

        var bucket = Bucket.Create("Test", "Description", 1000m).Value!.WithIdentity<Bucket, int>(1);
        var monthlyBucket1 = bucket.CreateMonthly(2024, 10).Value!.WithIdentity<MonthlyBucket, int>(1);
        var monthlyBucket2 = bucket.CreateMonthly(2024, 11).Value!.WithIdentity<MonthlyBucket, int>(2);
        var spending1 = Spending.Create("Spending1", 100m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("Spending2", 200m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        var monthlySpending1 = spending1.CreateMonthly(monthlyBucket1).Value!;
        var monthlySpending2 = spending2.CreateMonthly(monthlyBucket2).Value!;

        monthlySpendingRepository.SetupAsQueryable<IMonthlySpendingRepository, MonthlySpending, int>(new[] { monthlySpending1, monthlySpending2 });

        var query = new GetMonthlySpendingsQuery(MonthlyBucketId: monthlyBucket1.Identity);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(monthlyBucket1.Identity, result.First().MonthlyBucketId);
    }

    [Fact]
    public async Task Handle_GetMonthlySpendingsQuery_WithDateRangeFilter_ReturnsFilteredMonthlySpendings()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new MonthlySpendingCollectionQueryHandler(monthlySpendingRepository.Object);

        var bucket = Bucket.Create("Test", "Description", 1000m).Value!.WithIdentity<Bucket, int>(1);
        var monthlyBucket1 = bucket.CreateMonthly(2024, 10).Value!.WithIdentity<MonthlyBucket, int>(1);
        var monthlyBucket2 = bucket.CreateMonthly(2024, 11).Value!.WithIdentity<MonthlyBucket, int>(2);
        var spending1 = Spending.Create("Spending1", 100m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("Spending2", 200m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        var monthlySpending1 = spending1.CreateMonthly(monthlyBucket1).Value!;
        var monthlySpending2 = spending2.CreateMonthly(monthlyBucket2).Value!;

        monthlySpendingRepository.SetupAsQueryable<IMonthlySpendingRepository, MonthlySpending, int>(new[] { monthlySpending1, monthlySpending2 });

        var query = new GetMonthlySpendingsQuery(
            StartDate: new DateOnly(2024, 10, 1),
            EndDate: new DateOnly(2024, 10, 31));

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(new DateOnly(2024, 10, 1), result.First().Date);
    }

    [Fact]
    public async Task Handle_GetMonthlySpendingsQuery_WithOwnerFilter_ReturnsFilteredMonthlySpendings()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new MonthlySpendingCollectionQueryHandler(monthlySpendingRepository.Object);

        var bucket = Bucket.Create("Test", "Description", 1000m).Value!.WithIdentity<Bucket, int>(1);
        var monthlyBucket = bucket.CreateMonthly(2024, 10).Value!.WithIdentity<MonthlyBucket, int>(1);
        var spending1 = Spending.Create("Spending1", 100m, "Owner1", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("Spending2", 200m, "Owner2", Array.Empty<Tag>(), bucket).Value!;
        var monthlySpending1 = spending1.CreateMonthly(monthlyBucket).Value!;
        var monthlySpending2 = spending2.CreateMonthly(monthlyBucket).Value!;

        monthlySpendingRepository.SetupAsQueryable<IMonthlySpendingRepository, MonthlySpending, int>(new[] { monthlySpending1, monthlySpending2 });

        var query = new GetMonthlySpendingsQuery(Owner: "Owner1");

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Owner1", result.First().Owner);
    }
}

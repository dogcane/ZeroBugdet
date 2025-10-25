using ECO.Data;
using ECO.Integrations.Moq;
using Moq;
using Xunit;
using zerobudget.core.application.Handlers.Queries;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;

namespace zerobudget.core.application.tests;

public class MonthlyBucketQueryHandlerTests
{
    [Fact]
    public async Task Handle_GetMonthlyBucketByIdQuery_ReturnsMonthlyBucket()
    {
        // Arrange
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var handler = new GetMonthlyBucketByIdQueryHandler(monthlyBucketRepository.Object);

        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2024, 10);

        monthlyBucketRepository
            .SetupRepository<IMonthlyBucketRepository, MonthlyBucket, int>([monthlyBucket]);

        var query = new GetMonthlyBucketByIdQuery(1);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2024, result.Year);
        Assert.Equal(10, result.Month);
    }

    [Fact]
    public async Task Handle_GetMonthlyBucketByIdQuery_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var handler = new GetMonthlyBucketByIdQueryHandler(monthlyBucketRepository.Object);

        monthlyBucketRepository
            .SetupRepository<IMonthlyBucketRepository, MonthlyBucket, int>([]);

        var query = new GetMonthlyBucketByIdQuery(999);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_GetAllMonthlyBucketsQuery_ReturnsAllMonthlyBuckets()
    {
        // Arrange
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var handler = new GetAllMonthlyBucketsQueryHandler(monthlyBucketRepository.Object);

        var bucket1 = Bucket.Create("Test1", "Description1", 1000m).Value!;
        var bucket2 = Bucket.Create("Test2", "Description2", 2000m).Value!;
        var monthlyBucket1 = bucket1.CreateMonthly(2024, 10);
        var monthlyBucket2 = bucket2.CreateMonthly(2024, 10);

        monthlyBucketRepository
            .Setup(r => r.AsQueryable())
            .Returns(new[] { monthlyBucket1, monthlyBucket2 }.AsQueryable());

        var query = new GetAllMonthlyBucketsQuery();

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Handle_GetMonthlyBucketsByYearMonthQuery_ReturnsFilteredMonthlyBuckets()
    {
        // Arrange
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var handler = new GetMonthlyBucketsByYearMonthQueryHandler(monthlyBucketRepository.Object);

        var bucket1 = Bucket.Create("Test1", "Description1", 1000m).Value!;
        var bucket2 = Bucket.Create("Test2", "Description2", 2000m).Value!;
        var monthlyBucket1 = bucket1.CreateMonthly(2024, 10);
        var monthlyBucket2 = bucket2.CreateMonthly(2024, 11);

        monthlyBucketRepository
            .Setup(r => r.AsQueryable())
            .Returns(new[] { monthlyBucket1, monthlyBucket2 }.AsQueryable());

        var query = new GetMonthlyBucketsByYearMonthQuery(2024, 10);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(10, result.First().Month);
    }

    [Fact]
    public async Task Handle_GetMonthlyBucketsByBucketIdQuery_ReturnsFilteredMonthlyBuckets()
    {
        // Arrange
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var handler = new GetMonthlyBucketsByBucketIdQueryHandler(monthlyBucketRepository.Object);

        var bucket1 = Bucket.Create("Test1", "Description1", 1000m).Value!;
        var bucket2 = Bucket.Create("Test2", "Description2", 2000m).Value!;
        var monthlyBucket1 = bucket1.CreateMonthly(2024, 10);
        var monthlyBucket2 = bucket2.CreateMonthly(2024, 10);

        monthlyBucketRepository
            .Setup(r => r.AsQueryable())
            .Returns(new[] { monthlyBucket1, monthlyBucket2 }.AsQueryable());

        var query = new GetMonthlyBucketsByBucketIdQuery(bucket1.Identity);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(bucket1.Identity, result.First().BucketId);
    }
}

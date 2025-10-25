using ECO.Data;
using ECO.Integrations.Moq;
using Moq;
using Xunit;
using zerobudget.core.application.Handlers.Queries;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;

namespace zerobudget.core.application.tests;

public class SpendingQueryHandlerTests
{
    [Fact]
    public async Task Handle_GetSpendingByIdQuery_ReturnsSpending()
    {
        // Arrange
        var spendingRepository = new Mock<ISpendingRepository>();
        var handler = new GetSpendingByIdQueryHandler(spendingRepository.Object);

        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var spending = Spending.Create("Test Spending", 100m, "Owner", Array.Empty<Tag>(), bucket).Value!;

        spendingRepository
            .SetupRepository<ISpendingRepository, Spending, int>([spending]);

        var query = new GetSpendingByIdQuery(1);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Spending", result.Description);
        Assert.Equal(100m, result.Amount);
    }

    [Fact]
    public async Task Handle_GetSpendingByIdQuery_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var spendingRepository = new Mock<ISpendingRepository>();
        var handler = new GetSpendingByIdQueryHandler(spendingRepository.Object);

        spendingRepository
            .SetupRepository<ISpendingRepository, Spending, int>([]);

        var query = new GetSpendingByIdQuery(999);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_GetAllSpendingsQuery_ReturnsAllSpendings()
    {
        // Arrange
        var spendingRepository = new Mock<ISpendingRepository>();
        var handler = new GetAllSpendingsQueryHandler(spendingRepository.Object);

        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var spending1 = Spending.Create("Spending1", 100m, "Owner1", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("Spending2", 200m, "Owner2", Array.Empty<Tag>(), bucket).Value!;

        spendingRepository
            .Setup(r => r.AsQueryable())
            .Returns(new[] { spending1, spending2 }.AsQueryable());

        var query = new GetAllSpendingsQuery();

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Handle_GetSpendingsByBucketIdQuery_ReturnsFilteredSpendings()
    {
        // Arrange
        var spendingRepository = new Mock<ISpendingRepository>();
        var handler = new GetSpendingsByBucketIdQueryHandler(spendingRepository.Object);

        var bucket1 = Bucket.Create("Test1", "Description1", 1000m).Value!;
        var bucket2 = Bucket.Create("Test2", "Description2", 2000m).Value!;
        var spending1 = Spending.Create("Spending1", 100m, "Owner", Array.Empty<Tag>(), bucket1).Value!;
        var spending2 = Spending.Create("Spending2", 200m, "Owner", Array.Empty<Tag>(), bucket2).Value!;

        spendingRepository
            .Setup(r => r.AsQueryable())
            .Returns(new[] { spending1, spending2 }.AsQueryable());

        var query = new GetSpendingsByBucketIdQuery(bucket1.Identity);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(bucket1.Identity, result.First().BucketId);
    }

    [Fact]
    public async Task Handle_GetSpendingsByOwnerQuery_ReturnsFilteredSpendings()
    {
        // Arrange
        var spendingRepository = new Mock<ISpendingRepository>();
        var handler = new GetSpendingsByOwnerQueryHandler(spendingRepository.Object);

        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var spending1 = Spending.Create("Spending1", 100m, "Owner1", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("Spending2", 200m, "Owner2", Array.Empty<Tag>(), bucket).Value!;

        spendingRepository
            .SetupRepository<ISpendingRepository, Spending, int>([spending1, spending2]);

        var query = new GetSpendingsByOwnerQuery("Owner1");

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Owner1", result.First().Owner);
    }
}

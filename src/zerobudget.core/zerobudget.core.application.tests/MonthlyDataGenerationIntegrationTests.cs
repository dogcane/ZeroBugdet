using Xunit;
using zerobudget.core.application.Commands;
using zerobudget.core.domain;
using Moq;
using zerobudget.core.application.Handlers.Commands;

namespace zerobudget.core.application.tests.Integration;

/// <summary>
/// Integration tests for MonthlyDataGeneration command handler
/// Uses mocking to test handler behavior
/// </summary>
public class MonthlyDataGenerationIntegrationTests
{
    [Fact]
    public async Task Handle_GenerateMonthlyDataCommand_WithValidData_ShouldSucceed()
    {
        // Arrange
        var mockMonthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var mockBucketRepository = new Mock<IBucketRepository>();
        var mockMonthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var mockSpendingRepository = new Mock<ISpendingRepository>();

        var handler = new MonthlyBucketCommandHandlers(
            mockMonthlyBucketRepository.Object,
            mockBucketRepository.Object,
            mockSpendingRepository.Object,
            mockMonthlySpendingRepository.Object);

        // Setup buckets
        var bucket1 = Bucket.Create("Test Bucket 1", "Description 1", 1000m).Value!;
        var bucket2 = Bucket.Create("Test Bucket 2", "Description 2", 2000m).Value!;

        mockBucketRepository.Setup(r => r.AsQueryable())
            .Returns(new[] { bucket1, bucket2 }.AsQueryable());

        // Setup spendings
        var spending1 = Spending.Create(
            "Test Spending 1",
            100m,
            "Owner 1",
            Array.Empty<Tag>(),
            bucket1).Value!;

        var spending2 = Spending.Create(
            "Test Spending 2",
            200m,
            "Owner 2",
            Array.Empty<Tag>(),
            bucket2).Value!;

        mockSpendingRepository.Setup(r => r.AsQueryable())
            .Returns(new[] { spending1, spending2 }.AsQueryable());

        mockMonthlyBucketRepository.Setup(r => r.AsQueryable())
            .Returns(Enumerable.Empty<MonthlyBucket>().AsQueryable());

        var command = new GenerateMonthlyDataCommand(2025, 1);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(2025, result.Value.Year);
        Assert.Equal(1, result.Value.Month);
        Assert.True(result.Value.MonthlyBucketsCreated > 0);
        Assert.True(result.Value.MonthlySpendingsCreated > 0);
    }

    [Fact]
    public async Task Handle_GenerateMonthlyDataCommand_WhenDataExists_ShouldFail()
    {
        // Arrange
        var mockMonthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var mockBucketRepository = new Mock<IBucketRepository>();
        var mockMonthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var mockSpendingRepository = new Mock<ISpendingRepository>();

        var handler = new MonthlyBucketCommandHandlers(
            mockMonthlyBucketRepository.Object,
            mockBucketRepository.Object,
            mockSpendingRepository.Object,
            mockMonthlySpendingRepository.Object);

        // Setup existing monthly data
        var bucket = Bucket.Create("Test Bucket", "Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 1);

        mockBucketRepository.Setup(r => r.AsQueryable())
            .Returns(new[] { bucket }.AsQueryable());

        mockMonthlyBucketRepository.Setup(r => r.AsQueryable())
            .Returns(new[] { monthlyBucket }.AsQueryable());

        mockSpendingRepository.Setup(r => r.AsQueryable())
            .Returns(Enumerable.Empty<Spending>().AsQueryable());

        var command = new GenerateMonthlyDataCommand(2025, 1);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("already exists", result.Errors.First().Description);
    }
}

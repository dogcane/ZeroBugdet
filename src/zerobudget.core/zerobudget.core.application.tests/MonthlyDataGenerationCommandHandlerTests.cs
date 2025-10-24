using ECO.Integrations.Moq;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using zerobudget.core.application.Commands;
using zerobudget.core.application.DTOs;
using zerobudget.core.application.Handlers.Commands;
using zerobudget.core.domain;

namespace zerobudget.core.application.tests;

/// <summary>
/// Tests for MonthlyDataGenerationCommand handler using LINQ IQueryable approach
/// Note: These tests are simplified examples. For production, consider using in-memory database
/// or more sophisticated mocking libraries that handle IQueryable operations better.
/// </summary>
public class MonthlyDataGenerationCommandHandlerTests
{
    private readonly Mock<IMonthlyBucketRepository> _mockMonthlyBucketRepository;
    private readonly Mock<IBucketRepository> _mockBucketRepository;
    private readonly Mock<ILogger<GenerateMonthlyDataCommandHandler>> _mockLogger;
    private readonly GenerateMonthlyDataCommandHandler _handler;

    public MonthlyDataGenerationCommandHandlerTests()
    {
        _mockMonthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        _mockBucketRepository = new Mock<IBucketRepository>();
        _mockLogger = new Mock<ILogger<GenerateMonthlyDataCommandHandler>>();

        _handler = new GenerateMonthlyDataCommandHandler(
            _mockMonthlyBucketRepository.Object,
            _mockBucketRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_GenerateMonthlyDataCommand_ShouldReturnTrueWhenSuccessful()
    {
        // Arrange
        var command = new GenerateMonthlyDataCommand(2025, 1);

        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var bucketRepository = new Mock<IBucketRepository>();
        var logger = new Mock<ILogger<GenerateMonthlyDataCommandHandler>>();

        var handler = new GenerateMonthlyDataCommandHandler(
            monthlyBucketRepository.Object,
            bucketRepository.Object,
            logger.Object);

        // Setup: Create test buckets
        var bucket1 = Bucket.Create("Bucket1", "Description1", 1000m).Value!;
        var bucket2 = Bucket.Create("Bucket2", "Description2", 2000m).Value!;
        var buckets = new[] { bucket1, bucket2 };

        bucketRepository.SetupAsQueryable<IBucketRepository, Bucket, int>(buckets);

        monthlyBucketRepository
            .Setup(r => r.AddAsync(It.IsAny<MonthlyBucket>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result);
        monthlyBucketRepository.Verify(r => r.AddAsync(It.IsAny<MonthlyBucket>()), Times.Exactly(2));
    }

    // Note: For comprehensive testing with IQueryable operations, consider using:
    // 1. An in-memory database (Microsoft.EntityFrameworkCore.InMemory)
    // 2. MockQueryable NuGet package for better IQueryable mocking
    // 3. Integration tests with TestContainers

    [Fact]
    public void GenerateMonthlyDataCommand_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var command = new GenerateMonthlyDataCommand(2025, 1);

        // Assert
        Assert.Equal(2025, command.Year);
        Assert.Equal(1, command.Month);
    }

    [Fact]
    public void GenerateMonthlyDataResult_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var result = new GenerateMonthlyDataResult(2025, 1, 5, 10);

        // Assert
        Assert.Equal(2025, result.Year);
        Assert.Equal(1, result.Month);
        Assert.Equal(5, result.MonthlyBucketsCreated);
        Assert.Equal(10, result.MonthlySpendingsCreated);
    }
}

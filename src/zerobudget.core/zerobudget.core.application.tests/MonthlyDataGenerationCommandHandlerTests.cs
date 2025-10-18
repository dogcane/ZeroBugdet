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
    private readonly Mock<ISpendingRepository> _mockSpendingRepository;
    private readonly Mock<IMonthlySpendingRepository> _mockMonthlySpendingRepository;
    private readonly Mock<ILogger<MonthlyBucketCommandHandlers>> _mockLogger;
    private readonly MonthlyBucketCommandHandlers _handler;

    public MonthlyDataGenerationCommandHandlerTests()
    {
        _mockMonthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        _mockBucketRepository = new Mock<IBucketRepository>();
        _mockSpendingRepository = new Mock<ISpendingRepository>();
        _mockMonthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        _mockLogger = new Mock<ILogger<MonthlyBucketCommandHandlers>>();

        _handler = new MonthlyBucketCommandHandlers(
            _mockMonthlyBucketRepository.Object,
            _mockBucketRepository.Object,
            _mockSpendingRepository.Object,
            _mockMonthlySpendingRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_GenerateMonthlyDataCommand_ShouldFailWhenDataAlreadyExists()
    {
        // Arrange
        var command = new GenerateMonthlyDataCommand(2025, 1);

        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var bucketRepository = new Mock<IBucketRepository>();
        var spendingRepository = new Mock<ISpendingRepository>();
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var logger = new Mock<ILogger<MonthlyBucketCommandHandlers>>();

        var handler = new MonthlyBucketCommandHandlers(
            monthlyBucketRepository.Object,
            bucketRepository.Object,
            spendingRepository.Object,
            monthlySpendingRepository.Object,
            logger.Object);

        // Setup: Existing monthly bucket for the same period
        monthlyBucketRepository
            .Setup(r => r.Any(It.IsAny<Func<MonthlyBucket, bool>>()))
            .Returns(true); // Simulate existing data

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
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

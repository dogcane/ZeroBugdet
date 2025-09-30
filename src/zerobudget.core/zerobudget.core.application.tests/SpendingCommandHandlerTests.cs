using Moq;
using Xunit;
using zerobudget.core.application.Commands;
using zerobudget.core.application.Handlers.Commands;
using zerobudget.core.domain;

namespace zerobudget.core.application.tests;

public class SpendingCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreateSpendingCommand_WithExistingTags_ShouldCreateSpending()
    {
        // Arrange
        var mockSpendingRepository = new Mock<ISpendingRepository>();
        var mockBucketRepository = new Mock<IBucketRepository>();
        var mockTagRepository = new Mock<ITagRepository>();
        var mockMonthlySpendingRepository = new Mock<IMonthlySpendingRepository>();

        var handler = new SpendingCommandHandlers(
            mockSpendingRepository.Object,
            mockBucketRepository.Object,
            mockTagRepository.Object,
            mockMonthlySpendingRepository.Object);

        var bucketResult = Bucket.Create("Test Bucket", "Test Description", 1000m);
        var bucket = bucketResult.Value!;

        var tag1Result = Tag.Create("Tag1", "Tag1 Description");
        var tag1 = tag1Result.Value!;
        var tag2Result = Tag.Create("Tag2", "Tag2 Description");
        var tag2 = tag2Result.Value!;

        mockBucketRepository.Setup(r => r.LoadAsync(1))
                           .ReturnsAsync(bucket);
        mockTagRepository.Setup(r => r.LoadAsync(1))
                        .ReturnsAsync(tag1);
        mockTagRepository.Setup(r => r.LoadAsync(2))
                        .ReturnsAsync(tag2);
        mockSpendingRepository.Setup(r => r.AddAsync(It.IsAny<Spending>()))
                             .Returns(Task.CompletedTask);

        var command = new CreateSpendingCommand(
            BucketId: 1,
            Description: "Test Spending",
            Amount: 100m,
            Owner: "John",
            TagIds: new[] { 1, 2 });

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Spending", result.Value.Description);
        Assert.Equal(100m, result.Value.Amount);
        Assert.Equal(2, result.Value.Tags.Length);
        mockSpendingRepository.Verify(r => r.AddAsync(It.IsAny<Spending>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateSpendingCommand_WithNonExistentBucket_ShouldReturnFailure()
    {
        // Arrange
        var mockSpendingRepository = new Mock<ISpendingRepository>();
        var mockBucketRepository = new Mock<IBucketRepository>();
        var mockTagRepository = new Mock<ITagRepository>();
        var mockMonthlySpendingRepository = new Mock<IMonthlySpendingRepository>();

        var handler = new SpendingCommandHandlers(
            mockSpendingRepository.Object,
            mockBucketRepository.Object,
            mockTagRepository.Object,
            mockMonthlySpendingRepository.Object);

        mockBucketRepository.Setup(r => r.LoadAsync(1))
                           .ReturnsAsync((Bucket?)null);

        var command = new CreateSpendingCommand(
            BucketId: 1,
            Description: "Test Spending",
            Amount: 100m,
            Owner: "John",
            TagIds: new[] { 1, 2 });

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Bucket not found", result.ErrorMessages.First());
        mockSpendingRepository.Verify(r => r.AddAsync(It.IsAny<Spending>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdateSpendingCommand_WithExistingTags_ShouldUpdateSpending()
    {
        // Arrange
        var mockSpendingRepository = new Mock<ISpendingRepository>();
        var mockBucketRepository = new Mock<IBucketRepository>();
        var mockTagRepository = new Mock<ITagRepository>();
        var mockMonthlySpendingRepository = new Mock<IMonthlySpendingRepository>();

        var handler = new SpendingCommandHandlers(
            mockSpendingRepository.Object,
            mockBucketRepository.Object,
            mockTagRepository.Object,
            mockMonthlySpendingRepository.Object);

        var bucketResult = Bucket.Create("Test Bucket", "Test Description", 1000m);
        var bucket = bucketResult.Value!;
        var spendingResult = Spending.Create("Original", 50m, "John", new Tag[0], bucket);
        var spending = spendingResult.Value!;

        var tag1Result = Tag.Create("Tag1", "Tag1 Description");
        var tag1 = tag1Result.Value!;

        mockSpendingRepository.Setup(r => r.LoadAsync(1))
                             .ReturnsAsync(spending);
        mockTagRepository.Setup(r => r.LoadAsync(1))
                        .ReturnsAsync(tag1);
        mockSpendingRepository.Setup(r => r.UpdateAsync(It.IsAny<Spending>()))
                             .Returns(Task.CompletedTask);

        var command = new UpdateSpendingCommand(
            Id: 1,
            Description: "Updated Spending",
            Amount: 150m,
            Owner: "Jane",
            TagIds: new[] { 1 });

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Updated Spending", result.Value.Description);
        Assert.Equal(150m, result.Value.Amount);
        Assert.Equal("Jane", result.Value.Owner);
        mockSpendingRepository.Verify(r => r.UpdateAsync(It.IsAny<Spending>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateSpendingCommand_WithNonExistentSpending_ShouldReturnFailure()
    {
        // Arrange
        var mockSpendingRepository = new Mock<ISpendingRepository>();
        var mockBucketRepository = new Mock<IBucketRepository>();
        var mockTagRepository = new Mock<ITagRepository>();
        var mockMonthlySpendingRepository = new Mock<IMonthlySpendingRepository>();

        var handler = new SpendingCommandHandlers(
            mockSpendingRepository.Object,
            mockBucketRepository.Object,
            mockTagRepository.Object,
            mockMonthlySpendingRepository.Object);

        mockSpendingRepository.Setup(r => r.LoadAsync(1))
                             .ReturnsAsync((Spending?)null);

        var command = new UpdateSpendingCommand(
            Id: 1,
            Description: "Updated Spending",
            Amount: 150m,
            Owner: "Jane",
            TagIds: new[] { 1 });

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Spending not found", result.ErrorMessages.First());
        mockSpendingRepository.Verify(r => r.UpdateAsync(It.IsAny<Spending>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CreateSpendingCommand_WithSomeNonExistentTags_ShouldOnlyIncludeExistingTags()
    {
        // Arrange
        var mockSpendingRepository = new Mock<ISpendingRepository>();
        var mockBucketRepository = new Mock<IBucketRepository>();
        var mockTagRepository = new Mock<ITagRepository>();
        var mockMonthlySpendingRepository = new Mock<IMonthlySpendingRepository>();

        var handler = new SpendingCommandHandlers(
            mockSpendingRepository.Object,
            mockBucketRepository.Object,
            mockTagRepository.Object,
            mockMonthlySpendingRepository.Object);

        var bucketResult = Bucket.Create("Test Bucket", "Test Description", 1000m);
        var bucket = bucketResult.Value!;

        var tag1Result = Tag.Create("Tag1", "Tag1 Description");
        var tag1 = tag1Result.Value!;

        mockBucketRepository.Setup(r => r.LoadAsync(1))
                           .ReturnsAsync(bucket);
        mockTagRepository.Setup(r => r.LoadAsync(1))
                        .ReturnsAsync(tag1);
        mockTagRepository.Setup(r => r.LoadAsync(999))
                        .ReturnsAsync((Tag?)null); // Non-existent tag
        mockSpendingRepository.Setup(r => r.AddAsync(It.IsAny<Spending>()))
                             .Returns(Task.CompletedTask);

        var command = new CreateSpendingCommand(
            BucketId: 1,
            Description: "Test Spending",
            Amount: 100m,
            Owner: "John",
            TagIds: new[] { 1, 999 });

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        // Only Tag1 should be included, Tag 999 is ignored
        Assert.Equal(1, result.Value.Tags.Length);
        Assert.Equal("Tag1", result.Value.Tags[0]);
        mockSpendingRepository.Verify(r => r.AddAsync(It.IsAny<Spending>()), Times.Once);
    }
}

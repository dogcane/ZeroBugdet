using ECO.Integrations.Moq;
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
        var spendingRepository = new Mock<ISpendingRepository>();
        var bucketRepository = new Mock<IBucketRepository>();
        var tagService = new Mock<ITagService>();
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();

        var handler = new SpendingCommandHandlers(
            spendingRepository.Object,
            bucketRepository.Object,
            tagService.Object,
            monthlySpendingRepository.Object);

        var bucketResult = Bucket.Create("Test Bucket", "Test Description", 1000m);
        var bucket = bucketResult.Value!;

        var tag1Result = Tag.Create("food");
        var tag1 = tag1Result.Value!;
        var tag2Result = Tag.Create("holiday");
        var tag2 = tag2Result.Value!;

        bucketRepository
            .SetupRepository<IBucketRepository, Bucket, int>([bucket]);

        tagService.Setup(s => s.EnsureTagsByNameAsync(It.IsAny<string[]>()))
                  .ReturnsAsync(new List<Tag> { tag1, tag2 });

        spendingRepository.Setup(r => r.AddAsync(It.IsAny<Spending>()))
                         .Returns(Task.CompletedTask);

        var command = new CreateSpendingCommand(
            Date: DateOnly.FromDateTime(DateTime.Now),
            BucketId: 1,
            Description: "Test Spending",
            Amount: 100m,
            Owner: "John",
            TagNames: new[] { "food", "holiday" });

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Spending", result.Value.Description);
        Assert.Equal(100m, result.Value.Amount);
        Assert.Equal(2, result.Value.Tags.Length);
        Assert.Contains("food", result.Value.Tags);
        Assert.Contains("holiday", result.Value.Tags);
        spendingRepository.Verify(r => r.AddAsync(It.IsAny<Spending>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateSpendingCommand_WithNewTag_ShouldCreateTag()
    {
        // Arrange
        var spendingRepository = new Mock<ISpendingRepository>();
        var bucketRepository = new Mock<IBucketRepository>();
        var tagService = new Mock<ITagService>();
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();

        var handler = new SpendingCommandHandlers(
            spendingRepository.Object,
            bucketRepository.Object,
            tagService.Object,
            monthlySpendingRepository.Object);

        var bucketResult = Bucket.Create("Test Bucket", "Test Description", 1000m);
        var bucket = bucketResult.Value!;

        var existingTagResult = Tag.Create("food");
        var existingTag = existingTagResult.Value!;
        var newTagResult = Tag.Create("cinema");
        var newTag = newTagResult.Value!;

        bucketRepository
            .SetupRepository<IBucketRepository, Bucket, int>([bucket]);

        tagService.Setup(s => s.EnsureTagsByNameAsync(It.IsAny<string[]>()))
                  .ReturnsAsync(new List<Tag> { existingTag, newTag });

        spendingRepository.Setup(r => r.AddAsync(It.IsAny<Spending>()))
                         .Returns(Task.CompletedTask);

        var command = new CreateSpendingCommand(
            Date: DateOnly.FromDateTime(DateTime.Now),
            BucketId: 1,
            Description: "Movie night",
            Amount: 50m,
            Owner: "John",
            TagNames: new[] { "food", "cinema" }); // "cinema" is new

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Tags.Length);
        Assert.Contains("food", result.Value.Tags);
        Assert.Contains("cinema", result.Value.Tags);
        spendingRepository.Verify(r => r.AddAsync(It.IsAny<Spending>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateSpendingCommand_WithNonExistentBucket_ShouldReturnFailure()
    {
        // Arrange
        var spendingRepository = new Mock<ISpendingRepository>();
        var bucketRepository = new Mock<IBucketRepository>();
        var tagService = new Mock<ITagService>();
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();

        var handler = new SpendingCommandHandlers(
            spendingRepository.Object,
            bucketRepository.Object,
            tagService.Object,
            monthlySpendingRepository.Object);

        bucketRepository
            .SetupRepository<IBucketRepository, Bucket, int>([]);

        var command = new CreateSpendingCommand(
            Date: DateOnly.FromDateTime(DateTime.Now),
            BucketId: 1,
            Description: "Test Spending",
            Amount: 100m,
            Owner: "John",
            TagNames: new[] { "food" });

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        spendingRepository.Verify(r => r.AddAsync(It.IsAny<Spending>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdateSpendingCommand_WithExistingTags_ShouldUpdateSpending()
    {
        // Arrange
        var spendingRepository = new Mock<ISpendingRepository>();
        var bucketRepository = new Mock<IBucketRepository>();
        var tagService = new Mock<ITagService>();
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();

        var handler = new SpendingCommandHandlers(
            spendingRepository.Object,
            bucketRepository.Object,
            tagService.Object,
            monthlySpendingRepository.Object);

        var bucketResult = Bucket.Create("Test Bucket", "Test Description", 1000m);
        var bucket = bucketResult.Value!;
        var spendingResult = Spending.Create("Original", 50m, "John", new Tag[0], bucket);
        var spending = spendingResult.Value!;

        var tag1Result = Tag.Create("food");
        var tag1 = tag1Result.Value!;

        spendingRepository
            .SetupRepository<ISpendingRepository, Spending, int>([spending]);

        tagService.Setup(s => s.EnsureTagsByNameAsync(It.IsAny<string[]>()))
                  .ReturnsAsync(new List<Tag> { tag1 });

        spendingRepository.Setup(r => r.UpdateAsync(It.IsAny<Spending>()))
                         .Returns(Task.CompletedTask);

        var command = new UpdateSpendingCommand(
            Id: 1,
            Date: DateOnly.FromDateTime(DateTime.Now),
            Description: "Updated Spending",
            Amount: 150m,
            Owner: "Jane",
            TagNames: new[] { "food" });

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Updated Spending", result.Value.Description);
        Assert.Equal(150m, result.Value.Amount);
        Assert.Equal("Jane", result.Value.Owner);
        Assert.Contains("food", result.Value.Tags);
        spendingRepository.Verify(r => r.UpdateAsync(It.IsAny<Spending>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateSpendingCommand_WithNonExistentSpending_ShouldReturnFailure()
    {
        // Arrange
        var spendingRepository = new Mock<ISpendingRepository>();
        var bucketRepository = new Mock<IBucketRepository>();
        var tagService = new Mock<ITagService>();
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();

        var handler = new SpendingCommandHandlers(
            spendingRepository.Object,
            bucketRepository.Object,
            tagService.Object,
            monthlySpendingRepository.Object);

        spendingRepository
            .SetupRepository<ISpendingRepository, Spending, int>([]);

        var command = new UpdateSpendingCommand(
            Id: 1,
            Date: DateOnly.FromDateTime(DateTime.Now),
            Description: "Updated Spending",
            Amount: 150m,
            Owner: "Jane",
            TagNames: new[] { "food" });

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        spendingRepository.Verify(r => r.UpdateAsync(It.IsAny<Spending>()), Times.Never);
    }
}

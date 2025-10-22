using ECO.Data;
using ECO.Integrations.Moq;
using Moq;
using Xunit;
using zerobudget.core.application.Commands;
using zerobudget.core.application.Handlers.Commands;
using zerobudget.core.domain;

namespace zerobudget.core.application.tests;

public class BucketCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreateBucketCommand_ShouldCreateBucket()
    {
        // Arrange
        var bucketRepository = new Mock<IBucketRepository>();
        var handler = new CreateBucketCommandHandler(bucketRepository.Object);
        var command = new CreateBucketCommand("Test Bucket", "Test Description", 1000m);

        bucketRepository.Setup(r => r.AddAsync(It.IsAny<Bucket>()))
                     .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Bucket", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(1000m, result.DefaultLimit);
        bucketRepository.Verify(r => r.AddAsync(It.IsAny<Bucket>()), Times.Once);
    }


    [Fact]
    public async Task Handle_UpdateBucketCommand_ShouldUpdateExistingBucket()
    {
        // Arrange
        var bucketRepository = new Mock<IBucketRepository>();
        var handler = new UpdateBucketCommandHandler(bucketRepository.Object);
        var bucketResult = Bucket.Create("Original", "Original Description", 500m);
        var bucket = bucketResult.Value!;

        var command = new UpdateBucketCommand(1, "Updated Bucket", "Updated Description", 1500m);
        bucketRepository
            .SetupRepository<IBucketRepository, Bucket, int>([bucket]);
        bucketRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Bucket>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Bucket", result.Name);
        Assert.Equal("Updated Description", result.Description);
        Assert.Equal(1500m, result.DefaultLimit);
        bucketRepository.Verify(r => r.UpdateAsync(It.IsAny<Bucket>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteBucketCommand_ShouldDeleteExistingBucket()
    {
        // Arrange
        var bucketRepository = new Mock<IBucketRepository>();
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var handler = new DeleteBucketCommandHandler(bucketRepository.Object, monthlyBucketRepository.Object);
        var bucketResult = Bucket.Create("Test", "Test Description", 1000m);
        var bucket = bucketResult.Value!;

        var command = new DeleteBucketCommand(1);

        bucketRepository
            .SetupRepository<IBucketRepository, Bucket, int>([bucket]);
        monthlyBucketRepository
            .Setup(r => r.Any(It.IsAny<Func<MonthlyBucket, bool>>()))
            .Returns(false);
        bucketRepository.Setup(r => r.RemoveAsync(It.IsAny<Bucket>()))
                     .Returns(Task.CompletedTask);

        // Act
        await handler.Handle(command);

        // Assert
        bucketRepository.Verify(r => r.RemoveAsync(It.IsAny<Bucket>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateBucketCommand_WithNonExistentBucket_ShouldThrowException()
    {
        // Arrange
        var bucketRepository = new Mock<IBucketRepository>();
        var handler = new UpdateBucketCommandHandler(bucketRepository.Object);
        var command = new UpdateBucketCommand(999, "Test", "Test Description", 1000m);

        bucketRepository
            .SetupRepository<IBucketRepository, Bucket, int>([]);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command));
    }
}

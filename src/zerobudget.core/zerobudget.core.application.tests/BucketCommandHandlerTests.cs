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
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var handler = new BucketCommandHandlers(bucketRepository.Object, monthlyBucketRepository.Object);
        var command = new CreateBucketCommand("Test Bucket", "Test Description", 1000m);

        bucketRepository.Setup(r => r.AddAsync(It.IsAny<Bucket>()))
                     .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Bucket", result.Value.Name);
        Assert.Equal("Test Description", result.Value.Description);
        Assert.Equal(1000m, result.Value.DefaultLimit);
        bucketRepository.Verify(r => r.AddAsync(It.IsAny<Bucket>()), Times.Once);
    }


    [Fact]
    public async Task Handle_UpdateBucketCommand_ShouldUpdateExistingBucket()
    {
        // Arrange
        var bucketRepository = new Mock<IBucketRepository>();
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var handler = new BucketCommandHandlers(bucketRepository.Object, monthlyBucketRepository.Object);
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
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Updated Bucket", result.Value.Name);
        Assert.Equal("Updated Description", result.Value.Description);
        Assert.Equal(1500m, result.Value.DefaultLimit);
        bucketRepository.Verify(r => r.UpdateAsync(It.IsAny<Bucket>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteBucketCommand_ShouldDeleteExistingBucket()
    {
        // Arrange
        var bucketRepository = new Mock<IBucketRepository>();
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var handler = new BucketCommandHandlers(bucketRepository.Object, monthlyBucketRepository.Object);
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
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        bucketRepository.Verify(r => r.RemoveAsync(It.IsAny<Bucket>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateBucketCommand_WithNonExistentBucket_ShouldReturnFailure()
    {
        // Arrange
        var bucketRepository = new Mock<IBucketRepository>();
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var handler = new BucketCommandHandlers(bucketRepository.Object, monthlyBucketRepository.Object);
        var command = new UpdateBucketCommand(999, "Test", "Test Description", 1000m);

        bucketRepository
            .SetupRepository<IBucketRepository, Bucket, int>([]);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }
}

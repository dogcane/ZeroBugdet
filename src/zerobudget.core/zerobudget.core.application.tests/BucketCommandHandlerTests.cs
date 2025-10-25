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
        Assert.True(result.Success);
        Assert.Equal("Test Bucket", result.Value!.Name);
        Assert.Equal("Test Description", result.Value.Description);
        Assert.Equal(1000m, result.Value.DefaultLimit);
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
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .Returns(new ValueTask<Bucket?>(bucket));
        bucketRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Bucket>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Updated Bucket", result.Value!.Name);
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
        var handler = new DeleteBucketCommandHandler(bucketRepository.Object, monthlyBucketRepository.Object);
        var bucketResult = Bucket.Create("Test", "Test Description", 1000m);
        var bucket = bucketResult.Value!;

        var command = new DeleteBucketCommand(1);

        bucketRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .Returns(new ValueTask<Bucket?>(bucket));
        
        // Setup monthlyBucketRepository as IQueryable with empty data
        // This will make Any() return false
        monthlyBucketRepository.SetupAsQueryable<IMonthlyBucketRepository, MonthlyBucket, int>(Array.Empty<MonthlyBucket>());
        
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
        var handler = new UpdateBucketCommandHandler(bucketRepository.Object);
        var command = new UpdateBucketCommand(999, "Test", "Test Description", 1000m);

        bucketRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .Returns(new ValueTask<Bucket?>(null as Bucket));

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }
}

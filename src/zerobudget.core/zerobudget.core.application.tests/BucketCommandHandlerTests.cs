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
        var mockRepository = new Mock<IBucketRepository>();
        var handler = new BucketCommandHandlers(mockRepository.Object);
        var command = new CreateBucketCommand("Test Bucket", "Test Description", 1000m);

        mockRepository.Setup(r => r.AddAsync(It.IsAny<Bucket>()))
                     .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Bucket", result.Value.Name);
        Assert.Equal("Test Description", result.Value.Description);
        Assert.Equal(1000m, result.Value.DefaultLimit);
        mockRepository.Verify(r => r.AddAsync(It.IsAny<Bucket>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateBucketCommand_ShouldUpdateExistingBucket()
    {
        // Arrange
        var mockRepository = new Mock<IBucketRepository>();
        var handler = new BucketCommandHandlers(mockRepository.Object);
        var bucketResult = Bucket.Create("Original", "Original Description", 500m);
        var bucket = bucketResult.Value!;
        
        var command = new UpdateBucketCommand(1, "Updated Bucket", "Updated Description", 1500m);

        mockRepository.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(bucket);
        mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Bucket>()))
                     .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Updated Bucket", result.Value.Name);
        Assert.Equal("Updated Description", result.Value.Description);
        Assert.Equal(1500m, result.Value.DefaultLimit);
        mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Bucket>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteBucketCommand_ShouldDeleteExistingBucket()
    {
        // Arrange
        var mockRepository = new Mock<IBucketRepository>();
        var handler = new BucketCommandHandlers(mockRepository.Object);
        var bucketResult = Bucket.Create("Test", "Test Description", 1000m);
        var bucket = bucketResult.Value!;

        var command = new DeleteBucketCommand(1);

        mockRepository.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(bucket);
        mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Bucket>()))
                     .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.IsSuccess);
        mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Bucket>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateBucketCommand_WithNonExistentBucket_ShouldReturnFailure()
    {
        // Arrange
        var mockRepository = new Mock<IBucketRepository>();
        var handler = new BucketCommandHandlers(mockRepository.Object);
        var command = new UpdateBucketCommand(999, "Test", "Test Description", 1000m);

        mockRepository.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((Bucket?)null);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Bucket not found", result.Errors);
    }
}
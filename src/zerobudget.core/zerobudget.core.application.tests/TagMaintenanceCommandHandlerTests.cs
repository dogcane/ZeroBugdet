using Moq;
using Xunit;
using zerobudget.core.application.Commands;
using zerobudget.core.application.Handlers.Commands;
using zerobudget.core.domain;
using Microsoft.Extensions.Logging;

namespace zerobudget.core.application.tests;

public class TagMaintenanceCommandHandlerTests
{
    [Fact]
    public async Task Handle_CleanupUnusedTagsCommand_ShouldRemoveUnusedTags()
    {
        // Arrange
        var mockTagRepository = new Mock<ITagRepository>();
        var mockLogger = new Mock<ILogger<TagMaintenanceCommandHandlers>>();
        var handler = new TagMaintenanceCommandHandlers(mockTagRepository.Object, mockLogger.Object);
        var command = new CleanupUnusedTagsCommand();

        // Setup: 5 tags were removed
        mockTagRepository.Setup(r => r.RemoveUnusedTagsAsync())
                        .ReturnsAsync(5);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(5, result.Value);
        mockTagRepository.Verify(r => r.RemoveUnusedTagsAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_CleanupUnusedTagsCommand_NoUnusedTags_ShouldReturnZero()
    {
        // Arrange
        var mockTagRepository = new Mock<ITagRepository>();
        var mockLogger = new Mock<ILogger<TagMaintenanceCommandHandlers>>();
        var handler = new TagMaintenanceCommandHandlers(mockTagRepository.Object, mockLogger.Object);
        var command = new CleanupUnusedTagsCommand();

        // Setup: No tags were removed
        mockTagRepository.Setup(r => r.RemoveUnusedTagsAsync())
                        .ReturnsAsync(0);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.Value);
        mockTagRepository.Verify(r => r.RemoveUnusedTagsAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_CleanupUnusedTagsCommand_ThrowsException_ShouldReturnFailure()
    {
        // Arrange
        var mockTagRepository = new Mock<ITagRepository>();
        var mockLogger = new Mock<ILogger<TagMaintenanceCommandHandlers>>();
        var handler = new TagMaintenanceCommandHandlers(mockTagRepository.Object, mockLogger.Object);
        var command = new CleanupUnusedTagsCommand();

        // Setup: Exception is thrown
        mockTagRepository.Setup(r => r.RemoveUnusedTagsAsync())
                        .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Database error", result.ErrorMessages.First());
        mockTagRepository.Verify(r => r.RemoveUnusedTagsAsync(), Times.Once);
    }
}

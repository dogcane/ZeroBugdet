using ECO.Data;
using Moq;
using Xunit;
using zerobudget.core.application.Commands;
using zerobudget.core.application.Handlers.Commands;
using zerobudget.core.domain;

namespace zerobudget.core.application.tests;

public class TagCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreateTagCommand_ShouldCreateTag()
    {
        // Arrange
        var tagRepository = new Mock<ITagRepository>();
        var handler = new CreateTagCommandHandler(tagRepository.Object);
        var command = new CreateTagCommand("TestTag");

        tagRepository.Setup(r => r.AddAsync(It.IsAny<Tag>()))
                     .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testtag", result.Name);
        tagRepository.Verify(r => r.AddAsync(It.IsAny<Tag>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CreateTagCommand_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var tagRepository = new Mock<ITagRepository>();
        var handler = new CreateTagCommandHandler(tagRepository.Object);
        var command = new CreateTagCommand("");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command));
        tagRepository.Verify(r => r.AddAsync(It.IsAny<Tag>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeleteTagCommand_ShouldDeleteExistingTag()
    {
        // Arrange
        var tagRepository = new Mock<ITagRepository>();
        var handler = new DeleteTagCommandHandler(tagRepository.Object);
        var tagResult = Tag.Create("TestTag");
        var tag = tagResult.Value!;

        var command = new DeleteTagCommand(1);

        tagRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync(tag);
        tagRepository.Setup(r => r.RemoveAsync(It.IsAny<Tag>()))
                     .Returns(Task.CompletedTask);

        // Act
        await handler.Handle(command);

        // Assert
        tagRepository.Verify(r => r.RemoveAsync(It.IsAny<Tag>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteTagCommand_WithNonExistentTag_ShouldThrowException()
    {
        // Arrange
        var tagRepository = new Mock<ITagRepository>();
        var handler = new DeleteTagCommandHandler(tagRepository.Object);
        var command = new DeleteTagCommand(999);

        tagRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync((Tag?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command));
        tagRepository.Verify(r => r.RemoveAsync(It.IsAny<Tag>()), Times.Never);
    }
}

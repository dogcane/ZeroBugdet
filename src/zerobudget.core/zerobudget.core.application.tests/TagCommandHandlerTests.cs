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
        var handler = new TagCommandHandlers(tagRepository.Object);
        var command = new CreateTagCommand("TestTag");
        
        tagRepository.Setup(r => r.AddAsync(It.IsAny<Tag>()))
                     .Returns(Task.CompletedTask);
        
        // Act
        var result = await handler.Handle(command);
        
        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("testtag", result.Value.Name);
        tagRepository.Verify(r => r.AddAsync(It.IsAny<Tag>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_CreateTagCommand_WithEmptyName_ShouldReturnFailure()
    {
        // Arrange
        var tagRepository = new Mock<ITagRepository>();
        var handler = new TagCommandHandlers(tagRepository.Object);
        var command = new CreateTagCommand("");
        
        // Act
        var result = await handler.Handle(command);
        
        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        tagRepository.Verify(r => r.AddAsync(It.IsAny<Tag>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_DeleteTagCommand_ShouldDeleteExistingTag()
    {
        // Arrange
        var tagRepository = new Mock<ITagRepository>();
        var handler = new TagCommandHandlers(tagRepository.Object);
        var tagResult = Tag.Create("TestTag");
        var tag = tagResult.Value!;
        
        var command = new DeleteTagCommand(1);
        
        tagRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync(tag);
        tagRepository.Setup(r => r.RemoveAsync(It.IsAny<Tag>()))
                     .Returns(Task.CompletedTask);
        
        // Act
        var result = await handler.Handle(command);
        
        // Assert
        Assert.True(result.Success);
        tagRepository.Verify(r => r.RemoveAsync(It.IsAny<Tag>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_DeleteTagCommand_WithNonExistentTag_ShouldReturnFailure()
    {
        // Arrange
        var tagRepository = new Mock<ITagRepository>();
        var handler = new TagCommandHandlers(tagRepository.Object);
        var command = new DeleteTagCommand(999);
        
        tagRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync((Tag?)null);
        
        // Act
        var result = await handler.Handle(command);
        
        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        tagRepository.Verify(r => r.RemoveAsync(It.IsAny<Tag>()), Times.Never);
    }
}

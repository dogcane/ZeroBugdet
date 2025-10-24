using ECO.Data;
using Moq;
using Xunit;
using zerobudget.core.application.Handlers.Queries;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;

namespace zerobudget.core.application.tests;

public class TagQueryHandlerTests
{
    [Fact]
    public async Task Handle_GetTagByIdQuery_ReturnsTag()
    {
        // Arrange
        var tagRepository = new Mock<ITagRepository>();
        var handler = new GetTagByIdQueryHandler(tagRepository.Object);

        var tag = Tag.Create("TestTag").Value!;

        tagRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync(tag);

        var query = new GetTagByIdQuery(1);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testtag", result.Name);
    }

    [Fact]
    public async Task Handle_GetTagByIdQuery_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var tagRepository = new Mock<ITagRepository>();
        var handler = new GetTagByIdQueryHandler(tagRepository.Object);

        tagRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync((Tag?)null);

        var query = new GetTagByIdQuery(999);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_GetAllTagsQuery_ReturnsAllTags()
    {
        // Arrange
        var tagRepository = new Mock<ITagRepository>();
        var handler = new GetAllTagsQueryHandler(tagRepository.Object);

        var tag1 = Tag.Create("Tag1").Value!;
        var tag2 = Tag.Create("Tag2").Value!;

        tagRepository.SetupAsQueryable<ITagRepository, Tag, int>(new[] { tag1, tag2 });

        var query = new GetAllTagsQuery();

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Handle_GetTagsByNameQuery_ReturnsFilteredTags()
    {
        // Arrange
        var tagRepository = new Mock<ITagRepository>();
        var handler = new GetTagsByNameQueryHandler(tagRepository.Object);

        var tag1 = Tag.Create("TestTag1").Value!;
        var tag2 = Tag.Create("OtherTag").Value!;

        tagRepository.SetupAsQueryable<ITagRepository, Tag, int>(new[] { tag1, tag2 });

        var query = new GetTagsByNameQuery("test");

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("testtag1", result.First().Name);
    }
}

using Xunit;
using zerobudget.core.domain;

namespace zerobudget.core.domain.tests;

public class TagRepositoryTests
{
    [Fact]
    public void Tag_Create_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var result = Tag.Create("ValidTag123", "A valid tag description");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("ValidTag123", result.Value.Name);
        Assert.Equal("A valid tag description", result.Value.Description);
    }

    [Fact]
    public void Tag_Create_WithInvalidName_ShouldFail()
    {
        // Arrange & Act - Name too short
        var result = Tag.Create("abc", "Description");

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Tag_Create_WithSpecialCharacters_ShouldFail()
    {
        // Arrange & Act - Special characters not allowed
        var result = Tag.Create("Tag@123", "Description");

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Tag_Update_WithValidData_ShouldSucceed()
    {
        // Arrange
        var createResult = Tag.Create("OriginalTag", "Original description");
        var tag = createResult.Value!;

        // Act
        var updateResult = tag.Update("UpdatedTag", "Updated description");

        // Assert
        Assert.True(updateResult.Success);
        Assert.Equal("UpdatedTag", tag.Name);
        Assert.Equal("Updated description", tag.Description);
    }

    [Fact]
    public void Tag_Update_WithInvalidName_ShouldFail()
    {
        // Arrange
        var createResult = Tag.Create("OriginalTag", "Original description");
        var tag = createResult.Value!;

        // Act
        var updateResult = tag.Update("ab", "Updated description"); // Name too short

        // Assert
        Assert.False(updateResult.Success);
        Assert.NotEmpty(updateResult.Errors);
        // Original values should remain unchanged
        Assert.Equal("OriginalTag", tag.Name);
        Assert.Equal("Original description", tag.Description);
    }

    [Fact]
    public void Tag_Create_WithEmptyName_ShouldFail()
    {
        // Arrange & Act
        var result = Tag.Create("", "Description");

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Tag_Create_WithEmptyDescription_ShouldFail()
    {
        // Arrange & Act
        var result = Tag.Create("ValidTag", "");

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Tag_Create_WithDescriptionTooLong_ShouldFail()
    {
        // Arrange - Description over 500 characters
        var longDescription = new string('a', 501);

        // Act
        var result = Tag.Create("ValidTag", longDescription);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }
}

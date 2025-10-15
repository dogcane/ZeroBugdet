using Xunit;
using zerobudget.core.domain;

namespace zerobudget.core.domain.tests;

public class TagRepositoryTests
{
    [Fact]
    public void Tag_Create_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var result = Tag.Create("ValidTag123");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("ValidTag123", result.Value.Name);
    }

    [Fact]
    public void Tag_Create_WithInvalidName_ShouldFail()
    {
        // Arrange & Act - Name too short
        var result = Tag.Create("abc");

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Tag_Create_WithSpecialCharacters_ShouldFail()
    {
        // Arrange & Act - Special characters not allowed
        var result = Tag.Create("Tag@123");

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Tag_Create_WithEmptyName_ShouldFail()
    {
        // Arrange & Act
        var result = Tag.Create("");

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }
}

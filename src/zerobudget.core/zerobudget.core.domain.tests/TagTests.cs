using Xunit;
using zerobudget.core.domain;

namespace zerobudget.core.domain.tests;

public class TagTests
{
    #region Create Tests
    [Fact]
    public void Create_ValidTag_ReturnsSuccess()
    {
        var result = Tag.Create("TestTag");
        Assert.True(result.Success);
        Assert.Equal("testtag", result.Value!.Name);
    }

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = Tag.Create("ValidTag123");
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("validtag123", result.Value.Name);
    }

    [Fact]
    public void Create_WithUpperCase_ConvertsToLowerCase()
    {
        var result = Tag.Create("UPPERCASE");
        Assert.True(result.Success);
        Assert.Equal("uppercase", result.Value!.Name);
    }

    [Fact]
    public void Create_WithMixedCase_ConvertsToLowerCase()
    {
        var result = Tag.Create("MiXeDCaSe123");
        Assert.True(result.Success);
        Assert.Equal("mixedcase123", result.Value!.Name);
    }

    [Fact]
    public void Create_WithMinimumLength_ReturnsSuccess()
    {
        var result = Tag.Create("test");
        Assert.True(result.Success);
        Assert.Equal("test", result.Value!.Name);
    }

    [Fact]
    public void Create_WithMaximumLength_ReturnsSuccess()
    {
        var result = Tag.Create(new string('a', 50));
        Assert.True(result.Success);
        Assert.Equal(new string('a', 50), result.Value!.Name);
    }
    #endregion

    #region Validation Tests
    [Fact]
    public void Create_WithInvalidName_ShouldFail()
    {
        var result = Tag.Create("abc");
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithSpecialCharacters_ShouldFail()
    {
        var result = Tag.Create("Tag@123");
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithSpaces_ShouldFail()
    {
        var result = Tag.Create("Tag With Spaces");
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithHyphen_ShouldFail()
    {
        var result = Tag.Create("Tag-Name");
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithUnderscore_ShouldFail()
    {
        var result = Tag.Create("Tag_Name");
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldFail()
    {
        var result = Tag.Create("");
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithNullName_ShouldFail()
    {
        var result = Tag.Create(null!);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithTooShortName_ShouldFail()
    {
        var result = Tag.Create("ab");
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithTooLongName_ShouldFail()
    {
        var result = Tag.Create(new string('a', 51));
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Validate_WithValidName_ReturnsSuccess()
    {
        var result = Tag.Validate("ValidTag123");
        Assert.True(result.Success);
    }

    [Fact]
    public void Validate_WithInvalidName_ReturnsFailure()
    {
        var result = Tag.Validate("Invalid@Tag");
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }
    #endregion
}

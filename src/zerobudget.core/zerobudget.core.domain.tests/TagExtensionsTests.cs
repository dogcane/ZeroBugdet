using Xunit;
using zerobudget.core.domain;

namespace zerobudget.core.domain.tests;

public class TagExtensionsTests
{
    #region ToTagNames Tests
    [Fact]
    public void ToTagNames_WithEmptyCollection_ReturnsEmptyArray()
    {
        var tags = Array.Empty<Tag>();
        var result = tags.ToTagNames();
        Assert.Empty(result);
    }

    [Fact]
    public void ToTagNames_WithSingleTag_ReturnsSingleName()
    {
        var tag = Tag.Create("TestTag").Value!;
        var tags = new[] { tag };
        var result = tags.ToTagNames();
        Assert.Single(result);
        Assert.Equal("testtag", result[0]);
    }

    [Fact]
    public void ToTagNames_WithMultipleTags_ReturnsAllNames()
    {
        var tag1 = Tag.Create("Tag1").Value!;
        var tag2 = Tag.Create("Tag2").Value!;
        var tag3 = Tag.Create("Tag3").Value!;
        var tags = new[] { tag1, tag2, tag3 };
        var result = tags.ToTagNames();
        Assert.Equal(3, result.Length);
        Assert.Contains("tag1", result);
        Assert.Contains("tag2", result);
        Assert.Contains("tag3", result);
    }

    [Fact]
    public void ToTagNames_WithDuplicateTags_ReturnsDistinctNames()
    {
        var tag1 = Tag.Create("TestTag").Value!;
        var tag2 = Tag.Create("TESTTAG").Value!;
        var tags = new[] { tag1, tag2 };
        var result = tags.ToTagNames();
        Assert.Single(result);
        Assert.Equal("testtag", result[0]);
    }

    [Fact]
    public void ToTagNames_ResultIsSorted()
    {
        var tag1 = Tag.Create("Zebra").Value!;
        var tag2 = Tag.Create("Alpha").Value!;
        var tag3 = Tag.Create("Beta").Value!;
        var tags = new[] { tag1, tag2, tag3 };
        var result = tags.ToTagNames();
        Assert.Equal(3, result.Length);
        Assert.Equal("alpha", result[0]);
        Assert.Equal("beta", result[1]);
        Assert.Equal("zebra", result[2]);
    }

    [Fact]
    public void ToTagNames_ConvertsToLowerCase()
    {
        var tag = Tag.Create("UPPERCASE").Value!;
        var tags = new[] { tag };
        var result = tags.ToTagNames();
        Assert.Single(result);
        Assert.Equal("uppercase", result[0]);
    }
    #endregion

    #region NormalizeTagNames Tests
    [Fact]
    public void NormalizeTagNames_WithEmptyCollection_ReturnsEmptyArray()
    {
        var tagNames = Array.Empty<string>();
        var result = tagNames.NormalizeTagNames();
        Assert.Empty(result);
    }

    [Fact]
    public void NormalizeTagNames_WithSingleName_ReturnsSingleName()
    {
        var tagNames = new[] { "TestTag" };
        var result = tagNames.NormalizeTagNames();
        Assert.Single(result);
        Assert.Equal("testtag", result[0]);
    }

    [Fact]
    public void NormalizeTagNames_WithMultipleNames_ReturnsAllNames()
    {
        var tagNames = new[] { "Tag1", "Tag2", "Tag3" };
        var result = tagNames.NormalizeTagNames();
        Assert.Equal(3, result.Length);
        Assert.Contains("tag1", result);
        Assert.Contains("tag2", result);
        Assert.Contains("tag3", result);
    }

    [Fact]
    public void NormalizeTagNames_WithDuplicateNames_ReturnsDistinctNames()
    {
        var tagNames = new[] { "TestTag", "TESTTAG", "testtag" };
        var result = tagNames.NormalizeTagNames();
        Assert.Single(result);
        Assert.Equal("testtag", result[0]);
    }

    [Fact]
    public void NormalizeTagNames_ResultIsSorted()
    {
        var tagNames = new[] { "Zebra", "Alpha", "Beta" };
        var result = tagNames.NormalizeTagNames();
        Assert.Equal(3, result.Length);
        Assert.Equal("alpha", result[0]);
        Assert.Equal("beta", result[1]);
        Assert.Equal("zebra", result[2]);
    }

    [Fact]
    public void NormalizeTagNames_ConvertsToLowerCase()
    {
        var tagNames = new[] { "UPPERCASE", "MixedCase", "lowercase" };
        var result = tagNames.NormalizeTagNames();
        Assert.Equal(3, result.Length);
        Assert.All(result, name => Assert.Equal(name, name.ToLowerInvariant()));
    }

    [Fact]
    public void NormalizeTagNames_TrimsWhitespace()
    {
        var tagNames = new[] { "  Tag1  ", " Tag2 ", "Tag3   " };
        var result = tagNames.NormalizeTagNames();
        Assert.Equal(3, result.Length);
        Assert.Contains("tag1", result);
        Assert.Contains("tag2", result);
        Assert.Contains("tag3", result);
    }

    [Fact]
    public void NormalizeTagNames_FiltersOutEmptyStrings()
    {
        var tagNames = new[] { "Tag1", "", "Tag2", "   ", "Tag3" };
        var result = tagNames.NormalizeTagNames();
        Assert.Equal(3, result.Length);
        Assert.Contains("tag1", result);
        Assert.Contains("tag2", result);
        Assert.Contains("tag3", result);
    }

    [Fact]
    public void NormalizeTagNames_FiltersOutWhitespaceOnly()
    {
        var tagNames = new[] { "Tag1", "   ", "\t", "\n", "Tag2" };
        var result = tagNames.NormalizeTagNames();
        Assert.Equal(2, result.Length);
        Assert.Contains("tag1", result);
        Assert.Contains("tag2", result);
    }

    [Fact]
    public void NormalizeTagNames_WithMixedCaseAndWhitespace_NormalizesCorrectly()
    {
        var tagNames = new[] { "  UPPERCASE  ", " MixedCase ", "lowercase" };
        var result = tagNames.NormalizeTagNames();
        Assert.Equal(3, result.Length);
        Assert.Equal("lowercase", result[0]);
        Assert.Equal("mixedcase", result[1]);
        Assert.Equal("uppercase", result[2]);
    }
    #endregion
}

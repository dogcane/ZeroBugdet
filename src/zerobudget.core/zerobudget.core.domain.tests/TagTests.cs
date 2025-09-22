namespace zerobudget.core.domain.tests;

public class TagTests
{
    [Fact]
    public void Create_ValidTag_ReturnsSuccess()
    {
        var result = Tag.Create("TestTag");
        Assert.True(result.Success);
        Assert.Equal("TestTag", result.Value!.Name);
    }

    [Fact]
    public void Update_ValidName_UpdatesName()
    {
        var tag = Tag.Create("OldName").Value!;
        tag.Update("NewName");
        Assert.Equal("NewName", tag.Name);
    }
}

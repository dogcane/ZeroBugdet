namespace zerobudget.core.domain.tests;

public class TagTests
{
    [Fact]
    public void Create_ValidTag_ReturnsSuccess()
    {
        var result = Tag.Create("TestTag", "Test Description");
        Assert.True(result.Success);
        Assert.Equal("TestTag", result.Value!.Name);
    }

    [Fact]
    public void Update_ValidName_UpdatesName()
    {
        var tag = Tag.Create("OldName", "Old Description").Value!;
        tag.Update("NewName", "New Description");
        Assert.Equal("NewName", tag.Name);
    }
}

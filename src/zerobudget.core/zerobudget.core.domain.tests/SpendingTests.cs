namespace zerobudget.core.domain.tests;

public class SpendingTests
{
    [Fact]
    public void Create_ValidSpending_ReturnsSuccess()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var result = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket);
        Assert.True(result.Success);
        Assert.Equal(10m, result.Value!.Amount);
        Assert.Equal("desc", result.Value.Description);
        Assert.Equal("owner", result.Value.Owner);
        Assert.Equal(bucket.Identity, result.Value.BucketId);
    }

    [Fact]
    public void Update_ValidData_UpdatesProperties()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var spending = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        spending.Update("newdesc", 20m, "newowner", Array.Empty<Tag>());
        Assert.Equal(20m, spending.Amount);
        Assert.Equal("newdesc", spending.Description);
        Assert.Equal("newowner", spending.Owner);
    }
}

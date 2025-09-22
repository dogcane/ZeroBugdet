namespace zerobudget.core.domain.tests;

public class BucketTests
{
    [Fact]
    public void Create_ValidBucket_ReturnsSuccess()
    {
        var result = Bucket.Create("Test Bucket", "Test Description", 100m);
        Assert.True(result.Success);
        Assert.Equal("Test Bucket", result.Value!.Name);
        Assert.Equal("Test Description", result.Value.Description);
        Assert.Equal(100m, result.Value.DefaultLimit);
    }

    [Fact]
    public void Update_ValidData_UpdatesProperties()
    {
        var bucket = Bucket.Create("Old Name", "Old Desc", 50m).Value!;
        bucket.Update("New Name", "New Desc", 200m);
        Assert.Equal("New Name", bucket.Name);
        Assert.Equal("New Desc", bucket.Description);
        Assert.Equal(200m, bucket.DefaultLimit);
    }
}

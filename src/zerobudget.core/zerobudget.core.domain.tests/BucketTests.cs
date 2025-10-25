namespace zerobudget.core.domain.tests;

public class BucketTests
{
    #region Create Tests
    [Fact]
    public void Create_ValidBucket_ReturnsSuccess()
    {
        var result = Bucket.Create("Test Bucket", "Test Description", 100m);
        Assert.True(result.Success);
        Assert.Equal("Test Bucket", result.Value!.Name);
        Assert.Equal("Test Description", result.Value.Description);
        Assert.Equal(100m, result.Value.DefaultLimit);
        Assert.Equal(0m, result.Value.DefaultBalance);
        Assert.True(result.Value.Enabled);
    }

    [Fact]
    public void Create_WithEmptyName_ReturnsFailure()
    {
        var result = Bucket.Create("", "Test Description", 100m);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithNullName_ReturnsFailure()
    {
        var result = Bucket.Create(null!, "Test Description", 100m);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithNameTooLong_ReturnsFailure()
    {
        var result = Bucket.Create(new string('a', 101), "Test Description", 100m);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithEmptyDescription_ReturnsFailure()
    {
        var result = Bucket.Create("Test Bucket", "", 100m);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithNullDescription_ReturnsFailure()
    {
        var result = Bucket.Create("Test Bucket", null!, 100m);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ReturnsFailure()
    {
        var result = Bucket.Create("Test Bucket", new string('a', 501), 100m);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithNegativeDefaultLimit_ReturnsFailure()
    {
        var result = Bucket.Create("Test Bucket", "Test Description", -1m);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithZeroDefaultLimit_ReturnsSuccess()
    {
        var result = Bucket.Create("Test Bucket", "Test Description", 0m);
        Assert.True(result.Success);
        Assert.Equal(0m, result.Value!.DefaultLimit);
    }
    #endregion

    #region Update Tests
    [Fact]
    public void Update_ValidData_UpdatesProperties()
    {
        var bucket = Bucket.Create("Old Name", "Old Desc", 50m).Value!;
        bucket.Update("New Name", "New Desc", 200m);
        Assert.Equal("New Name", bucket.Name);
        Assert.Equal("New Desc", bucket.Description);
        Assert.Equal(200m, bucket.DefaultLimit);
    }

    [Fact]
    public void Update_WithInvalidName_DoesNotUpdate()
    {
        var bucket = Bucket.Create("Old Name", "Old Desc", 50m).Value!;
        bucket.Update("", "New Desc", 200m);
        Assert.Equal("Old Name", bucket.Name);
        Assert.Equal("Old Desc", bucket.Description);
        Assert.Equal(50m, bucket.DefaultLimit);
    }

    [Fact]
    public void Update_WithInvalidDescription_DoesNotUpdate()
    {
        var bucket = Bucket.Create("Old Name", "Old Desc", 50m).Value!;
        bucket.Update("New Name", null!, 200m);
        Assert.Equal("Old Name", bucket.Name);
        Assert.Equal("Old Desc", bucket.Description);
        Assert.Equal(50m, bucket.DefaultLimit);
    }

    [Fact]
    public void Update_WithNegativeLimit_DoesNotUpdate()
    {
        var bucket = Bucket.Create("Old Name", "Old Desc", 50m).Value!;
        bucket.Update("New Name", "New Desc", -100m);
        Assert.Equal("Old Name", bucket.Name);
        Assert.Equal("Old Desc", bucket.Description);
        Assert.Equal(50m, bucket.DefaultLimit);
    }
    #endregion

    #region UpdateDefaultBalance Tests
    [Fact]
    public void UpdateDefaultBalance_SetsNewBalance()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        bucket.UpdateDefaultBalance(50m);
        Assert.Equal(50m, bucket.DefaultBalance);
    }

    [Fact]
    public void UpdateDefaultBalance_WithNegativeValue_SetsBalance()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        bucket.UpdateDefaultBalance(-25m);
        Assert.Equal(-25m, bucket.DefaultBalance);
    }
    #endregion

    #region Enable/Disable Tests
    [Fact]
    public void Enable_SetsEnabledToTrue()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        bucket.Disable();
        bucket.Enable();
        Assert.True(bucket.Enabled);
    }

    [Fact]
    public void Disable_SetsEnabledToFalse()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        bucket.Disable();
        Assert.False(bucket.Enabled);
    }

    [Fact]
    public void Update_OnDisabledBucket_DoesNotUpdate()
    {
        var bucket = Bucket.Create("Old Name", "Old Desc", 50m).Value!;
        bucket.Disable();
        bucket.Update("New Name", "New Desc", 200m);
        Assert.Equal("Old Name", bucket.Name);
    }
    #endregion

    #region CreateMonthly Tests
    [Fact]
    public void CreateMonthly_CreatesMonthlyBucket()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var result = bucket.CreateMonthly(2025, 9);
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal((short)2025, result.Value!.Year);
        Assert.Equal((short)9, result.Value.Month);
        Assert.Equal(bucket, result.Value.Bucket);
    }
    #endregion
}

namespace zerobudget.core.domain.tests;

public class MonthlySpendingTests
{
    [Fact]
    public void Create_ValidMonthlySpending_ReturnsSuccess()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var result = Spending.Create(DateOnly.FromDateTime(DateTime.Now), "desc", 10m, "owner", Array.Empty<Tag>(), monthlyBucket);
        Assert.True(result.Success);
        Assert.Equal(10m, result.Value!.Amount);
        Assert.Equal("desc", result.Value.Description);
        Assert.Equal("owner", result.Value.Owner);
        Assert.Equal(monthlyBucket.Identity, result.Value.MonthlyBucketId);
    }

    [Fact]
    public void Update_ValidData_UpdatesProperties()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var spending = Spending.Create(DateOnly.FromDateTime(DateTime.Now), "desc", 10m, "owner", Array.Empty<Tag>(), monthlyBucket).Value!;
        spending.Update(DateOnly.FromDateTime(DateTime.Now), "newdesc", 20m, "newowner", Array.Empty<Tag>());
        Assert.Equal(20m, spending.Amount);
        Assert.Equal("newdesc", spending.Description);
        Assert.Equal("newowner", spending.Owner);
    }
}

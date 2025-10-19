namespace zerobudget.core.domain.tests;

public class MonthlySpendingTests
{
    #region Create Tests
    [Fact]
    public void Create_ValidMonthlySpending_ReturnsSuccess()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var result = MonthlySpending.Create(date, "desc", 10m, "owner", Array.Empty<string>(), monthlyBucket);
        Assert.True(result.Success);
        Assert.Equal(date, result.Value!.Date);
        Assert.Equal(10m, result.Value.Amount);
        Assert.Equal("desc", result.Value.Description);
        Assert.Equal("owner", result.Value.Owner);
        Assert.Equal(monthlyBucket.Identity, result.Value.MonthlyBucketId);
        Assert.Empty(result.Value.Tags);
    }

    [Fact]
    public void Create_WithTags_ReturnsSuccessWithTags()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var tags = new[] { "tag1", "tag2" };
        var result = MonthlySpending.Create(date, "desc", 10m, "owner", tags, monthlyBucket);
        Assert.True(result.Success);
        Assert.Equal(2, result.Value!.Tags.Length);
        Assert.Contains("tag1", result.Value.Tags);
        Assert.Contains("tag2", result.Value.Tags);
    }

    [Fact]
    public void Create_WithEmptyDescription_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var result = MonthlySpending.Create(date, "", 10m, "owner", Array.Empty<string>(), monthlyBucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithNullDescription_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var result = MonthlySpending.Create(date, null!, 10m, "owner", Array.Empty<string>(), monthlyBucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var result = MonthlySpending.Create(date, new string('a', 501), 10m, "owner", Array.Empty<string>(), monthlyBucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithNegativeAmount_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var result = MonthlySpending.Create(date, "desc", -1m, "owner", Array.Empty<string>(), monthlyBucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithZeroAmount_ReturnsSuccess()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var result = MonthlySpending.Create(date, "desc", 0m, "owner", Array.Empty<string>(), monthlyBucket);
        Assert.True(result.Success);
        Assert.Equal(0m, result.Value!.Amount);
    }

    [Fact]
    public void Create_WithEmptyOwner_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var result = MonthlySpending.Create(date, "desc", 10m, "", Array.Empty<string>(), monthlyBucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithNullOwner_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var result = MonthlySpending.Create(date, "desc", 10m, null!, Array.Empty<string>(), monthlyBucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithOwnerTooLong_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var result = MonthlySpending.Create(date, "desc", 10m, new string('a', 101), Array.Empty<string>(), monthlyBucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithMoreThanThreeTags_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var tags = new[] { "tag1", "tag2", "tag3", "tag4" };
        var result = MonthlySpending.Create(date, "desc", 10m, "owner", tags, monthlyBucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithThreeTags_ReturnsSuccess()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var tags = new[] { "tag1", "tag2", "tag3" };
        var result = MonthlySpending.Create(date, "desc", 10m, "owner", tags, monthlyBucket);
        Assert.True(result.Success);
        Assert.Equal(3, result.Value!.Tags.Length);
    }

    #endregion

    #region Update Tests
    [Fact]
    public void Update_ValidData_UpdatesProperties()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var spending = MonthlySpending.Create(date, "desc", 10m, "owner", Array.Empty<string>(), monthlyBucket).Value!;
        var newDate = new DateOnly(2025, 9, 20);
        spending.Update(newDate, "newdesc", 20m, "newowner", Array.Empty<string>());
        Assert.Equal(newDate, spending.Date);
        Assert.Equal(20m, spending.Amount);
        Assert.Equal("newdesc", spending.Description);
        Assert.Equal("newowner", spending.Owner);
    }

    [Fact]
    public void Update_WithInvalidDescription_DoesNotUpdate()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var spending = MonthlySpending.Create(date, "desc", 10m, "owner", Array.Empty<string>(), monthlyBucket).Value!;
        var newDate = new DateOnly(2025, 9, 20);
        spending.Update(newDate, "", 20m, "newowner", Array.Empty<string>());
        Assert.Equal("desc", spending.Description);
        Assert.Equal(date, spending.Date);
    }

    [Fact]
    public void Update_WithNegativeAmount_DoesNotUpdate()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var spending = MonthlySpending.Create(date, "desc", 10m, "owner", Array.Empty<string>(), monthlyBucket).Value!;
        var newDate = new DateOnly(2025, 9, 20);
        spending.Update(newDate, "newdesc", -20m, "newowner", Array.Empty<string>());
        Assert.Equal(10m, spending.Amount);
    }
    #endregion

    #region Equality Tests
    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var spending1 = MonthlySpending.Create(date, "desc", 10m, "owner", Array.Empty<string>(), monthlyBucket).Value!;
        var spending2 = MonthlySpending.Create(date, "desc", 10m, "owner", Array.Empty<string>(), monthlyBucket).Value!;
        Assert.True(spending1.Equals(spending2));
        Assert.True(spending1.Equals((object)spending2));
    }

    [Fact]
    public void Equals_DifferentDate_ReturnsFalse()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date1 = new DateOnly(2025, 9, 15);
        var date2 = new DateOnly(2025, 9, 20);
        var spending1 = MonthlySpending.Create(date1, "desc", 10m, "owner", Array.Empty<string>(), monthlyBucket).Value!;
        var spending2 = MonthlySpending.Create(date2, "desc", 10m, "owner", Array.Empty<string>(), monthlyBucket).Value!;
        Assert.False(spending1.Equals(spending2));
    }

    [Fact]
    public void Equals_DifferentDescription_ReturnsFalse()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var spending1 = MonthlySpending.Create(date, "desc1", 10m, "owner", Array.Empty<string>(), monthlyBucket).Value!;
        var spending2 = MonthlySpending.Create(date, "desc2", 10m, "owner", Array.Empty<string>(), monthlyBucket).Value!;
        Assert.False(spending1.Equals(spending2));
    }

    [Fact]
    public void Equals_DifferentAmount_ReturnsFalse()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var spending1 = MonthlySpending.Create(date, "desc", 10m, "owner", Array.Empty<string>(), monthlyBucket).Value!;
        var spending2 = MonthlySpending.Create(date, "desc", 20m, "owner", Array.Empty<string>(), monthlyBucket).Value!;
        Assert.False(spending1.Equals(spending2));
    }

    [Fact]
    public void Equals_DifferentOwner_ReturnsFalse()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var spending1 = MonthlySpending.Create(date, "desc", 10m, "owner1", Array.Empty<string>(), monthlyBucket).Value!;
        var spending2 = MonthlySpending.Create(date, "desc", 10m, "owner2", Array.Empty<string>(), monthlyBucket).Value!;
        Assert.False(spending1.Equals(spending2));
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var spending = MonthlySpending.Create(date, "desc", 10m, "owner", Array.Empty<string>(), monthlyBucket).Value!;
        Assert.False(spending.Equals(null));
        Assert.False(spending.Equals((object?)null));
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHash()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        var date = new DateOnly(2025, 9, 15);
        var spending1 = MonthlySpending.Create(date, "desc", 10m, "owner", Array.Empty<string>(), monthlyBucket).Value!;
        var spending2 = MonthlySpending.Create(date, "desc", 10m, "owner", Array.Empty<string>(), monthlyBucket).Value!;
        Assert.Equal(spending1.GetHashCode(), spending2.GetHashCode());
    }
    #endregion

    #region Validation Tests
    [Fact]
    public void Validate_WithValidData_ReturnsSuccess()
    {
        var date = new DateOnly(2025, 9, 15);
        var result = MonthlySpending.Validate(date, "desc", 10m, "owner", Array.Empty<string>());
        Assert.True(result.Success);
    }
    #endregion
}

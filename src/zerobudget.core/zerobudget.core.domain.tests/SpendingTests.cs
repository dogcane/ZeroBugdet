namespace zerobudget.core.domain.tests;

public class SpendingTests
{
    #region Create Tests
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
        Assert.True(result.Value.Enabled);
        Assert.Empty(result.Value.Tags);
    }

    [Fact]
    public void Create_WithTags_ReturnsSuccessWithTags()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var tag1 = Tag.Create("Tag1").Value!;
        var tag2 = Tag.Create("Tag2").Value!;
        var result = Spending.Create("desc", 10m, "owner", new[] { tag1, tag2 }, bucket);
        Assert.True(result.Success);
        Assert.Equal(2, result.Value!.Tags.Length);
        Assert.Contains("tag1", result.Value.Tags);
        Assert.Contains("tag2", result.Value.Tags);
    }

    [Fact]
    public void Create_WithEmptyDescription_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var result = Spending.Create("", 10m, "owner", Array.Empty<Tag>(), bucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithNullDescription_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var result = Spending.Create(null!, 10m, "owner", Array.Empty<Tag>(), bucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var result = Spending.Create(new string('a', 501), 10m, "owner", Array.Empty<Tag>(), bucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithNegativeAmount_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var result = Spending.Create("desc", -1m, "owner", Array.Empty<Tag>(), bucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithZeroAmount_ReturnsSuccess()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var result = Spending.Create("desc", 0m, "owner", Array.Empty<Tag>(), bucket);
        Assert.True(result.Success);
        Assert.Equal(0m, result.Value!.Amount);
    }

    [Fact]
    public void Create_WithEmptyOwner_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var result = Spending.Create("desc", 10m, "", Array.Empty<Tag>(), bucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithNullOwner_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var result = Spending.Create("desc", 10m, null!, Array.Empty<Tag>(), bucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithOwnerTooLong_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var result = Spending.Create("desc", 10m, new string('a', 101), Array.Empty<Tag>(), bucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithMoreThanThreeTags_ReturnsFailure()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var tag1 = Tag.Create("Tag1").Value!;
        var tag2 = Tag.Create("Tag2").Value!;
        var tag3 = Tag.Create("Tag3").Value!;
        var tag4 = Tag.Create("Tag4").Value!;
        var result = Spending.Create("desc", 10m, "owner", new[] { tag1, tag2, tag3, tag4 }, bucket);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Create_WithThreeTags_ReturnsSuccess()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var tag1 = Tag.Create("Tag1").Value!;
        var tag2 = Tag.Create("Tag2").Value!;
        var tag3 = Tag.Create("Tag3").Value!;
        var result = Spending.Create("desc", 10m, "owner", new[] { tag1, tag2, tag3 }, bucket);
        Assert.True(result.Success);
        Assert.Equal(3, result.Value!.Tags.Length);
    }

    [Fact]
    public void Create_WithNullBucket_ReturnsFailure()
    {
        var result = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), null!);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }
    #endregion

    #region Update Tests
    [Fact]
    public void Update_ValidData_UpdatesProperties()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var spending = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        var updateResult = spending.Update("newdesc", 20m, "newowner", Array.Empty<Tag>());
        Assert.True(updateResult.Success);
        Assert.Equal(20m, spending.Amount);
        Assert.Equal("newdesc", spending.Description);
        Assert.Equal("newowner", spending.Owner);
    }

    [Fact]
    public void Update_WithInvalidDescription_DoesNotUpdate()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var spending = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        var updateResult = spending.Update("", 20m, "newowner", Array.Empty<Tag>());
        Assert.False(updateResult.Success);
        Assert.Equal("desc", spending.Description);
    }

    [Fact]
    public void Update_WithNegativeAmount_DoesNotUpdate()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var spending = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        var updateResult = spending.Update("newdesc", -20m, "newowner", Array.Empty<Tag>());
        Assert.False(updateResult.Success);
        Assert.Equal(10m, spending.Amount);
    }

    [Fact]
    public void Update_OnDisabledSpending_DoesNotUpdate()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var spending = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        spending.Disable();
        var updateResult = spending.Update("newdesc", 20m, "newowner", Array.Empty<Tag>());
        Assert.False(updateResult.Success);
        Assert.Equal("desc", spending.Description);
    }
    #endregion

    #region Enable/Disable Tests
    [Fact]
    public void Enable_SetsEnabledToTrue()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var spending = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        var disableResult = spending.Disable();
        Assert.True(disableResult.Success);
        var enableResult = spending.Enable();
        Assert.True(enableResult.Success);
        Assert.True(spending.Enabled);
    }

    [Fact]
    public void Disable_SetsEnabledToFalse()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var spending = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        var result = spending.Disable();
        Assert.True(result.Success);
        Assert.False(spending.Enabled);
    }
    #endregion

    #region CreateMonthly Tests
    [Fact]
    public void CreateMonthly_CreatesMonthlySpending()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9).Value!;
        var spending = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        var result = spending.CreateMonthly(monthlyBucket);
        Assert.True(result.Success);
        var monthlySpending = result.Value!;
        Assert.NotNull(monthlySpending);
        Assert.Equal(new DateOnly(2025, 9, 1), monthlySpending.Date);
        Assert.Equal("desc", monthlySpending.Description);
        Assert.Equal(10m, monthlySpending.Amount);
        Assert.Equal("owner", monthlySpending.Owner);
        Assert.Equal(monthlyBucket.Identity, monthlySpending.MonthlyBucketId);
    }

    [Fact]
    public void CreateMonthly_WithTags_PreservesTags()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9).Value!;
        var tag1 = Tag.Create("Tag1").Value!;
        var tag2 = Tag.Create("Tag2").Value!;
        var spending = Spending.Create("desc", 10m, "owner", new[] { tag1, tag2 }, bucket).Value!;
        var result = spending.CreateMonthly(monthlyBucket);
        Assert.True(result.Success);
        var monthlySpending = result.Value!;
        Assert.Equal(2, monthlySpending.Tags.Length);
        Assert.Contains("tag1", monthlySpending.Tags);
        Assert.Contains("tag2", monthlySpending.Tags);
    }
    #endregion

    #region Equality Tests
    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var spending1 = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        Assert.True(spending1.Equals(spending2));
        Assert.True(spending1.Equals((object)spending2));
    }

    [Fact]
    public void Equals_DifferentDescription_ReturnsFalse()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var spending1 = Spending.Create("desc1", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("desc2", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        Assert.False(spending1.Equals(spending2));
    }

    [Fact]
    public void Equals_DifferentAmount_ReturnsFalse()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var spending1 = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("desc", 20m, "owner", Array.Empty<Tag>(), bucket).Value!;
        Assert.False(spending1.Equals(spending2));
    }

    [Fact]
    public void Equals_DifferentOwner_ReturnsFalse()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var spending1 = Spending.Create("desc", 10m, "owner1", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("desc", 10m, "owner2", Array.Empty<Tag>(), bucket).Value!;
        Assert.False(spending1.Equals(spending2));
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var spending = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        Assert.False(spending.Equals(null));
        Assert.False(spending.Equals((object?)null));
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHash()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var spending1 = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        Assert.Equal(spending1.GetHashCode(), spending2.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var spending = Spending.Create("desc", 10m, "owner", Array.Empty<Tag>(), bucket).Value!;
        Assert.Equal("desc : 10 (owner)", spending.ToString());
    }
    #endregion
}

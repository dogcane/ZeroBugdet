using System;
using Xunit;
using zerobudget.core.domain;
using Resulz;

namespace zerobudget.core.domain.tests;

public class MonthlyBucketTests
{
    #region Constructor Tests
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var result = bucket.CreateMonthly(2025, 9);
        Assert.True(result.Success);
        var monthlyBucket = result.Value!;
        Assert.Equal((short)2025, monthlyBucket.Year);
        Assert.Equal((short)9, monthlyBucket.Month);
        Assert.Equal(bucket.Identity, monthlyBucket.BucketId);
        Assert.Equal(bucket.Description, monthlyBucket.Description);
        Assert.Equal(bucket.DefaultLimit, monthlyBucket.Limit);
        Assert.Equal(0m, monthlyBucket.Balance);
    }

    [Fact]
    public void Constructor_WithJanuaryMonth_SetsPropertiesCorrectly()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var result = bucket.CreateMonthly(2025, 1);
        Assert.True(result.Success);
        var monthlyBucket = result.Value!;
        Assert.Equal((short)2025, monthlyBucket.Year);
        Assert.Equal((short)1, monthlyBucket.Month);
    }

    [Fact]
    public void Constructor_WithDecemberMonth_SetsPropertiesCorrectly()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var result = bucket.CreateMonthly(2025, 12);
        Assert.True(result.Success);
        var monthlyBucket = result.Value!;
        Assert.Equal((short)2025, monthlyBucket.Year);
        Assert.Equal((short)12, monthlyBucket.Month);
    }
    #endregion

    #region UpdateBalance Tests
    [Fact]
    public void UpdateBalance_SetsNewBalance()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9).Value!;
        var result = monthlyBucket.UpdateBalance(75m);
        Assert.True(result.Success);
        Assert.Equal(75m, monthlyBucket.Balance);
    }

    [Fact]
    public void UpdateBalance_WithNegativeValue_SetsBalance()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9).Value!;
        var result = monthlyBucket.UpdateBalance(-50m);
        Assert.True(result.Success);
        Assert.Equal(-50m, monthlyBucket.Balance);
    }

    [Fact]
    public void UpdateBalance_WithZero_SetsBalance()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9).Value!;
        var result = monthlyBucket.UpdateBalance(0m);
        Assert.True(result.Success);
        Assert.Equal(0m, monthlyBucket.Balance);
    }
    #endregion

    #region Validation Tests
    [Fact]
    public void Validate_WithValidData_ReturnsSuccess()
    {
        var result = MonthlyBucket.Validate(2025, 9, 100m, 1);
        Assert.True(result.Success);
    }

    [Fact]
    public void Validate_WithYearBefore2000_ReturnsFailure()
    {
        var result = MonthlyBucket.Validate(1999, 9, 100m, 1);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Validate_WithYear2000_ReturnsSuccess()
    {
        var result = MonthlyBucket.Validate(2000, 9, 100m, 1);
        Assert.True(result.Success);
    }

    [Fact]
    public void Validate_WithMonthZero_ReturnsFailure()
    {
        var result = MonthlyBucket.Validate(2025, 0, 100m, 1);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Validate_WithMonthThirteen_ReturnsFailure()
    {
        var result = MonthlyBucket.Validate(2025, 13, 100m, 1);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Validate_WithInvalidBucketId_ReturnsFailure()
    {
        var result = MonthlyBucket.Validate(2025, 9, 100m, 0);
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }
    #endregion
}

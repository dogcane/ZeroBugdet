using System;
using Xunit;
using zerobudget.core.domain;
using Resulz;

namespace zerobudget.core.domain.tests;

public class MonthlyBucketTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var bucket = Bucket.Create("Test", "Desc", 100m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2025, 9);
        Assert.Equal((short)2025, monthlyBucket.Year);
        Assert.Equal((short)9, monthlyBucket.Month);
        Assert.Equal(bucket, monthlyBucket.Bucket);
        Assert.Equal(bucket.Description, monthlyBucket.Description);
        Assert.Equal(bucket.DefaultLimit, monthlyBucket.Limit);
    }
}

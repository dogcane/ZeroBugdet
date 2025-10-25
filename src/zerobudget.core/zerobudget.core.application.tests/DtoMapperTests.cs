using Xunit;
using zerobudget.core.application.Mappers;
using zerobudget.core.domain;

namespace zerobudget.core.application.tests;

public class DtoMapperTests
{
    #region BucketMapper Tests
    [Fact]
    public void BucketMapper_ToDto_MapsAllProperties()
    {
        // Arrange
        var mapper = new BucketMapper();
        var bucket = Bucket.Create("Test Bucket", "Test Description", 1000m).Value!;
        bucket.UpdateDefaultBalance(100m);
        
        // Act
        var dto = mapper.ToDto(bucket);
        
        // Assert
        Assert.Equal(bucket.Identity, dto.Id);
        Assert.Equal("Test Bucket", dto.Name);
        Assert.Equal("Test Description", dto.Description);
        Assert.Equal(1000m, dto.DefaultLimit);
        Assert.Equal(100m, dto.DefaultBalance);
        Assert.True(dto.Enabled);
    }
    
    [Fact]
    public void BucketMapper_ToDto_WithDisabledBucket_MapsCorrectly()
    {
        // Arrange
        var mapper = new BucketMapper();
        var bucket = Bucket.Create("Test Bucket", "Test Description", 1000m).Value!;
        bucket.Disable();
        
        // Act
        var dto = mapper.ToDto(bucket);
        
        // Assert
        Assert.False(dto.Enabled);
    }
    #endregion
    
    #region SpendingMapper Tests
    [Fact]
    public void SpendingMapper_ToDto_MapsAllProperties()
    {
        // Arrange
        var mapper = new SpendingMapper();
        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var tag1 = Tag.Create("Tag1").Value!;
        var tag2 = Tag.Create("Tag2").Value!;
        var spending = Spending.Create("Test Spending", 100m, "Owner", new[] { tag1, tag2 }, bucket).Value!;
        
        // Act
        var dto = mapper.ToDto(spending);
        
        // Assert
        Assert.Equal(spending.Identity, dto.Id);
        Assert.Equal(bucket.Identity, dto.BucketId);
        Assert.Equal("Test Spending", dto.Description);
        Assert.Equal(100m, dto.Amount);
        Assert.Equal("Owner", dto.Owner);
        Assert.Equal(2, dto.Tags.Length);
        Assert.Contains("tag1", dto.Tags);
        Assert.Contains("tag2", dto.Tags);
        Assert.True(dto.Enabled);
    }
    
    [Fact]
    public void SpendingMapper_ToDto_WithNoTags_MapsCorrectly()
    {
        // Arrange
        var mapper = new SpendingMapper();
        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var spending = Spending.Create("Test Spending", 100m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        
        // Act
        var dto = mapper.ToDto(spending);
        
        // Assert
        Assert.Empty(dto.Tags);
    }
    
    [Fact]
    public void SpendingMapper_ToDto_WithDisabledSpending_MapsCorrectly()
    {
        // Arrange
        var mapper = new SpendingMapper();
        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var spending = Spending.Create("Test Spending", 100m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        spending.Disable();
        
        // Act
        var dto = mapper.ToDto(spending);
        
        // Assert
        Assert.False(dto.Enabled);
    }
    #endregion
    
    #region MonthlyBucketMapper Tests
    [Fact]
    public void MonthlyBucketMapper_ToDto_MapsAllProperties()
    {
        // Arrange
        var mapper = new MonthlyBucketMapper();
        var bucket = Bucket.Create("Test Bucket", "Test Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2024, 10).Value!;
        
        // Act
        var dto = mapper.ToDto(monthlyBucket);
        
        // Assert
        Assert.Equal(monthlyBucket.Identity, dto.Id);
        Assert.Equal(2024, dto.Year);
        Assert.Equal(10, dto.Month);
        Assert.Equal(monthlyBucket.Balance, dto.Balance);
        Assert.Equal(monthlyBucket.Description, dto.Description);
        Assert.Equal(monthlyBucket.Limit, dto.Limit);
        Assert.Equal(bucket.Identity, dto.BucketId);
    }
    
    [Fact]
    public void MonthlyBucketMapper_ToDto_WithNullBucket_HandlesGracefully()
    {
        // Arrange
        var mapper = new MonthlyBucketMapper();
        var bucket = Bucket.Create("Test Bucket", "Test Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2024, 10).Value!;
        
        // Act
        var dto = mapper.ToDto(monthlyBucket);
        
        // Assert
        Assert.NotNull(dto);
        Assert.Equal(bucket.Identity, dto.BucketId);
    }
    #endregion
    
    #region MonthlySpendingMapper Tests
    [Fact]
    public void MonthlySpendingMapper_ToDto_MapsAllProperties()
    {
        // Arrange
        var mapper = new MonthlySpendingMapper();
        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2024, 10).Value!;
        var tag1 = Tag.Create("Tag1").Value!;
        var tag2 = Tag.Create("Tag2").Value!;
        var spending = Spending.Create("Test Spending", 100m, "Owner", new[] { tag1, tag2 }, bucket).Value!;
        var monthlySpending = spending.CreateMonthly(monthlyBucket).Value!;
        
        // Act
        var dto = mapper.ToDto(monthlySpending);
        
        // Assert
        Assert.Equal(monthlySpending.Identity, dto.Id);
        Assert.Equal(new DateOnly(2024, 10, 1), dto.Date);
        Assert.Equal(monthlyBucket.Identity, dto.MonthlyBucketId);
        Assert.Equal("Test Spending", dto.Description);
        Assert.Equal(100m, dto.Amount);
        Assert.Equal("Owner", dto.Owner);
        Assert.Equal(2, dto.Tags.Length);
        Assert.Contains("tag1", dto.Tags);
        Assert.Contains("tag2", dto.Tags);
    }
    
    [Fact]
    public void MonthlySpendingMapper_ToDto_WithNoTags_MapsCorrectly()
    {
        // Arrange
        var mapper = new MonthlySpendingMapper();
        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2024, 10).Value!;
        var spending = Spending.Create("Test Spending", 100m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        var monthlySpending = spending.CreateMonthly(monthlyBucket).Value!;
        
        // Act
        var dto = mapper.ToDto(monthlySpending);
        
        // Assert
        Assert.Empty(dto.Tags);
    }
    #endregion
    
    #region TagMapper Tests
    [Fact]
    public void TagMapper_ToDto_MapsAllProperties()
    {
        // Arrange
        var mapper = new TagMapper();
        var tag = Tag.Create("TestTag").Value!;
        
        // Act
        var dto = mapper.ToDto(tag);
        
        // Assert
        Assert.Equal(tag.Identity, dto.Id);
        Assert.Equal("testtag", dto.Name);
    }
    
    [Fact]
    public void TagMapper_ToDto_WithLongerName_MapsCorrectly()
    {
        // Arrange
        var mapper = new TagMapper();
        var tag = Tag.Create("TestTag123").Value!;
        
        // Act
        var dto = mapper.ToDto(tag);
        
        // Assert
        Assert.Equal("testtag123", dto.Name);
    }
    #endregion
}

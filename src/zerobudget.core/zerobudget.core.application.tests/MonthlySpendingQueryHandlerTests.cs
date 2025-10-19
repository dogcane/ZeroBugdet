using ECO.Data;
using Moq;
using Xunit;
using zerobudget.core.application.Handlers.Queries;
using zerobudget.core.application.Queries;
using zerobudget.core.domain;

namespace zerobudget.core.application.tests;

public class MonthlySpendingQueryHandlerTests
{
    [Fact]
    public async Task Handle_GetMonthlySpendingByIdQuery_ReturnsMonthlySpending()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new MonthlySpendingQueryHandlers(monthlySpendingRepository.Object);
        
        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2024, 10);
        var spending = Spending.Create("Test Spending", 100m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        var monthlySpending = spending.CreateMonthly(monthlyBucket);
        
        monthlySpendingRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync(monthlySpending);
        
        var query = new GetMonthlySpendingByIdQuery(1);
        
        // Act
        var result = await handler.Handle(query);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Spending", result.Description);
        Assert.Equal(100m, result.Amount);
    }
    
    [Fact]
    public async Task Handle_GetMonthlySpendingByIdQuery_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new MonthlySpendingQueryHandlers(monthlySpendingRepository.Object);
        
        monthlySpendingRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync((MonthlySpending?)null);
        
        var query = new GetMonthlySpendingByIdQuery(999);
        
        // Act
        var result = await handler.Handle(query);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task Handle_GetAllMonthlySpendingsQuery_ReturnsAllMonthlySpendings()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new MonthlySpendingQueryHandlers(monthlySpendingRepository.Object);
        
        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2024, 10);
        var spending1 = Spending.Create("Spending1", 100m, "Owner1", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("Spending2", 200m, "Owner2", Array.Empty<Tag>(), bucket).Value!;
        var monthlySpending1 = spending1.CreateMonthly(monthlyBucket);
        var monthlySpending2 = spending2.CreateMonthly(monthlyBucket);
        
        monthlySpendingRepository
            .Setup(r => r.AsQueryable())
            .Returns(new[] { monthlySpending1, monthlySpending2 }.AsQueryable());
        
        var query = new GetAllMonthlySpendingsQuery();
        
        // Act
        var result = await handler.Handle(query);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    
    [Fact]
    public async Task Handle_GetMonthlySpendingsByMonthlyBucketIdQuery_ReturnsFilteredMonthlySpendings()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new MonthlySpendingQueryHandlers(monthlySpendingRepository.Object);
        
        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var monthlyBucket1 = bucket.CreateMonthly(2024, 10);
        var monthlyBucket2 = bucket.CreateMonthly(2024, 11);
        var spending1 = Spending.Create("Spending1", 100m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("Spending2", 200m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        var monthlySpending1 = spending1.CreateMonthly(monthlyBucket1);
        var monthlySpending2 = spending2.CreateMonthly(monthlyBucket2);
        
        monthlySpendingRepository
            .Setup(r => r.AsQueryable())
            .Returns(new[] { monthlySpending1, monthlySpending2 }.AsQueryable());
        
        var query = new GetMonthlySpendingsByMonthlyBucketIdQuery(monthlyBucket1.Identity);
        
        // Act
        var result = await handler.Handle(query);
        
        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(monthlyBucket1.Identity, result.First().MonthlyBucketId);
    }
    
    [Fact]
    public async Task Handle_GetMonthlySpendingsByDateRangeQuery_ReturnsFilteredMonthlySpendings()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new MonthlySpendingQueryHandlers(monthlySpendingRepository.Object);
        
        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var monthlyBucket1 = bucket.CreateMonthly(2024, 10);
        var monthlyBucket2 = bucket.CreateMonthly(2024, 11);
        var spending1 = Spending.Create("Spending1", 100m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("Spending2", 200m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        var monthlySpending1 = spending1.CreateMonthly(monthlyBucket1);
        var monthlySpending2 = spending2.CreateMonthly(monthlyBucket2);
        
        monthlySpendingRepository
            .Setup(r => r.AsQueryable())
            .Returns(new[] { monthlySpending1, monthlySpending2 }.AsQueryable());
        
        var query = new GetMonthlySpendingsByDateRangeQuery(
            new DateOnly(2024, 10, 1), 
            new DateOnly(2024, 10, 31));
        
        // Act
        var result = await handler.Handle(query);
        
        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(new DateOnly(2024, 10, 1), result.First().Date);
    }
    
    [Fact]
    public async Task Handle_GetMonthlySpendingsByOwnerQuery_ReturnsFilteredMonthlySpendings()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var handler = new MonthlySpendingQueryHandlers(monthlySpendingRepository.Object);
        
        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2024, 10);
        var spending1 = Spending.Create("Spending1", 100m, "Owner1", Array.Empty<Tag>(), bucket).Value!;
        var spending2 = Spending.Create("Spending2", 200m, "Owner2", Array.Empty<Tag>(), bucket).Value!;
        var monthlySpending1 = spending1.CreateMonthly(monthlyBucket);
        var monthlySpending2 = spending2.CreateMonthly(monthlyBucket);
        
        monthlySpendingRepository
            .Setup(r => r.AsQueryable())
            .Returns(new[] { monthlySpending1, monthlySpending2 }.AsQueryable());
        
        var query = new GetMonthlySpendingsByOwnerQuery("Owner1");
        
        // Act
        var result = await handler.Handle(query);
        
        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Owner1", result.First().Owner);
    }
}

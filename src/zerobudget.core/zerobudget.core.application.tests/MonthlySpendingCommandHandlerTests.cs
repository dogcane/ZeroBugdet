using ECO.Data;
using Moq;
using Xunit;
using zerobudget.core.application.Commands;
using zerobudget.core.application.Handlers.Commands;
using zerobudget.core.domain;

namespace zerobudget.core.application.tests;

public class MonthlySpendingCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreateMonthlySpendingCommand_ShouldCreateMonthlySpending()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var tagService = new Mock<ITagService>();
        var handler = new MonthlySpendingCommandHandlers(
            monthlySpendingRepository.Object, 
            monthlyBucketRepository.Object,
            tagService.Object);
        
        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2024, 10);
        
        monthlyBucketRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync(monthlyBucket);
        
        tagService
            .Setup(s => s.EnsureTagsByNameAsync(It.IsAny<string[]>()))
            .ReturnsAsync(new List<Tag>());
        
        monthlySpendingRepository.Setup(r => r.AddAsync(It.IsAny<MonthlySpending>()))
                                  .Returns(Task.CompletedTask);
        
        var command = new CreateMonthlySpendingCommand(
            new DateOnly(2024, 10, 15),
            monthlyBucket.Identity,
            "Test Spending",
            100m,
            "Owner",
            Array.Empty<string>());
        
        // Act
        var result = await handler.Handle(command);
        
        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Spending", result.Value.Description);
        Assert.Equal(100m, result.Value.Amount);
        monthlySpendingRepository.Verify(r => r.AddAsync(It.IsAny<MonthlySpending>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_CreateMonthlySpendingCommand_WithNonExistentMonthlyBucket_ShouldReturnFailure()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var tagService = new Mock<ITagService>();
        var handler = new MonthlySpendingCommandHandlers(
            monthlySpendingRepository.Object, 
            monthlyBucketRepository.Object,
            tagService.Object);
        
        monthlyBucketRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync((MonthlyBucket?)null);
        
        var command = new CreateMonthlySpendingCommand(
            new DateOnly(2024, 10, 15),
            999,
            "Test Spending",
            100m,
            "Owner",
            Array.Empty<string>());
        
        // Act
        var result = await handler.Handle(command);
        
        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        monthlySpendingRepository.Verify(r => r.AddAsync(It.IsAny<MonthlySpending>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_UpdateMonthlySpendingCommand_ShouldUpdateExistingMonthlySpending()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var tagService = new Mock<ITagService>();
        var handler = new MonthlySpendingCommandHandlers(
            monthlySpendingRepository.Object, 
            monthlyBucketRepository.Object,
            tagService.Object);
        
        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2024, 10);
        var spending = Spending.Create("Original", 50m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        var monthlySpending = spending.CreateMonthly(monthlyBucket);
        
        monthlySpendingRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync(monthlySpending);
        
        tagService
            .Setup(s => s.EnsureTagsByNameAsync(It.IsAny<string[]>()))
            .ReturnsAsync(new List<Tag>());
        
        monthlySpendingRepository
            .Setup(r => r.UpdateAsync(It.IsAny<MonthlySpending>()))
            .Returns(Task.CompletedTask);
        
        var command = new UpdateMonthlySpendingCommand(
            1,
            new DateOnly(2024, 10, 15),
            "Updated Spending",
            150m,
            "NewOwner",
            Array.Empty<string>());
        
        // Act
        var result = await handler.Handle(command);
        
        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Updated Spending", result.Value.Description);
        Assert.Equal(150m, result.Value.Amount);
        monthlySpendingRepository.Verify(r => r.UpdateAsync(It.IsAny<MonthlySpending>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_DeleteMonthlySpendingCommand_ShouldDeleteExistingMonthlySpending()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var tagService = new Mock<ITagService>();
        var handler = new MonthlySpendingCommandHandlers(
            monthlySpendingRepository.Object, 
            monthlyBucketRepository.Object,
            tagService.Object);
        
        var bucket = Bucket.Create("Test", "Description", 1000m).Value!;
        var monthlyBucket = bucket.CreateMonthly(2024, 10);
        var spending = Spending.Create("Test", 100m, "Owner", Array.Empty<Tag>(), bucket).Value!;
        var monthlySpending = spending.CreateMonthly(monthlyBucket);
        
        monthlySpendingRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync(monthlySpending);
        
        monthlySpendingRepository.Setup(r => r.RemoveAsync(It.IsAny<MonthlySpending>()))
                                  .Returns(Task.CompletedTask);
        
        var command = new DeleteMonthlySpendingCommand(1);
        
        // Act
        var result = await handler.Handle(command);
        
        // Assert
        Assert.True(result.Success);
        monthlySpendingRepository.Verify(r => r.RemoveAsync(It.IsAny<MonthlySpending>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_DeleteMonthlySpendingCommand_WithNonExistentMonthlySpending_ShouldReturnFailure()
    {
        // Arrange
        var monthlySpendingRepository = new Mock<IMonthlySpendingRepository>();
        var monthlyBucketRepository = new Mock<IMonthlyBucketRepository>();
        var tagService = new Mock<ITagService>();
        var handler = new MonthlySpendingCommandHandlers(
            monthlySpendingRepository.Object, 
            monthlyBucketRepository.Object,
            tagService.Object);
        
        monthlySpendingRepository
            .Setup(r => r.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync((MonthlySpending?)null);
        
        var command = new DeleteMonthlySpendingCommand(999);
        
        // Act
        var result = await handler.Handle(command);
        
        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        monthlySpendingRepository.Verify(r => r.RemoveAsync(It.IsAny<MonthlySpending>()), Times.Never);
    }
}

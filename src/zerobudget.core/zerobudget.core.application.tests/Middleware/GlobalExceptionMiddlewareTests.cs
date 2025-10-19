using Microsoft.Extensions.Logging;
using Moq;
using Resulz;
using Wolverine;
using Xunit;
using zerobudget.core.application.Middleware;

namespace zerobudget.core.application.tests.Middleware;

/// <summary>
/// Unit tests for the GlobalExceptionMiddleware
/// </summary>
public class GlobalExceptionMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionMiddleware>> _mockLogger;
    private readonly Mock<IMessageContext> _mockContext;
    private readonly Mock<Envelope> _mockEnvelope;
    private readonly GlobalExceptionMiddleware _middleware;

    public GlobalExceptionMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
        _mockContext = new Mock<IMessageContext>();
        _mockEnvelope = new Mock<Envelope>();
        _middleware = new GlobalExceptionMiddleware(_mockLogger.Object);

        // Setup mock context
        _mockEnvelope.Setup(e => e.Message).Returns(new TestCommand());
        _mockContext.Setup(c => c.Envelope).Returns(_mockEnvelope.Object);
    }

    [Fact]
    public async Task Before_WhenHandlerSucceeds_ShouldReturnResult()
    {
        // Arrange
        var expectedResult = OperationResult<string>.MakeSuccess("test result");
        Func<Task<OperationResult<string>>> handler = () => Task.FromResult(expectedResult);

        // Act
        var result = await _middleware.Before(_mockContext.Object, handler);

        // Assert
        Assert.Equal(expectedResult, result);
        Assert.True(result.Success);
        Assert.Equal("test result", result.Value);
    }

    [Fact]
    public async Task Before_WhenHandlerThrowsException_ForOperationResultT_ShouldReturnFailure()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        Func<Task<OperationResult<string>>> handler = () => throw expectedException;

        // Act
        var result = await _middleware.Before(_mockContext.Object, handler);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        var errorString = result.Errors.First().ToString();
        Assert.Contains("An unexpected error occurred while processing TestCommand", errorString);
        Assert.Contains("Test exception", errorString);

        // Verify logging
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unhandled exception in handler for message type: TestCommand")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Before_WhenHandlerThrowsException_ForOperationResult_ShouldReturnFailure()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        Func<Task<OperationResult>> handler = () => throw expectedException;

        // Act
        var result = await _middleware.Before(_mockContext.Object, handler);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        var errorString = result.Errors.First().ToString();
        Assert.Contains("An unexpected error occurred while processing TestCommand", errorString);
        Assert.Contains("Test exception", errorString);
    }

    [Fact]
    public async Task Before_WhenHandlerThrowsException_ForNullableReference_ShouldReturnNull()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        Func<Task<string?>> handler = () => throw expectedException;

        // Act
        var result = await _middleware.Before(_mockContext.Object, handler);

        // Assert
        Assert.Null(result);

        // Verify logging
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unhandled exception in handler for message type: TestCommand")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Before_WhenHandlerThrowsException_ForEnumerable_ShouldReturnEmptyCollection()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        Func<Task<IEnumerable<string>>> handler = () => throw expectedException;

        // Act
        var result = await _middleware.Before(_mockContext.Object, handler);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Before_WhenHandlerThrowsException_ForValueType_ShouldThrowApplicationException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        Func<Task<int>> handler = () => throw expectedException;

        // Act & Assert
        var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _middleware.Before(_mockContext.Object, handler));

        Assert.Contains("An unexpected error occurred while processing TestCommand", thrownException.Message);
        Assert.Contains("Test exception", thrownException.Message);
        Assert.Equal(expectedException, thrownException.InnerException);
    }

    // Test command class for mock context
    private class TestCommand
    {
    }
}

using Api.ExceptionHandlers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ApiUnitTests.ExceptionHandlers;

public class GlobalExceptionHandlerTests
{
    private readonly Mock<IProblemDetailsService> _problemDetailsServiceMock;
    private readonly GlobalExceptionHandler _handler;
    private readonly DefaultHttpContext _httpContext;

    public GlobalExceptionHandlerTests()
    {
        _problemDetailsServiceMock = new Mock<IProblemDetailsService>();
        _handler = new GlobalExceptionHandler(_problemDetailsServiceMock.Object);
        _httpContext = new DefaultHttpContext();
    }
    
    [Fact]
    public async Task TryHandleAsync_CustomException_ShouldSetCorrectType()
    {
        // Arrange
        var customException = new InvalidOperationException("Operation is not valid");

        // Act
        await _handler.TryHandleAsync(_httpContext, customException, CancellationToken.None);

        // Assert
        _problemDetailsServiceMock.Verify(x => x.WriteAsync(It.Is<ProblemDetailsContext>(
            ctx => ctx.ProblemDetails.Type == nameof(InvalidOperationException)
        )), Times.Once);
    }
    
    [Fact]
    public async Task TryHandleAsync_ShouldAlwaysReturnTrue()
    {
        // Arrange
        var exception = new ApplicationException("Exception");

        // Act
        var result = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }
}
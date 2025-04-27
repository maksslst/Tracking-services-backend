using Api.ExceptionHandlers;
using Application.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ApiUnitTests.ExceptionHandlers;

public class ApplicationExceptionHandlerTests
{
    private readonly Mock<IProblemDetailsService> _problemDetailsServiceMock;
    private readonly ApplicationExceptionHandler _handler;
    private readonly DefaultHttpContext _httpContext;

    public ApplicationExceptionHandlerTests()
    {
        _problemDetailsServiceMock = new Mock<IProblemDetailsService>();
        _handler = new ApplicationExceptionHandler(_problemDetailsServiceMock.Object);
        _httpContext = new DefaultHttpContext();
    }
    
    [Fact]
    public async Task TryHandleAsync_ExceptionIsBaseApplicationException_ReturnsTrueAndWritesProblemDetails()
    {
        // Arrange
        var exception = new NotFoundApplicationException("Task not found");

        // Act
        var result = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _httpContext.Response.StatusCode.Should().Be((int)exception.StatusCode);
        _httpContext.Response.ContentType.Should().Be("application/problem+json");

        _problemDetailsServiceMock.Verify(x =>
                x.WriteAsync(It.Is<ProblemDetailsContext>(ctx =>
                    ctx.ProblemDetails.Title == exception.Title &&
                    ctx.ProblemDetails.Detail == exception.Message &&
                    ctx.ProblemDetails.Status == (int)exception.StatusCode &&
                    ctx.ProblemDetails.Type == exception.GetType().Name &&
                    ctx.ProblemDetails.Instance == _httpContext.Request.Path
                )),
            Times.Once);
    }
    
    [Fact]
    public async Task TryHandleAsync_ExceptionIsNotBaseApplicationException_ReturnsFalseAndDoesNotWriteProblemDetails()
    {
        // Arrange
        var exception = new InvalidOperationException("Error");

        // Act
        var result = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _problemDetailsServiceMock.Verify(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Never);
    }
    
    [Fact]
    public async Task TryHandleAsync_ResponseStatusCodeAlreadySetAndExceptionIsNotHandled_ReturnResponseUnchanged()
    {
        // Arrange
        _httpContext.Response.StatusCode = StatusCodes.Status418ImATeapot;
        var exception = new Exception("error");

        // Act
        var result = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status418ImATeapot);
        _httpContext.Response.ContentType.Should().BeNull();
        _problemDetailsServiceMock.Verify(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Never);
    }
}
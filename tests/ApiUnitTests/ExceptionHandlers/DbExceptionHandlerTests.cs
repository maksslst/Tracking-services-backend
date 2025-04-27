using System.Data.Common;
using Api.ExceptionHandlers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ApiUnitTests.ExceptionHandlers;

public class DbExceptionHandlerTests
{
    private readonly Mock<IProblemDetailsService> _problemDetailsServiceMock;
    private readonly DbExceptionHandler _handler;
    private readonly DefaultHttpContext _httpContext;

    public DbExceptionHandlerTests()
    {
        _problemDetailsServiceMock = new Mock<IProblemDetailsService>();
        _handler = new DbExceptionHandler(_problemDetailsServiceMock.Object);
        _httpContext = new DefaultHttpContext();
    }

    [Fact]
    public async Task TryHandleAsync_DbExceptionProvided_ShouldReturnTrue()
    {
        // Arrange
        var exception = new Mock<DbException>();
        exception.Setup(e => e.Message).Returns("Database connection failed");

        // Act
        var result = await _handler.TryHandleAsync(_httpContext, exception.Object, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, _httpContext.Response.StatusCode);
        Assert.Equal("application/problem+json", _httpContext.Response.ContentType);
        _problemDetailsServiceMock.Verify(p => p.WriteAsync(It.Is<ProblemDetailsContext>(
            ctx => ctx.ProblemDetails.Status == StatusCodes.Status500InternalServerError &&
                   ctx.ProblemDetails.Title == "Database error" &&
                   ctx.ProblemDetails.Detail == "Database connection failed" &&
                   ctx.Exception == exception.Object &&
                   ctx.HttpContext == _httpContext
        )), Times.Once);
    }
    
    [Fact]
    public async Task TryHandleAsync_ExceptionIsNotDbException_ShouldReturnFalse()
    {
        // Arrange
        var exception = new InvalidOperationException("Error");

        // Act
        var result = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        _httpContext.Response.ContentType.Should().BeNull();
        _problemDetailsServiceMock.Verify(p => p.WriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Never);
    }
    
    [Fact]
    public async Task TryHandleAsync_DbException_ResponseShouldContainCorrectPath()
    {
        // Arrange
        _httpContext.Request.Path = "/api/test";
        var exception = new Mock<DbException>();
        exception.Setup(e => e.Message).Returns("Test exception");

        // Act
        var result = await _handler.TryHandleAsync(_httpContext, exception.Object, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _problemDetailsServiceMock.Verify(p => p.WriteAsync(It.Is<ProblemDetailsContext>(
            ctx => ctx.ProblemDetails.Instance == "/api/test"
        )), Times.Once);
    }
}
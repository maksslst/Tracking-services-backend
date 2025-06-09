using System.Security.Claims;
using Api.Controllers;
using Application.Requests;
using Application.Responses;
using Application.Services;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;
    private readonly Faker _faker;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
        _faker = new Faker();
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsCreated()
    {
        // Arrange
        string password = _faker.Random.Word();
        var request = new RegistrationRequest
        {
            Username = _faker.Person.UserName,
            Email = _faker.Person.Email,
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            Password = password,
            PasswordConfirmation = password
        };
        var principal = new ClaimsPrincipal();
        _authServiceMock.Setup(s => s.Register(request)).ReturnsAsync(principal);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        var createdResult = result as CreatedResult;
        createdResult?.StatusCode.Should().Be(201);
        _authServiceMock.Verify(s => s.Register(request), Times.Once());
    }

    [Fact]
    public async Task Login_ValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = _faker.Person.UserName,
            Password = _faker.Random.String(10)
        };

        var principal = new ClaimsPrincipal();
        _authServiceMock.Setup(s => s.Login(request)).ReturnsAsync(principal);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.StatusCode.Should().Be(200);
        _authServiceMock.Verify(s => s.Login(request), Times.Once());
    }

    [Fact]
    public async Task Login_UnauthorizedAccessException_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = _faker.Person.UserName,
            Password = _faker.Random.Word()
        };
        _authServiceMock.Setup(s => s.Login(request))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid login attempt"));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.Login(request));
    }
}
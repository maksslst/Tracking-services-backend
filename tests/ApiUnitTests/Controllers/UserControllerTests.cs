using Api.Controllers;
using Application.Requests;
using Application.Responses;
using Application.Services;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _controller;
    private readonly Faker _faker;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new UserController(_userServiceMock.Object);
        _faker = new Faker();
    }

    #region AddTests

    [Fact]
    public async Task Add_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Username = _faker.Person.UserName,
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            CompanyId = _faker.Random.Int(1, 100),
            Email = _faker.Person.Email
        };
        var userId = _faker.Random.Int(1, 100);
        _userServiceMock.Setup(x => x.Add(request)).ReturnsAsync(userId);

        // Act
        var result = await _controller.Add(request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult?.ActionName.Should().Be(nameof(_controller.GetById));
        createdResult?.RouteValues?["userId"].Should().Be(userId);
        createdResult?.Value.Should().Be(userId);
        _userServiceMock.Verify(x => x.Add(request), Times.Once());
    }

    #endregion

    #region UpdateTests

    [Fact]
    public async Task Update_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            Id = _faker.Random.Int(1, 100),
            Username = _faker.Person.UserName,
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            CompanyId = _faker.Random.Int(1, 100),
            Email = _faker.Person.Email
        };
        _userServiceMock.Setup(x => x.Update(request)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _userServiceMock.Verify(x => x.Update(request), Times.Once());
    }

    #endregion

    #region DeleteTests

    [Fact]
    public async Task Delete_ExistingUser_ReturnsNoContent()
    {
        // Arrange
        var userId = _faker.Random.Int(1, 100);
        _userServiceMock.Setup(x => x.Delete(userId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(userId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _userServiceMock.Verify(x => x.Delete(userId), Times.Once());
    }

    #endregion

    #region GetTests

    [Fact]
    public async Task GetById_ExistingUser_ReturnsOkWithUser()
    {
        // Arrange
        var userId = _faker.Random.Int(1, 100);
        var user = CreatingUserResponse(_faker.Random.Int(1, 100));
        _userServiceMock.Setup(x => x.GetById(userId)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetById(userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(user);
        _userServiceMock.Verify(x => x.GetById(userId), Times.Once());
    }

    [Fact]
    public async Task GetAll_WithUsers_ReturnsOkWithUsers()
    {
        // Arrange
        var users = new List<UserResponse>
        {
            CreatingUserResponse(_faker.Random.Int(1, 100)),
            CreatingUserResponse(_faker.Random.Int(1, 100))
        };
        _userServiceMock.Setup(x => x.GetAll()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(users);
        _userServiceMock.Verify(x => x.GetAll(), Times.Once());
    }

    #endregion

    private UserResponse CreatingUserResponse(int companyId)
    {
        var user = new UserResponse()
        {
            Username = _faker.Person.UserName,
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            CompanyId = _faker.Random.Int(1, 100),
            Email = _faker.Person.Email
        };

        return user;
    }
}
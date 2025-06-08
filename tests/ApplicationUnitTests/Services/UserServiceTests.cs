using Application.Exceptions;
using Application.Mappings;
using Application.Requests;
using Application.Services;
using AutoMapper;
using Infrastructure.Repositories.UserRepository;
using Moq;
using Microsoft.Extensions.Logging;
using Bogus;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace ApplicationUnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly IUserService _userService;
    private readonly Faker _faker;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        var loggerMock = new Mock<ILogger<UserService>>();
        _faker = new Faker();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = mappingConfig.CreateMapper();

        _userService = new UserService(_userRepositoryMock.Object, mapper, loggerMock.Object);
    }

    # region AddTests

    [Fact]
    public async Task Add_WhenValidRequest_ReturnsUserId()
    {
        // Arrange
        var request = new CreateUserRequest()
        {
            Username = _faker.Person.UserName,
            Email = _faker.Person.Email,
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            CompanyId = _faker.Random.Int(1, 100),
            Role = UserRoles.User,
            Password = _passwordHasherMock.Object.HashPassword(_faker.Random.String(10))
        };

        _userRepositoryMock.Setup(i => i.CreateUser(It.IsAny<User>())).ReturnsAsync(1);

        // Act
        var result = await _userService.Add(request);

        // Assert
        result.Should().Be(1);
        _userRepositoryMock.Verify(i => i.CreateUser(It.Is<User>(u =>
            u.FirstName == request.FirstName &&
            u.Email == request.Email &&
            u.LastName == request.LastName &&
            u.CompanyId == request.CompanyId)), Times.Once);
    }

    #endregion

    #region UpdateTests

    [Fact]
    public async Task Update_WhenUserExist_UpdatesUserSuccessfully()
    {
        // Arrange 
        int userId = _faker.Random.Int(1, 100);
        var request = new UpdateUserRequest()
        {
            Id = userId,
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            Username = _faker.Person.UserName,
            Email = _faker.Person.Email,
            CompanyId = _faker.Random.Int(1, 100),
            Role = UserRoles.User,
            Password = _passwordHasherMock.Object.HashPassword(_faker.Random.String(10))
        };

        var user = CreatingUser(request.CompanyId ?? 0);
        user.Id = userId;

        _userRepositoryMock.Setup(i => i.ReadById(request.Id)).ReturnsAsync(user);
        _userRepositoryMock.Setup(i => i.UpdateUser(It.Is<User>(u =>
            u.Id == request.Id &&
            u.Username == request.Username &&
            u.Email == request.Email &&
            u.CompanyId == request.CompanyId &&
            u.LastName == request.LastName &&
            u.FirstName == request.FirstName))).ReturnsAsync(true);

        // Act
        await _userService.Update(request);

        // Assert
        _userRepositoryMock.Verify(i => i.ReadById(request.Id), Times.Once);
        _userRepositoryMock.Verify(i => i.UpdateUser(It.Is<User>(u =>
            u.Id == request.Id &&
            u.Email == request.Email &&
            u.CompanyId == request.CompanyId &&
            u.Username == request.Username &&
            u.LastName == request.LastName &&
            u.FirstName == request.FirstName)), Times.Once);
    }

    [Fact]
    public async Task Update_WhenUserNotFound_ThrowsNotFoundApplicationException()
    {
        // Arrange
        var request = new UpdateUserRequest()
        {
            Id = _faker.Random.Int(1, 100),
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            Username = _faker.Person.UserName,
            Email = _faker.Person.Email,
            CompanyId = _faker.Random.Int(1, 100),
            Role = UserRoles.User,
            Password = _passwordHasherMock.Object.HashPassword(_faker.Random.String(10))
        };

        _userRepositoryMock.Setup(i => i.ReadById(request.Id)).ReturnsAsync((User)null!);

        // Act & Assert
        await _userService.Invoking(i => i.Update(request))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("User not found");

        _userRepositoryMock.Verify(i => i.ReadById(request.Id), Times.Once);
        _userRepositoryMock.Verify(i => i.UpdateUser(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Update_WhenUpdateFails_ThrowsEntityUpdateException()
    {
        // Arrange
        int userId = _faker.Random.Int(1, 100);
        var request = new UpdateUserRequest()
        {
            Id = userId,
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            Username = _faker.Person.UserName,
            Email = _faker.Person.Email,
            CompanyId = _faker.Random.Int(1, 100),
            Role = UserRoles.User,
            Password = _passwordHasherMock.Object.HashPassword(_faker.Random.String(10))
        };

        var user = CreatingUser(request.CompanyId ?? 0);
        user.Id = userId;

        _userRepositoryMock.Setup(i => i.ReadById(request.Id)).ReturnsAsync(user);
        _userRepositoryMock.Setup(i => i.UpdateUser(It.IsAny<User>())).ReturnsAsync(false);

        // Act & Assert
        await _userService.Invoking(i => i.Update(request))
            .Should()
            .ThrowAsync<EntityUpdateException>()
            .WithMessage("Couldn't update the user");

        _userRepositoryMock.Verify(i => i.ReadById(request.Id), Times.Once);
        _userRepositoryMock.Verify(i => i.UpdateUser(It.IsAny<User>()), Times.Once);
    }

    #endregion

    #region DeleteTests

    [Fact]
    public async Task Delete_WhenUserExist_DeletesUserSuccessfully()
    {
        // Arrange
        int userId = _faker.Random.Int(1, 100);
        _userRepositoryMock.Setup(i => i.DeleteUser(userId)).ReturnsAsync(true);

        // Act
        await _userService.Delete(userId);

        // Assert
        _userRepositoryMock.Verify(i => i.DeleteUser(userId), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeletionFails_ThrowsEntityDeleteException()
    {
        // Arrange
        int userId = _faker.Random.Int(1, 100);
        _userRepositoryMock.Setup(i => i.DeleteUser(userId)).ReturnsAsync(false);

        // Act & Assert
        await _userService.Invoking(i => i.Delete(userId))
            .Should()
            .ThrowAsync<EntityDeleteException>()
            .WithMessage("Couldn't delete user");

        _userRepositoryMock.Verify(i => i.DeleteUser(userId), Times.Once);
    }

    #endregion

    #region GetTests

    [Fact]
    public async Task GetById_WhenUserExist_ReturnsUser()
    {
        // Arrange
        int userId = _faker.Random.Int(1, 100);
        var user = CreatingUser(_faker.Random.Int(1, 100));
        user.Id = userId;

        _userRepositoryMock.Setup(i => i.ReadById(userId)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetById(userId);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
        result.Username.Should().Be(user.Username);
        result.Email.Should().Be(user.Email);
        result.CompanyId.Should().Be(user.CompanyId);
        _userRepositoryMock.Verify(i => i.ReadById(userId), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenUserNotFound_ThrowsNotFoundApplicationException()
    {
        // Arrange
        int userId = _faker.Random.Int(1, 100);
        _userRepositoryMock.Setup(i => i.ReadById(userId)).ReturnsAsync((User)null!);

        // Act & Assert
        await _userService.Invoking(i => i.GetById(userId))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("User not found");
        _userRepositoryMock.Verify(i => i.ReadById(userId), Times.Once);
    }

    [Fact]
    public async Task GetAll_WhenUsersExist_ReturnsUserResponses()
    {
        // Arrange
        var users = new List<User>()
        {
            CreatingUser(_faker.Random.Int(1, 100)),
            CreatingUser(_faker.Random.Int(1, 100)),
        };

        _userRepositoryMock.Setup(i => i.ReadAll()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAll();
        var userResponses = result.ToList();

        // Assert
        userResponses.Should().HaveCount(2);
        userResponses.Select(r => r.Username).Should().Contain(users.Select(t => t.Username));
        _userRepositoryMock.Verify(i => i.ReadAll(), Times.Once());
    }

    [Fact]
    public async Task GetAll_WhenNoUsersExist_ReturnsEmptyCollection()
    {
        // Arrange
        _userRepositoryMock.Setup(i => i.ReadAll()).ReturnsAsync(new List<User>());

        // Act
        var result = await _userService.GetAll();

        // Assert
        result.Should().BeEmpty();
        _userRepositoryMock.Verify(i => i.ReadAll(), Times.Once());
    }

    #endregion

    private User CreatingUser(int companyId)
    {
        var user = new User()
        {
            Id = _faker.Random.Int(1, 100),
            FirstName = _faker.Person.FirstName,
            Username = _faker.Person.UserName,
            LastName = _faker.Person.LastName,
            Email = _faker.Person.Email,
            CompanyId = companyId,
            PasswordHash = _faker.Random.String(),
            Role = UserRoles.User
        };

        return user;
    }
}
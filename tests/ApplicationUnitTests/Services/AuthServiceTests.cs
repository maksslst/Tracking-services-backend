using Application.Mappings;
using Application.Requests;
using Application.Services;
using AutoMapper;
using Bogus;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Repositories.UserRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApplicationUnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly IAuthService _authService;
    private readonly Faker _faker;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        var loggerMock = new Mock<ILogger<AuthService>>();
        var configurationMock = new Mock<IConfiguration>();
        _faker = new Faker();

        var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = mappingConfig.CreateMapper();

        configurationMock.Setup(c => c["JwtSettings:Secret"])
            .Returns("your_very_secure_jwt_secret_key_here_minimum_32_chars");
        configurationMock.Setup(c => c["JwtSettings:Issuer"]).Returns("TestIssuer");
        configurationMock.Setup(c => c["JwtSettings:Audience"]).Returns("TestAudience");
        configurationMock.Setup(c => c["JwtSettings:ExpirationInMinutes"]).Returns("60");

        _authService = new AuthService(configurationMock.Object, mapper, _userRepositoryMock.Object,
            _passwordHasherMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsUserId()
    {
        // Arrange
        var password = _faker.Random.String(10);
        var request = new RegistrationRequest
        {
            Username = _faker.Person.UserName,
            Email = _faker.Person.Email,
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            Password = password,
            PasswordConfirmation = password
        };

        var hashedPassword = _faker.Random.String(12);
        _passwordHasherMock.Setup(p => p.HashPassword(request.Password)).Returns(hashedPassword);
        _userRepositoryMock.Setup(r => r.CreateUser(It.IsAny<User>())).ReturnsAsync(1);

        // Act
        var result = await _authService.Register(request);

        // Assert
        result.Should().Be(1);
        _userRepositoryMock.Verify(r => r.CreateUser(It.Is<User>(u =>
            u.Username == request.Username &&
            u.Email == request.Email &&
            u.FirstName == request.FirstName &&
            u.LastName == request.LastName &&
            u.PasswordHash == hashedPassword &&
            u.Role == UserRoles.User)), Times.Once());
        _passwordHasherMock.Verify(p => p.HashPassword(request.Password), Times.Once());
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsLoginResponseWithToken()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = _faker.Person.UserName,
            Password = _faker.Random.String(10)
        };

        var user = CreatingUser(_faker.Random.Int(1, 100));
        user.Username = request.Username;
        user.PasswordHash = _faker.Random.String(12);

        _userRepositoryMock.Setup(r => r.ReadByUsername(request.Username)).ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.VerifyPassword(request.Password, user.PasswordHash)).Returns(true);

        // Act
        var result = await _authService.Login(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        _userRepositoryMock.Verify(r => r.ReadByUsername(request.Username), Times.Once());
        _passwordHasherMock.Verify(p => p.VerifyPassword(request.Password, user.PasswordHash), Times.Once());
    }

    [Fact]
    public async Task Login_InvalidUsername_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = _faker.Person.UserName,
            Password = _faker.Random.String(10)
        };
        
        _userRepositoryMock.Setup(r => r.ReadByUsername(request.Username)).ReturnsAsync((User)null!);

        // Act & Assert
        await _authService.Invoking(a => a.Login(request))
            .Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid login attempt");

        _userRepositoryMock.Verify(r => r.ReadByUsername(request.Username), Times.Once());
        _passwordHasherMock.Verify(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
    }
    
    [Fact]
    public async Task Login_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = _faker.Person.UserName,
            Password = _faker.Random.String(10)
        };
        
        var user = CreatingUser(_faker.Random.Int(1, 100));
        user.Username = request.Username;
        user.PasswordHash = _faker.Random.String(12);

        _userRepositoryMock.Setup(r => r.ReadByUsername(request.Username)).ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.VerifyPassword(request.Password, user.PasswordHash)).Returns(false);

        // Act & Assert
        await _authService.Invoking(a => a.Login(request))
            .Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid login attempt");

        _userRepositoryMock.Verify(r => r.ReadByUsername(request.Username), Times.Once());
        _passwordHasherMock.Verify(p => p.VerifyPassword(request.Password, user.PasswordHash), Times.Once());
    }

    private User CreatingUser(int companyId)
    {
        return new User
        {
            Id = _faker.Random.Int(1, 100),
            Username = _faker.Person.UserName,
            Email = _faker.Person.Email,
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            CompanyId = companyId,
            Role = UserRoles.User,
            PasswordHash = _faker.Random.String(10)
        };
    }
}
using Application.Services;
using Bogus;
using FluentAssertions;

namespace ApplicationUnitTests.Services;

public class BCryptHasherTests
{
    private readonly BCryptHasher _hasher;
    private readonly Faker _faker;

    public BCryptHasherTests()
    {
        _hasher = new BCryptHasher();
        _faker = new Faker();
    }

    [Fact]
    public void HashPassword_ValidPassword_ReturnsNonEmptyHash()
    {
        // Arrange
        var password = _faker.Random.Word();

        // Act
        var hash = _hasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        Assert.NotEqual(hash, password);
    }

    [Fact]
    public void VerifyPassword_ValidPasswordAndHash_ReturnsTrue()
    {
        // Arrange
        var password = _faker.Random.Word();
        var hash = _hasher.HashPassword(password);

        // Act
        var result = _hasher.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_InvalidPassword_ReturnsFalse()
    {
        // Arrange
        var password = _faker.Random.Word();
        var wrongPassword = _faker.Random.Word();
        var hash = _hasher.HashPassword(password);

        // Act
        var result = _hasher.VerifyPassword(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_NullStoredHash_ReturnsFalse()
    {
        // Arrange
        var password = _faker.Random.Word();

        // Act
        var result = _hasher.VerifyPassword(password, null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_EmptyStoredHash_ReturnsFalse()
    {
        // Arrange
        var password = _faker.Random.Word();

        // Act
        var result = _hasher.VerifyPassword(password, string.Empty);

        // Assert
        result.Should().BeFalse();
    }
}
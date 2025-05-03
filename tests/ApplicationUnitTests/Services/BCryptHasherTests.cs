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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void VerifyPassword_NullOrEmptyStoredHash_ReturnsFalse(string storedHash)
    {
        // Arrange
        var password = _faker.Random.Word();

        // Act
        var result = _hasher.VerifyPassword(password, storedHash);

        // Assert
        result.Should().BeFalse();
    }
}
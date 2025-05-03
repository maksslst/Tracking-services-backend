using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Requests;
using Application.Responses;
using AutoMapper;
using Infrastructure.Repositories.UserRepository;
using Microsoft.Extensions.Configuration;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthService(
    IConfiguration configuration,
    IMapper mapper,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<int> Register(RegistrationRequest request)
    {
        var user = mapper.Map<User>(request);
        user.PasswordHash = passwordHasher.HashPassword(request.Password);
        user.Role = UserRoles.User;

        var userId = await userRepository.CreateUser(user);
        logger.LogInformation("User with id: {userId} registered successfully", userId);
        return userId;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var user = await userRepository.ReadByUsername(request.Username);
        bool passwordVerified = false;
        if (user != null)
        {
            passwordVerified = passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        }

        if (!passwordVerified)
        {
            throw new UnauthorizedAccessException("Invalid login attempt");
        }

        var token = GenerateJwtToken(user);
        logger.LogInformation("User with id: {userId} logged in", user.Id);
        return new LoginResponse(token);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSecret = configuration["JwtSettings:Secret"] ?? throw new ArgumentNullException("JwtSettings:Secret");
        var jwtIssuer = configuration["JwtSettings:Issuer"] ?? throw new ArgumentNullException("JwtSettings:Issuer");
        var jwtAudience = configuration["JwtSettings:Audience"] ??
                          throw new ArgumentNullException("JwtSettings:Audience");
        var jwtExpirationMinutes = int.Parse(configuration["JwtSettings:ExpirationInMinutes"] ?? "60");

        var key = Encoding.ASCII.GetBytes(jwtSecret);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            ]),
            Expires = DateTime.UtcNow.AddMinutes(jwtExpirationMinutes),
            Issuer = jwtIssuer,
            Audience = jwtAudience,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
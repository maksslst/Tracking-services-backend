using System.Security.Claims;
using Application.Exceptions;
using Application.Requests;
using AutoMapper;
using Infrastructure.Repositories.UserRepository;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AuthService(
    IMapper mapper,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<ClaimsPrincipal> Register(RegistrationRequest request)
    {
        var user = mapper.Map<User>(request);
        user.PasswordHash = passwordHasher.HashPassword(request.Password);
        user.Role = UserRoles.User;

        var userId = await userRepository.CreateUser(user);
        var createdUser = await userRepository.ReadById(userId);
        var principal = GenerateClaimsPrincipal(createdUser);
        
        logger.LogInformation("User with id: {userId} registered successfully", userId);
        return principal;
    }

    public async Task<ClaimsPrincipal> Login(LoginRequest request)
    {
        var user = await userRepository.ReadByUsername(request.Username);
        bool passwordVerified = false;
        if (user != null)
        {
            passwordVerified = passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        }

        if (!passwordVerified)
        {
            throw new UnauthorizedApplicationException("Invalid login attempt");
        }

        var principal = GenerateClaimsPrincipal(user);
        logger.LogInformation("User with id: {userId} logged in", user.Id);
        return principal;
    }

    private ClaimsPrincipal GenerateClaimsPrincipal(User user)
    {
        var identity = new ClaimsIdentity([
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        ], "HttponlyAuth");

        var principal = new ClaimsPrincipal(identity);
        return principal;
    }
}
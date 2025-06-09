using System.Security.Claims;
using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface IAuthService
{
    public Task<ClaimsPrincipal> Register(RegistrationRequest request);
    public Task<ClaimsPrincipal> Login(LoginRequest request);
}
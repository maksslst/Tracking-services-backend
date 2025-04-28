using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface IAuthService
{
    public Task<int> Register(RegistrationRequest request);
    public Task<LoginResponse> Login(LoginRequest request);
}
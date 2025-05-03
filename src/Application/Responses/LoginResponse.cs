namespace Application.Responses;

public class LoginResponse(string token)
{
    public string? Token { get; set; } = token;
}
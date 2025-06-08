namespace Application.Responses;

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Patronymic { get; set; }
    public string Email { get; set; } = null!;
    public int CompanyId { get; set; }
}
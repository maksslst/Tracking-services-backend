using Domain.Enums;

namespace Domain.Entities;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Patronymic { get; set; }
    public required string Email { get; set; }
    public UserRoles Role { get; set; }
    public string? PasswordHash { get; set; }
    public int? CompanyId { get; set; }
    public Company? Company { get; set; }
}
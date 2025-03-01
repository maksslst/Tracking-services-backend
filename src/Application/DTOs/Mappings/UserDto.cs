namespace Application.DTOs.Mappings;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public string? Patronymic {get; set;}
    public string Email {get; set;}
    public int CompanyId { get; set; }
    public CompanyDto CompanyDto { get; set; }
}
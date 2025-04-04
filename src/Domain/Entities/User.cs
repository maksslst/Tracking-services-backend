using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User
{
    public int Id { get; set; }
    
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    public string? Patronymic { get; set; }
    
    [Required]
    public string Email { get; set; }
    public int CompanyId { get; set; }
    public Company? Company { get; set; }
}
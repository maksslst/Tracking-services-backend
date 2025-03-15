namespace Application.DTOs.Mappings;

public class CompanyDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; }
    public List<UserDto> Users { get; set; }
    public List<ResourceDto> Services { get; set; }

    public CompanyDto()
    {
        Users = new List<UserDto>();
        Services = new List<ResourceDto>();
    }
}
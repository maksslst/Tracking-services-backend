namespace Application.DTOs.Mappings;

public class CompanyDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; }
    public List<UserDto> UsersDto { get; set; }
    public List<ResourceDto> ServicesDto { get; set; }

    public CompanyDto()
    {
        UsersDto = new List<UserDto>();
        ServicesDto = new List<ResourceDto>();
    }
}
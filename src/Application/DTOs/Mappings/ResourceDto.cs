using Application.DTOs.Enums;

namespace Application.DTOs.Mappings;

public class ResourceDto
{
    public int? Id { get; set; }
    public int? CompanyId { get; set; }
    public CompanyDto? CompanyDto { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Source { get; set; }
    public ResourceStatus Status { get; set; }
}
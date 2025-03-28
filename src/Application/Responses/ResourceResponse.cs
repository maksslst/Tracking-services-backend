using Domain.Enums;

namespace Application.Responses;

public class ResourceResponse
{
    public int? CompanyId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Source { get; set; }
    public ResourceStatus Status { get; set; }
}
using Domain.Enums;

namespace Application.Responses;

public class ResourceResponse
{
    public int? CompanyId { get; set; }
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Source { get; set; } = null!;
    public ResourceStatus Status { get; set; }
}
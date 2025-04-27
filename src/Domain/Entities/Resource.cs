using Domain.Enums;

namespace Domain.Entities;

public class Resource
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public Company? Company { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string Source { get; set; }
    public ResourceStatus Status { get; set; }
}
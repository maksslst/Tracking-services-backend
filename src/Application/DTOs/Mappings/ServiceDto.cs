namespace Application.DTOs.Mappings;

public class ServiceDto
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public CompanyDto CompanyDto { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Source { get; set; }
    public ServiceStatus Status { get; set; }
    public enum ServiceStatus
    {
        Active,
        Inactive
    }
}
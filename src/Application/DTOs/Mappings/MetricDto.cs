namespace Application.DTOs.Mappings;

public class MetricDto
{
    public int Id { get; set; }
    public double Value { get; set; }
    public string Name { get; set; }
    public int ResourceId { get; set; }
    public ResourceDto Resource { get; set; }
    public DateTime Created { get; set; }
    public string Unit { get; set; }
}
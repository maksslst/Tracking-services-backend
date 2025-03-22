namespace Application.DTOs.Mappings;

public class MetricValueDto
{
    public int Id { get; set; }
    public int MetricId { get; set; }
    public MetricDto? MetricDto { get; set; }  
    public double Value { get; set; }
}
namespace Application.DTOs.Mappings;

public class AlertDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public ServiceDto ServiceDto { get; set; }
    public int MetricId { get; set; }
    public MetricDto MetricDto { get; set; }
    public string Message { get; set; }
    public DateTime Created { get; set; }
}
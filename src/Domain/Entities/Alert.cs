namespace Domain.Entities;

public class Alert
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public Service Service { get; set; }
    public int MetricId { get; set; }
    public Metric Metric { get; set; }
    public string Message { get; set; }
    public DateTime Created { get; set; }
}
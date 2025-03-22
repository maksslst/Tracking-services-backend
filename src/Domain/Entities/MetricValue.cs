namespace Domain.Entities;

public class MetricValue
{
    public int Id { get; set; }
    public int MetricId { get; set; }
    public Metric Metric { get; set; }
    public double Value { get; set; }
}
namespace Domain.Entities;

public class Metric
{
    public int Id { get; set; }
    public double Value { get; set; }
    public string Name { get; set; }
    public int ServiceId { get; set; }
    public Service Service { get; set; }
    public DateTime Created { get; set; }
    public string Unit { get; set; }
}
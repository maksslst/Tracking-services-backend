namespace Domain.Entities;

public class Metric
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ServiceId { get; set; }
    public Resource Resource { get; set; }
    public DateTime Created { get; set; }
    public string Unit { get; set; }
}
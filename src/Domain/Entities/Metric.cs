namespace Domain.Entities;

public class Metric
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int ResourceId { get; set; }
    public Resource? Resource { get; set; }
    public DateTime Created { get; set; }
    public required string Unit { get; set; }
}
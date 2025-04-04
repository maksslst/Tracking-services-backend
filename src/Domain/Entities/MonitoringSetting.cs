namespace Domain.Entities;

public class MonitoringSetting
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    public Resource? Resource { get; set; }
    public required string CheckInterval { get; set; }
    public bool Mode { get; set; }
}
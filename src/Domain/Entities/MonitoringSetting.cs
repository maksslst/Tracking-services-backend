namespace Domain.Entities;

public class MonitoringSetting
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public Service service { get; set; }
    public string CheckInterval { get; set; }
    public bool Mode { get; set; }
}
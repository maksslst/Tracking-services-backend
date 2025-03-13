namespace Application.DTOs.Mappings;

public class MonitoringSettingDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public ResourceDto ResourceDto { get; set; }
    public string CheckInterval { get; set; }
    public bool Mode { get; set; }
}
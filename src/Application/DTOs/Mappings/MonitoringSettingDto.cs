namespace Application.DTOs.Mappings;

public class MonitoringSettingDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public ServiceDto ServiceDto { get; set; }
    public DateTime CheckInterval { get; set; }
    public bool Mode { get; set; }
}
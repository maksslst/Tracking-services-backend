namespace Application.Responses;

public class MonitoringSettingResponse
{
    public int ResourceId { get; set; }
    public string CheckInterval { get; set; } = null!;
    public bool Mode { get; set; }
}
namespace Application.Responses;

public class MonitoringSettingResponse
{
    public int ResourceId { get; set; }
    public string CheckInterval { get; set; }
    public bool Mode { get; set; }
}
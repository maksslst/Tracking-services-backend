using Application.DTOs.Mappings;

namespace Application.Services;

public interface IMonitoringSettingService
{
    public Task Add (MonitoringSettingDto monitoringSettingDto);
    public Task<bool> Update (MonitoringSettingDto monitoringSettingDto);
    public Task<bool> Delete (MonitoringSettingDto monitoringSettingDto);
    public Task<MonitoringSettingDto?> GetByServiceId(int serviceId);
}
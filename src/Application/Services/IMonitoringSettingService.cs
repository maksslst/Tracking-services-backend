using Application.DTOs.Mappings;
using Domain.Entities;

namespace Application.Services;

public interface IMonitoringSettingService
{
    public Task<MonitoringSetting?> Add(MonitoringSettingDto monitoringSettingDto);
    public Task<bool> Update(MonitoringSettingDto monitoringSettingDto);
    public Task<bool> Delete(int monitoringSettingId);
    public Task<MonitoringSettingDto?> GetMonitoringSetting(int serviceId);
}
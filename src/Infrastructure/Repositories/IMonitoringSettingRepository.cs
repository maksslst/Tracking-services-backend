using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IMonitoringSettingRepository
{
    public Task<MonitoringSetting> CreateSetting(MonitoringSetting monitoringSetting);
    public Task<bool> UpdateSetting(MonitoringSetting monitoringSetting);
    public Task<bool> DeleteSetting(int monitoringSettingId);
    public Task<MonitoringSetting?> ReadByServiceId(int serviceId);
    public Task<IEnumerable<MonitoringSetting?>> ReadAll();
}
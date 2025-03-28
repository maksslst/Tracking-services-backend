using Domain.Entities;

namespace Infrastructure.Repositories.MonitoringSettingRepository;

public interface IMonitoringSettingRepository
{
    public Task<int> CreateSetting(MonitoringSetting monitoringSetting);
    public Task<bool> UpdateSetting(MonitoringSetting monitoringSetting);
    public Task<bool> DeleteSetting(int monitoringSettingId);
    public Task<MonitoringSetting?> ReadByResourceId(int resourceId);
    public Task<IEnumerable<MonitoringSetting?>> ReadAll();
    public Task<MonitoringSetting?> ReadById(int monitoringSettingId);
}
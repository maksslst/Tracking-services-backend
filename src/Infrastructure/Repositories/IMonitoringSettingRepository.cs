using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IMonitoringSettingRepository
{
    public Task CreateSetting (MonitoringSetting monitoringSetting);
    public Task<bool> UpdateSetting (MonitoringSetting monitoringSetting);
    public Task<bool> DeleteSetting (MonitoringSetting monitoringSetting);
    public Task<MonitoringSetting?> ReadByServiceId(int serviceId);
}
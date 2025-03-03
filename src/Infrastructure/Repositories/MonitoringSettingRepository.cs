using Domain.Entities;

namespace Infrastructure.Repositories;

public class MonitoringSettingRepository : IMonitoringSettingRepository
{
    private List<MonitoringSetting> _monitoringSettings;

    public MonitoringSettingRepository()
    {
        _monitoringSettings = new List<MonitoringSetting>();
    }
    
    public Task CreateSetting(MonitoringSetting monitoringSetting)
    {
        _monitoringSettings.Add(monitoringSetting);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateSetting(MonitoringSetting monitoringSetting)
    {
        var monitoringSettingToUpdate = _monitoringSettings.Find(i=> i.Id == monitoringSetting.Id);
        if (monitoringSettingToUpdate == null)
        {
            return Task.FromResult(false);
        }

        monitoringSettingToUpdate.Mode = monitoringSetting.Mode;
        monitoringSettingToUpdate.CheckInterval = monitoringSetting.CheckInterval;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteSetting(MonitoringSetting monitoringSetting)
    {
        var monitoringSettingToDelete = _monitoringSettings.Find(i => i.Id == monitoringSetting.Id);
        if (monitoringSettingToDelete == null)
        {
            return Task.FromResult(false);
        }
        
        _monitoringSettings.Remove(monitoringSettingToDelete);
        return Task.FromResult(true);
    }

    public Task<MonitoringSetting?> ReadByServiceId(int serviceId)
    {
        return Task.FromResult(_monitoringSettings.Find(i => i.ServiceId == serviceId));
    }
    
    public Task<List<MonitoringSetting?>> ReadAll()
    {
        return Task.FromResult(_monitoringSettings);
    }
}
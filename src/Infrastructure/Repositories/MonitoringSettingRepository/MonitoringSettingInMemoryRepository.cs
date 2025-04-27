using System.Diagnostics.CodeAnalysis;
using Domain.Entities;

namespace Infrastructure.Repositories.MonitoringSettingRepository;

[ExcludeFromCodeCoverage]
public class MonitoringSettingInMemoryRepository : IMonitoringSettingRepository
{
    private List<MonitoringSetting> _monitoringSettings;

    public MonitoringSettingInMemoryRepository()
    {
        _monitoringSettings = new List<MonitoringSetting>();
        DataGeneration();
    }

    public Task<int> CreateSetting(MonitoringSetting monitoringSetting)
    {
        _monitoringSettings.Add(monitoringSetting);
        return Task.FromResult(monitoringSetting.Id);
    }

    public Task<bool> UpdateSetting(MonitoringSetting monitoringSetting)
    {
        var monitoringSettingToUpdate = _monitoringSettings.Find(i => i.Id == monitoringSetting.Id);
        if (monitoringSettingToUpdate == null)
        {
            return Task.FromResult(false);
        }

        monitoringSettingToUpdate.Mode = monitoringSetting.Mode;
        monitoringSettingToUpdate.CheckInterval = monitoringSetting.CheckInterval;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteSetting(int monitoringSettingId)
    {
        var monitoringSettingToDelete = _monitoringSettings.Find(i => i.Id == monitoringSettingId);
        if (monitoringSettingToDelete == null)
        {
            return Task.FromResult(false);
        }

        _monitoringSettings.Remove(monitoringSettingToDelete);
        return Task.FromResult(true);
    }

    public Task<MonitoringSetting?> ReadByResourceId(int resourceId)
    {
        return Task.FromResult(_monitoringSettings.Find(i => i.ResourceId == resourceId));
    }

    public Task<IEnumerable<MonitoringSetting?>> ReadAll()
    {
        return Task.FromResult<IEnumerable<MonitoringSetting?>>(_monitoringSettings);
    }

    public Task<MonitoringSetting?> ReadById(int monitoringSettingId)
    {
        return Task.FromResult(_monitoringSettings.Find(i => i.Id == monitoringSettingId));
    }

    private void DataGeneration()
    {
        for (int i = 0; i < 5;i++)
        {
            MonitoringSetting monitoringSetting = new MonitoringSetting()
            {
                Id = i + 1,
                ResourceId = i + 1,
                CheckInterval = "0 0/5 * * * ?",
                Mode = i % 2 == 0
            };
            
            _monitoringSettings.Add(monitoringSetting);
        }
    }
}
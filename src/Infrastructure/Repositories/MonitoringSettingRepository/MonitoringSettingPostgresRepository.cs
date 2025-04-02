using Dapper;
using Domain.Entities;
using Npgsql;

namespace Infrastructure.Repositories.MonitoringSettingRepository;

public class MonitoringSettingPostgresRepository : IMonitoringSettingRepository
{
    private readonly NpgsqlConnection _connection;

    public MonitoringSettingPostgresRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> CreateSetting(MonitoringSetting monitoringSetting)
    {
        var settingId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO monitoring_settings (resource_id, check_interval, mode)
                VALUES(@ResourceId, @CheckInterval, @Mode)
                RETURNING id",
            new { monitoringSetting.ResourceId, monitoringSetting.CheckInterval, monitoringSetting.Mode });

        return settingId;
    }

    public async Task<bool> UpdateSetting(MonitoringSetting monitoringSetting)
    {
        var settingToUpdate = await _connection.ExecuteAsync(
            @"UPDATE monitoring_settings
                SET resource_id = @ResourceId, 
                    check_interval = @CheckInterval, 
                    mode = @Mode
                WHERE id = @Id", monitoringSetting);

        return settingToUpdate > 0;
    }

    public async Task<bool> DeleteSetting(int monitoringSettingId)
    {
        var settingToDelete = await _connection.ExecuteAsync(
            @"DELETE FROM monitoring_settings
                WHERE id = @Id", new { Id = monitoringSettingId });

        return settingToDelete > 0;
    }

    public async Task<MonitoringSetting?> ReadByResourceId(int resourceId)
    {
        var monitoringSetting = await _connection.QueryFirstOrDefaultAsync<MonitoringSetting>(
            @"SELECT id, resource_id, check_interval, mode
                FROM monitoring_settings
                WHERE resource_id = @ResourceId", new { ResourceId = resourceId });

        return monitoringSetting;
    }

    public async Task<IEnumerable<MonitoringSetting?>> ReadAll()
    {
        var settings = await _connection.QueryAsync<MonitoringSetting>(
            @"SELECT id, resource_id, check_interval, mode
                FROM monitoring_settings");

        return settings;
    }

    public async Task<MonitoringSetting?> ReadById(int monitoringSettingId)
    {
        var monitoringSetting = await _connection.QueryFirstOrDefaultAsync<MonitoringSetting>(
            @"SELECT id, resource_id, check_interval, mode
                FROM monitoring_settings
                WHERE id = @Id", new { Id = monitoringSettingId });
        
        return monitoringSetting;
    }
}
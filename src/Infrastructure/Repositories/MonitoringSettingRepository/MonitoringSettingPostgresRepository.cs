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
        await _connection.OpenAsync();

        var settingId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO ""monitoringSettings"" (resource_id, checkinterval, mode)
                VALUES(@ResourceId, @CheckInterval, @Mode)
                RETURNING id",
            new { monitoringSetting.ResourceId, monitoringSetting.CheckInterval, monitoringSetting.Mode });

        await _connection.CloseAsync();
        return settingId;
    }

    public async Task<bool> UpdateSetting(MonitoringSetting monitoringSetting)
    {
        await _connection.OpenAsync();

        var settingToUpdate = await _connection.ExecuteAsync(
            @"UPDATE ""monitoringSettings""
                SET resource_id = @ResourceId, 
                    checkinterval = @CheckInterval, 
                    mode = @Mode
                WHERE id=@Id", monitoringSetting);

        await _connection.CloseAsync();
        return settingToUpdate > 0;
    }

    public async Task<bool> DeleteSetting(int monitoringSettingId)
    {
        await _connection.OpenAsync();

        var settingToDelete = await _connection.ExecuteAsync(
            @"DELETE FROM ""monitoringSettings""
                WHERE id = @Id", new { Id = monitoringSettingId });

        await _connection.CloseAsync();
        return settingToDelete > 0;
    }

    public async Task<MonitoringSetting?> ReadByResourceId(int resourceId)
    {
        await _connection.OpenAsync();

        MonitoringSetting monitoringSetting = await _connection.QueryFirstOrDefaultAsync<MonitoringSetting>(
            @"SELECT id, resource_id as ResourceId, checkinterval, mode
                FROM ""monitoringSettings""
                WHERE resource_id = @ResourceId", new { ResourceId = resourceId });

        await _connection.CloseAsync();
        return monitoringSetting;
    }

    public async Task<IEnumerable<MonitoringSetting?>> ReadAll()
    {
        await _connection.OpenAsync();

        var settings = await _connection.QueryAsync<MonitoringSetting>(
            @"SELECT id, resource_id as ResourceId, checkinterval, mode
                FROM ""monitoringSettings""");

        await _connection.CloseAsync();
        return settings;
    }
}
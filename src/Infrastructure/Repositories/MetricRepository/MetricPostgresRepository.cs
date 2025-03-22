using Dapper;
using Domain.Entities;
using Npgsql;

namespace Infrastructure.Repositories.MetricRepository;

public class MetricPostgresRepository : IMetricRepository
{
    private readonly NpgsqlConnection _connection;

    public MetricPostgresRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> CreateMetric(Metric metric)
    {
        await _connection.OpenAsync();

        var metricId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO metrics (name, resource_id, created, unit) 
                VALUES (@name, @resourceId, @created, @unit)
                RETURNING id",
            new { metric.ResourceId, metric.Name, metric.Created, metric.Unit });

        await _connection.CloseAsync();
        return metricId;
    }

    public async Task<bool> UpdateMetric(Metric metric)
    {
        await _connection.OpenAsync();

        var metricToUpdate = await _connection.ExecuteAsync(
            @"UPDATE metrics
                SET name = @name, 
                    resource_id = @resourceId, 
                    unit = @unit
                WHERE id = @Id", metric);

        await _connection.CloseAsync();
        return metricToUpdate > 0;
    }

    public async Task<bool> DeleteMetric(int metricId)
    {
        await _connection.OpenAsync();

        var metricToDelete = await _connection.ExecuteAsync(
            @"DELETE FROM metrics
                WHERE id = @Id", new { Id = metricId });

        await _connection.CloseAsync();
        return metricToDelete > 0;
    }

    public async Task<Metric?> ReadMetricByResourceId(int resourceId)
    {
        await _connection.OpenAsync();

        Metric metric = await _connection.QueryFirstOrDefaultAsync<Metric>(
            @"SELECT id, name, resource_id as ResourceId, unit
                FROM metrics
                WHERE resource_id=@resource", new { resource = resourceId });

        await _connection.CloseAsync();
        return metric;
    }

    public async Task<Metric?> ReadMetricId(int metricId)
    {
        await _connection.OpenAsync();

        Metric metric = await _connection.QueryFirstOrDefaultAsync<Metric>(
            @"SELECT id, name, resource_id as ResourceId, unit
                FROM metrics
                WHERE id = @Id", new { Id = metricId });

        await _connection.CloseAsync();
        return metric;
    }

    public async Task<IEnumerable<Metric?>> ReadAllMetricValuesForResource(int resourceId)
    {
        await _connection.OpenAsync();

        var metrics = await _connection.QueryAsync<Metric>(
            @"SELECT id, name, resource_id as ResourceId, unit
                FROM metrics
                WHERE resource_id=@ResourceId", new { ResourceId = resourceId });

        await _connection.CloseAsync();
        return metrics;
    }

    public async Task<IEnumerable<Metric?>> ReadAll()
    {
        await _connection.OpenAsync();

        var metrics = await _connection.QueryAsync<Metric>(
            @"SELECT id, name, resource_id as ResourceId, unit
                FROM metrics");

        await _connection.CloseAsync();
        return metrics;
    }
}
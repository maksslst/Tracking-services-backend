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
        var metricId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO metrics (name, resource_id, created, unit) 
                VALUES (@name, @resourceId, @created, @unit)
                RETURNING id",
            new { metric.ResourceId, metric.Name, created = DateTime.Now, metric.Unit });

        return metricId;
    }

    public async Task<bool> UpdateMetric(Metric metric)
    {
        var metricToUpdate = await _connection.ExecuteAsync(
            @"UPDATE metrics
                SET name = @name, 
                    resource_id = @resourceId, 
                    unit = @unit
                WHERE id = @Id", metric);

        return metricToUpdate > 0;
    }

    public async Task<bool> DeleteMetric(int metricId)
    {
        var metricToDelete = await _connection.ExecuteAsync(
            @"DELETE FROM metrics
                WHERE id = @Id", new { Id = metricId });

        return metricToDelete > 0;
    }

    public async Task<Metric?> ReadMetricByResourceId(int resourceId)
    {
        var metric = await _connection.QueryFirstOrDefaultAsync<Metric>(
            @"SELECT id, name, resource_id, unit
                FROM metrics
                WHERE resource_id=@resource", new { resource = resourceId });

        return metric;
    }

    public async Task<Metric?> ReadMetricId(int metricId)
    {
        var metric = await _connection.QueryFirstOrDefaultAsync<Metric>(
            @"SELECT id, name, resource_id, unit
                FROM metrics
                WHERE id = @Id", new { Id = metricId });

        return metric;
    }

    public async Task<IEnumerable<Metric?>> ReadAllMetricValuesForResource(int resourceId)
    {
        var metrics = await _connection.QueryAsync<Metric>(
            @"SELECT id, name, resource_id, unit
                FROM metrics
                WHERE resource_id=@ResourceId", new { ResourceId = resourceId });

        return metrics;
    }

    public async Task<IEnumerable<Metric?>> ReadAll()
    {
        var metrics = await _connection.QueryAsync<Metric>(
            @"SELECT id, name, resource_id, unit
                FROM metrics");

        return metrics;
    }
}
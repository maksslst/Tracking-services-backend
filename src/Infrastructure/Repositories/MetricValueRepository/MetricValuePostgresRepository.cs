using Dapper;
using Domain.Entities;
using Npgsql;

namespace Infrastructure.Repositories.MetricValueRepository;

public class MetricValuePostgresRepository : IMetricValueRepository
{
    private readonly NpgsqlConnection _connection;

    public MetricValuePostgresRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> CreateMetricValue(MetricValue metricValue)
    {
        await _connection.OpenAsync();

        var metricValueId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO ""metricValues"" (metric_id, value) 
                VALUES (@metricId, @value)
                RETURNING Id",
            new { metricValue.MetricId, metricValue.Value });

        await _connection.CloseAsync();
        return metricValueId;
    }

    public async Task<MetricValue?> ReadMetricValueId(int metricValueId)
    {
        await _connection.OpenAsync();

        MetricValue? metricValue = await _connection.QueryFirstOrDefaultAsync<MetricValue>(
            @"SELECT id, metric_id, value
                FROM ""metricValues""
                WHERE id = @Id", new { Id = metricValueId });

        await _connection.CloseAsync();
        return metricValue;
    }

    public async Task<IEnumerable<MetricValue?>> ReadAllMetricValuesForMetricsId(IEnumerable<int> metricsId)
    {
        await _connection.OpenAsync();

        var metricValues = await _connection.QueryAsync<MetricValue>(
            @"SELECT id, metric_id as MetricId, value
                FROM ""metricValues""
                WHERE metric_id = any(@metrics)", new { metrics = metricsId.ToList() });

        await _connection.CloseAsync();
        return metricValues;
    }
}
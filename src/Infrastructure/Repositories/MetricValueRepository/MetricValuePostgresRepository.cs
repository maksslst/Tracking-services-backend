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
        var metricValueId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO metric_values (metric_id, value) 
                VALUES (@metricId, @value)
                RETURNING Id",
            new { metricValue.MetricId, metricValue.Value });

        return metricValueId;
    }

    public async Task<MetricValue?> ReadMetricValueId(int metricValueId)
    {
        var metricValue = await _connection.QueryFirstOrDefaultAsync<MetricValue>(
            @"SELECT id, metric_id as MetricId, value
                FROM metric_values
                WHERE id = @Id", new { Id = metricValueId });

        return metricValue;
    }

    public async Task<IEnumerable<MetricValue?>> ReadAllMetricValuesForMetricsId(IEnumerable<int> metricsId)
    {
        var metricValues = await _connection.QueryAsync<MetricValue>(
            @"SELECT id, metric_id as MetricId, value
                FROM metric_values
                WHERE metric_id = any(@metrics)", new { metrics = metricsId.ToList() });

        return metricValues;
    }
}
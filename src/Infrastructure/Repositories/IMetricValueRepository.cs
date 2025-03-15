using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IMetricValueRepository
{
    public Task<MetricValue> CreateMetricValue(MetricValue metricValue);
    public Task<MetricValue?> ReadMetricValueId (int metricValueId);
    public Task<IEnumerable<MetricValue?>> ReadAllMetricValuesForMetricsId (IEnumerable<int> metricsId);
}
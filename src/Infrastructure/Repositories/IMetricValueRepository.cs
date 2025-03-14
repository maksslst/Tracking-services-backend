using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IMetricValueRepository
{
    public Task<MetricValue> CreateMetricValue(MetricValue metricValue);
    public Task<IEnumerable<MetricValue?>> ReadAllMetricValuesForMetricsId (IEnumerable<int> metricsId);
}
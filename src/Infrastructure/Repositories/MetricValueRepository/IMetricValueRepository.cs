using Domain.Entities;

namespace Infrastructure.Repositories.MetricValueRepository;

public interface IMetricValueRepository
{
    public Task<int> CreateMetricValue(MetricValue metricValue);
    public Task<MetricValue?> ReadMetricValueId (int metricValueId);
    public Task<IEnumerable<MetricValue?>> ReadAllMetricValuesForMetricsId (IEnumerable<int> metricsId);
}
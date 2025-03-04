using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IMetricValueRepository
{
    public Task CreateMetricValue(MetricValue metricValue);
    public Task<List<MetricValue?>> ReadAllMetricValuesServiceId (int serviceId);
}
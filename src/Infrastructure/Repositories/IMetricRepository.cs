using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IMetricRepository
{
    public Task<Metric> CreateMetric(Metric metric);
    public Task<bool> UpdateMetric(Metric metric);
    public Task<bool> DeleteMetric(int metricId);
    public Task<Metric?> ReadMetricByServiceId(int serviceId);
    public Task<Metric?> ReadMetricId(int metricId);
    public Task<IEnumerable<Metric?>> ReadAllMetricValuesForResource (int serviceId);
    public Task<IEnumerable<Metric?>> ReadAll();
}
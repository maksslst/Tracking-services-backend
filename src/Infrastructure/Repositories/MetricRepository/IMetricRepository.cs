using Domain.Entities;

namespace Infrastructure.Repositories.MetricRepository;

public interface IMetricRepository
{
    public Task<int> CreateMetric(Metric metric);
    public Task<bool> UpdateMetric(Metric metric);
    public Task<bool> DeleteMetric(int metricId);
    public Task<Metric?> ReadMetricByResourceId(int resourceId);
    public Task<Metric?> ReadMetricId(int metricId);
    public Task<IEnumerable<Metric?>> ReadAllMetricValuesForResource (int resourceId);
    public Task<IEnumerable<Metric?>> ReadAll();
}
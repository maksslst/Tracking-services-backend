using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IMetricRepository
{
    public Task CreateMetric(Metric metric);
    public Task<bool> UpdateMetric(Metric metric);
    public Task<bool> DeleteMetric(int metricId);
    public Task<Metric?> ReadMetricServiceId(int serviceId);
    public Task<List<Metric?>> ReadAllMetricServiceId (int serviceId);
    public Task<List<Metric?>> ReadAll();
}
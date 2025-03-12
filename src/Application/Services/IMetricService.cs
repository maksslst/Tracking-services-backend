using Application.DTOs.Mappings;

namespace Application.Services;

public interface IMetricService
{
    public Task AddMetric(MetricDto metricDto);
    public Task<bool> UpdateMetric(MetricDto metricDto);
    public Task<bool> DeleteMetric(int metricId);
    public Task<MetricDto?> GetMetricServiceId(int serviceId);
    public Task<List<MetricDto?>> GetAllMetricServiceId (int serviceId);
    public Task<List<MetricDto?>> GetAll();
}
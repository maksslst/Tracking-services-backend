using Application.DTOs.Mappings;
using Domain.Entities;

namespace Application.Services;

public interface IMetricService
{
    public Task<Metric?> AddMetric(MetricDto metricDto);
    public Task<bool> UpdateMetric(MetricDto metricDto);
    public Task<bool> DeleteMetric(int metricId);
    public Task<MetricDto?> GetMetricByServiceId(int serviceId);
    public Task<IEnumerable<MetricDto?>> GetAllMetricsByServiceId(int serviceId);
    public Task<IEnumerable<MetricDto?>> GetAll();
}
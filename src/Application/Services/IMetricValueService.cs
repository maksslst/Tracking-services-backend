using Application.DTOs.Mappings;

namespace Application.Services;

public interface IMetricValueService
{
    public Task AddMetricValue(MetricValueDto metricValueDto);
    public Task<List<MetricValueDto?>> GetAllMetricValuesServiceId (int serviceId);
}
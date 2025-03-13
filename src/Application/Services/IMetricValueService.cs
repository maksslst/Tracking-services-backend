using Application.DTOs.Mappings;
using Domain.Entities;

namespace Application.Services;

public interface IMetricValueService
{
    public Task<MetricValue?> AddMetricValue(MetricValueDto metricValueDto);
    public Task<IEnumerable<MetricValueDto?>> GetAllMetricValuesForService (int serviceId);
}
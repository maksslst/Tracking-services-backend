using Application.DTOs.Mappings;
using Domain.Entities;

namespace Application.Services;

public interface IMetricValueService
{
    public Task<MetricValue?> AddMetricValue(MetricValueDto metricValueDto);
    public Task<IEnumerable<MetricValueDto?>> GetAllMetricValuesForResource(int resourceId);
    public Task<MetricValueDto?> GetMetricValue(int metricValueId);
}
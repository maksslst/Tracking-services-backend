using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface IMetricValueService
{
    public Task<int> AddMetricValue(CreateMetricValueRequest request);
    public Task<IEnumerable<MetricValueResponse>> GetAllMetricValuesForResource(int resourceId);
    public Task<MetricValueResponse> GetMetricValue(int metricValueId);
}
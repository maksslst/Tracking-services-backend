using Application.DTOs.Mappings;
using Domain.Entities;
using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface IMetricService
{
    public Task<int> AddMetric(CreateMetricRequest request);
    public Task<bool> UpdateMetric(UpdateMetricRequest request);
    public Task<bool> DeleteMetric(int metricId);
    public Task<MetricResponse> GetMetricByResourceId(int serviceId);
    public Task<IEnumerable<MetricResponse>> GetAllMetricsByServiceId(int serviceId);
    public Task<IEnumerable<MetricResponse>> GetAll();
}
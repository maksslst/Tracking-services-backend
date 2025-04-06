using Application.DTOs.Mappings;
using Domain.Entities;
using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface IMetricService
{
    public Task<int> AddMetric(CreateMetricRequest request);
    public Task UpdateMetric(UpdateMetricRequest request);
    public Task DeleteMetric(int metricId);
    public Task<MetricResponse> GetMetricByResourceId(int resourceId);
    public Task<IEnumerable<MetricResponse>> GetAllMetricsByResourceId(int resourceId);
    public Task<IEnumerable<MetricResponse>> GetAll();
}
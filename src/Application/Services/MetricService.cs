using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.MetricRepository;
using Infrastructure.Repositories.ResourceRepository;
using Application.Requests;
using Application.Responses;

namespace Application.Services;

public class MetricService : IMetricService
{
    private readonly IMetricRepository _metricRepository;
    private readonly IMapper _mapper;
    private readonly IResourceRepository _resourceRepository;

    public MetricService(IMetricRepository repository, IMapper mapper, IResourceRepository resourceRepository)
    {
        _metricRepository = repository;
        _mapper = mapper;
        _resourceRepository = resourceRepository;
    }

    public async Task<int> AddMetric(CreateMetricRequest request)
    {
        if (await _resourceRepository.ReadByResourceId(request.ResourceId) == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        var metric = _mapper.Map<Metric>(request);
        return await _metricRepository.CreateMetric(metric);
    }

    public async Task<bool> UpdateMetric(UpdateMetricRequest request)
    {
        var metric = await _metricRepository.ReadMetricId(request.MetricId);
        if (metric == null)
        {
            throw new NotFoundApplicationException("Metric not found");
        }

        metric.Name = request.Name;
        metric.Unit = request.Unit;
        metric.ResourceId = request.ResourceId;

        return await _metricRepository.UpdateMetric(metric);
    }

    public async Task<bool> DeleteMetric(int metricId)
    {
        bool isDeleted = await _metricRepository.DeleteMetric(metricId);
        if (!isDeleted)
        {
            throw new NotFoundApplicationException("Metric not found");
        }

        return true;
    }

    public async Task<MetricResponse> GetMetricByResourceId(int serviceId)
    {
        var metric = await _metricRepository.ReadMetricByResourceId(serviceId);
        if (metric == null)
        {
            throw new NotFoundApplicationException("Metric not found");
        }

        return _mapper.Map<MetricResponse>(metric);
    }

    public async Task<IEnumerable<MetricResponse>> GetAllMetricsByServiceId(int serviceId)
    {
        var metrics = await _metricRepository.ReadAllMetricValuesForResource(serviceId);
        if (metrics == null || metrics.Count() == 0)
        {
            throw new NotFoundApplicationException("Metrics not found");
        }

        var metricsResponses = metrics.Select(i => _mapper.Map<MetricResponse>(i));
        return metricsResponses;
    }

    public async Task<IEnumerable<MetricResponse>> GetAll()
    {
        var metrics = await _metricRepository.ReadAll();
        if (metrics == null || metrics.Count() == 0)
        {
            throw new NotFoundApplicationException("Metrics not found");
        }

        var metricsResponses = metrics.Select(i => _mapper.Map<MetricResponse>(i));
        return metricsResponses;
    }
}
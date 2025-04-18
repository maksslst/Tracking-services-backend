using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.MetricRepository;
using Infrastructure.Repositories.ResourceRepository;
using Application.Requests;
using Application.Responses;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class MetricService : IMetricService
{
    private readonly IMetricRepository _metricRepository;
    private readonly IMapper _mapper;
    private readonly IResourceRepository _resourceRepository;
    private readonly ILogger<MetricService> _logger;

    public MetricService(IMetricRepository repository, IMapper mapper, IResourceRepository resourceRepository,
        ILogger<MetricService> logger)
    {
        _metricRepository = repository;
        _mapper = mapper;
        _resourceRepository = resourceRepository;
        _logger = logger;
    }

    public async Task<int> AddMetric(CreateMetricRequest request)
    {
        var metric = _mapper.Map<Metric>(request);
        var metricId = await _metricRepository.CreateMetric(metric);
        _logger.LogInformation("Created metric with id: {metricId}", metricId);
        return metricId;
    }

    public async Task UpdateMetric(UpdateMetricRequest request)
    {
        var metric = await _metricRepository.ReadMetricId(request.Id);
        if (metric == null)
        {
            throw new NotFoundApplicationException("Metric not found");
        }

        metric = _mapper.Map<Metric>(request);
        bool isUpdated = await _metricRepository.UpdateMetric(metric);
        if (!isUpdated)
        {
            throw new EntityUpdateException("Couldn't update the metric");
        }

        _logger.LogInformation("Updated metric with id: {metricId}", metric.Id);
    }

    public async Task DeleteMetric(int metricId)
    {
        bool isDeleted = await _metricRepository.DeleteMetric(metricId);
        if (!isDeleted)
        {
            throw new EntityDeleteException("Couldn't delete the metric");
        }

        _logger.LogInformation("Deleted metric with id: {metricId}", metricId);
    }

    public async Task<MetricResponse> GetMetricByResourceId(int resourceId)
    {
        var metric = await _metricRepository.ReadMetricByResourceId(resourceId);
        if (metric == null)
        {
            throw new NotFoundApplicationException("Metric not found");
        }

        return _mapper.Map<MetricResponse>(metric);
    }

    public async Task<IEnumerable<MetricResponse>> GetAllMetricsByResourceId(int resourceId)
    {
        var metrics = await _metricRepository.ReadAllMetricValuesForResource(resourceId);
        var metricsResponses = metrics.Select(i => _mapper.Map<MetricResponse>(i));
        return metricsResponses;
    }

    public async Task<IEnumerable<MetricResponse>> GetAll()
    {
        var metrics = await _metricRepository.ReadAll();
        var metricsResponses = metrics.Select(i => _mapper.Map<MetricResponse>(i));
        return metricsResponses;
    }
}
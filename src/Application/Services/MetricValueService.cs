using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.MetricRepository;
using Infrastructure.Repositories.MetricValueRepository;
using Application.Requests;
using Application.Responses;
using Infrastructure.Repositories.ResourceRepository;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class MetricValueService : IMetricValueService
{
    private readonly IMetricValueRepository _metricValueRepository;
    private readonly IMapper _mapper;
    private readonly IMetricRepository _metricRepository;
    private readonly IResourceRepository _resourceRepository;
    private readonly ILogger<MetricValueService> _logger;

    public MetricValueService(IMetricValueRepository metricValueRepository, IMapper mapper,
        IMetricRepository metricRepository, IResourceRepository resourceRepository, ILogger<MetricValueService> logger)
    {
        _metricValueRepository = metricValueRepository;
        _mapper = mapper;
        _metricRepository = metricRepository;
        _resourceRepository = resourceRepository;
        _logger = logger;
    }

    public async Task<int> AddMetricValue(CreateMetricValueRequest request)
    {
        var metricValue = _mapper.Map<MetricValue>(request);
        var metricValueId = await _metricValueRepository.CreateMetricValue(metricValue);
        _logger.LogInformation("Created metricValue with id: {metricValueId}", metricValueId);
        return metricValueId;
    }

    public async Task<IEnumerable<MetricValueResponse>> GetAllMetricValuesForResource(int resourceId)
    {
        if (await _resourceRepository.ReadByResourceId(resourceId) == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        var metrics = await _metricRepository.ReadAllMetricValuesForResource(resourceId);
        var metricsIds = metrics.Select(i => i!.Id);
        var metricValues = await _metricValueRepository.ReadAllMetricValuesForMetricsId(metricsIds);
        var metricValuesResponse = metricValues.Select(i => _mapper.Map<MetricValueResponse>(i));
        return metricValuesResponse;
    }

    public async Task<MetricValueResponse> GetMetricValue(int metricValueId)
    {
        var metricValue = await _metricValueRepository.ReadMetricValueId(metricValueId);
        if (metricValue == null)
        {
            throw new NotFoundApplicationException("MetricValue not found");
        }

        return _mapper.Map<MetricValueResponse>(metricValue);
    }
}
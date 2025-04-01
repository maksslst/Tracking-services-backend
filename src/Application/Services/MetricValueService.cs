using Application.DTOs.Mappings;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.MetricRepository;
using Infrastructure.Repositories.MetricValueRepository;
using Application.Requests;
using Application.Responses;
using Infrastructure.Repositories.ResourceRepository;

namespace Application.Services;

public class MetricValueService : IMetricValueService
{
    private readonly IMetricValueRepository _metricValueRepository;
    private readonly IMapper _mapper;
    private readonly IMetricRepository _metricRepository;
    private readonly IResourceRepository _resourceRepository;

    public MetricValueService(IMetricValueRepository metricValueRepository, IMapper mapper,
        IMetricRepository metricRepository, IResourceRepository resourceRepository)
    {
        _metricValueRepository = metricValueRepository;
        _mapper = mapper;
        _metricRepository = metricRepository;
        _resourceRepository = resourceRepository;
    }

    public async Task<MetricValue?> AddMetricValue(CreateMetricValueRequest request)
    {
        if (await _metricRepository.ReadMetricId(request.MetricId) == null)
        {
            throw new NotFoundApplicationException("Metric not found");
        }

        var metricValue = new MetricValue()
        {
            MetricId = request.MetricId,
            Value = request.Value
        };

        await _metricValueRepository.CreateMetricValue(metricValue);
        return metricValue;
    }

    public async Task<IEnumerable<MetricValueResponse?>> GetAllMetricValuesForResource(int resourceId)
    {
        if (await _resourceRepository.ReadByResourceId(resourceId) == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        var metrics = await _metricRepository.ReadAllMetricValuesForResource(resourceId);
        if (metrics == null || metrics.Count() == 0)
        {
            throw new NotFoundApplicationException("Metrics not found");
        }

        var metricsId = metrics.Select(i => i.Id);
        var metricValues = await _metricValueRepository.ReadAllMetricValuesForMetricsId(metricsId);
        var metricValuesResponse = metricValues.Select(i => _mapper.Map<MetricValueResponse>(i));
        return metricValuesResponse;
    }

    public async Task<MetricValueResponse?> GetMetricValue(int metricValueId)
    {
        var metricValue = await _metricValueRepository.ReadMetricValueId(metricValueId);
        if (metricValue == null)
        {
            throw new NotFoundApplicationException("MetricValue not found");
        }

        return _mapper.Map<MetricValueResponse>(metricValue);
    }
}
using Application.DTOs.Mappings;
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

    public async Task<Metric?> AddMetric(CreateMetricRequest request)
    {
        if (await _resourceRepository.ReadByResourceId(request.ResourceId) == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        var metric = new Metric()
        {
            ResourceId = request.ResourceId,
            Name = request.Name,
            Unit = request.Unit
        };

        await _metricRepository.CreateMetric(metric);
        return metric;
    }

    public async Task<bool> UpdateMetric(UpdateMetricRequest request)
    {
        if (await _metricRepository.ReadMetricId(request.MetricId) == null)
        {
            throw new NotFoundApplicationException("Metric not found");
        }

        if (_resourceRepository.ReadByResourceId(request.ResourceId).Result == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        var metric = new Metric()
        {
            Id = request.MetricId,
            Name = request.Name,
            Unit = request.Unit,
            ResourceId = request.ResourceId
        };

        return await _metricRepository.UpdateMetric(metric);
    }

    public async Task<bool> DeleteMetric(int metricId)
    {
        if (await _metricRepository.ReadMetricId(metricId) == null)
        {
            throw new NotFoundApplicationException("Metric not found");
        }

        return await _metricRepository.DeleteMetric(metricId);
    }

    public async Task<MetricResponse?> GetMetricByResourceId(int serviceId)
    {
        var metric = await _metricRepository.ReadMetricByResourceId(serviceId);
        if (metric == null)
        {
            throw new NotFoundApplicationException("Metric not found");
        }

        var metricResponse = new MetricResponse()
        {
            ResourceId = metric.ResourceId,
            Name = metric.Name,
            Unit = metric.Unit,
            Created = metric.Created
        };

        return metricResponse;
    }

    public async Task<IEnumerable<MetricResponse?>> GetAllMetricsByServiceId(int serviceId)
    {
        var metrics = await _metricRepository.ReadAllMetricValuesForResource(serviceId);
        if (metrics == null || metrics.Count() == 0)
        {
            throw new NotFoundApplicationException("Metrics not found");
        }

        var metricsResponse = new List<MetricResponse>();
        foreach (var metric in metrics)
        {
            metricsResponse.Add(new MetricResponse()
            {
                ResourceId = metric.ResourceId,
                Name = metric.Name,
                Unit = metric.Unit,
                Created = metric.Created,
            });
        }

        return metricsResponse;
    }

    public async Task<IEnumerable<MetricResponse?>> GetAll()
    {
        var metrics = await _metricRepository.ReadAll();
        if (metrics == null || metrics.Count() == 0)
        {
            throw new NotFoundApplicationException("Metrics not found");
        }

        var metricsResponse = new List<MetricResponse>();
        foreach (var metric in metrics)
        {
            metricsResponse.Add(new MetricResponse()
            {
                ResourceId = metric.ResourceId,
                Name = metric.Name,
                Unit = metric.Unit,
                Created = metric.Created,
            });
        }

        return metricsResponse;
    }
}
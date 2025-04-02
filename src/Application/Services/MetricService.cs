using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.MetricRepository;
using Infrastructure.Repositories.ResourceRepository;
using Application.Requests;
using Application.Responses;
using Npgsql;

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
        try
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

            return await _metricRepository.CreateMetric(metric);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't add a metric");
        }
    }

    public async Task<bool> UpdateMetric(UpdateMetricRequest request)
    {
        try
        {
            var metric = await _metricRepository.ReadMetricId(request.MetricId);
            if (metric == null)
            {
                throw new NotFoundApplicationException("Metric not found");
            }

            var metricResourse = await _resourceRepository.ReadByResourceId(request.ResourceId);
            if (metricResourse == null)
            {
                throw new NotFoundApplicationException("Resource not found");
            }

            metric.Name = request.Name;
            metric.Unit = request.Unit;
            metric.ResourceId = request.ResourceId;

            return await _metricRepository.UpdateMetric(metric);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't update a metric");
        }
    }

    public async Task<bool> DeleteMetric(int metricId)
    {
        try
        {
            bool isDeleted = await _metricRepository.DeleteMetric(metricId);
            if (!isDeleted)
            {
                throw new NotFoundApplicationException("Metric not found");
            }

            return true;
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't delete a metric");
        }
    }

    public async Task<MetricResponse> GetMetricByResourceId(int serviceId)
    {
        try
        {
            var metric = await _metricRepository.ReadMetricByResourceId(serviceId);
            if (metric == null)
            {
                throw new NotFoundApplicationException("Metric not found");
            }

            return _mapper.Map<MetricResponse>(metric);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't find the metric");
        }
    }

    public async Task<IEnumerable<MetricResponse>> GetAllMetricsByServiceId(int serviceId)
    {
        try
        {
            var metrics = await _metricRepository.ReadAllMetricValuesForResource(serviceId);
            if (metrics == null || metrics.Count() == 0)
            {
                throw new NotFoundApplicationException("Metrics not found");
            }

            var metricsResponses = metrics.Select(i => _mapper.Map<MetricResponse>(i));
            return metricsResponses;
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't find the metrics");
        }
    }

    public async Task<IEnumerable<MetricResponse>> GetAll()
    {
        try
        {
            var metrics = await _metricRepository.ReadAll();
            if (metrics == null || metrics.Count() == 0)
            {
                throw new NotFoundApplicationException("Metrics not found");
            }

            var metricsResponses = metrics.Select(i => _mapper.Map<MetricResponse>(i));
            return metricsResponses;
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't find the metrics");
        }
    }
}
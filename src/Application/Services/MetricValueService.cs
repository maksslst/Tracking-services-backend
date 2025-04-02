using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.MetricRepository;
using Infrastructure.Repositories.MetricValueRepository;
using Application.Requests;
using Application.Responses;
using Infrastructure.Repositories.ResourceRepository;
using Npgsql;

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

    public async Task<int> AddMetricValue(CreateMetricValueRequest request)
    {
        try
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

            return await _metricValueRepository.CreateMetricValue(metricValue);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't add value");
        }
    }

    public async Task<IEnumerable<MetricValueResponse>> GetAllMetricValuesForResource(int resourceId)
    {
        try
        {
            if (await _resourceRepository.ReadByResourceId(resourceId) == null)
            {
                throw new NotFoundApplicationException("Resource not found");
            }

            var metrics = await _metricRepository.ReadAllMetricValuesForResource(resourceId);
            if (metrics == null || metrics.Count() == 0)
            {
                return new List<MetricValueResponse>();
            }

            var metricsId = metrics.Select(i => i.Id);
            var metricValues = await _metricValueRepository.ReadAllMetricValuesForMetricsId(metricsId);
            var metricValuesResponse = metricValues.Select(i => _mapper.Map<MetricValueResponse>(i));
            return metricValuesResponse;
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't get values for resource");
        }
    }

    public async Task<MetricValueResponse> GetMetricValue(int metricValueId)
    {
        try
        {
            var metricValue = await _metricValueRepository.ReadMetricValueId(metricValueId);
            if (metricValue == null)
            {
                throw new NotFoundApplicationException("MetricValue not found");
            }

            return _mapper.Map<MetricValueResponse>(metricValue);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't get value");
        }
    }
}
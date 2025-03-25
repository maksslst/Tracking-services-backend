using Application.DTOs.Mappings;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.MetricRepository;
using Infrastructure.Repositories.ResourceRepository;

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

    public async Task<Metric?> AddMetric(MetricDto metricDto)
    {
        if (await _resourceRepository.ReadByResourceId(metricDto.ResourceId) == null)
        {
            return null;
        }

        var mappedMetric = _mapper.Map<Metric>(metricDto);
        if (mappedMetric != null)
        {
            await _metricRepository.CreateMetric(mappedMetric);
            return mappedMetric;
        }

        return null;
    }

    public async Task<bool> UpdateMetric(MetricDto metricDto)
    {
        var mappedMetric = _mapper.Map<Metric>(metricDto);
        if (mappedMetric == null)
        {
            return false;
        }

        if (_resourceRepository.ReadByResourceId(metricDto.ResourceId).Result == null)
        {
            return false;
        }

        return await _metricRepository.UpdateMetric(mappedMetric);
    }

    public async Task<bool> DeleteMetric(int metricId)
    {
        return await _metricRepository.DeleteMetric(metricId);
    }

    public async Task<MetricDto?> GetMetricByResourceId(int serviceId)
    {
        var metric = await _metricRepository.ReadMetricByResourceId(serviceId);
        var mappedMetric = _mapper.Map<MetricDto>(metric);
        return mappedMetric;
    }

    public async Task<IEnumerable<MetricDto?>> GetAllMetricsByServiceId(int serviceId)
    {
        var metrics = await _metricRepository.ReadAllMetricValuesForResource(serviceId);
        var mappedMetrics = metrics.Select(i => _mapper.Map<MetricDto>(i));
        return mappedMetrics;
    }

    public async Task<IEnumerable<MetricDto?>> GetAll()
    {
        var metrics = await _metricRepository.ReadAll();
        var mappedMetrics = metrics.Select(i => _mapper.Map<MetricDto>(i));
        return mappedMetrics;
    }
}
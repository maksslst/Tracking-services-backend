using Application.DTOs.Mappings;
using Domain.Entities;
using Infrastructure.Repositories;
using AutoMapper;

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
        Metric mappedMetric = _mapper.Map<Metric>(metricDto);
        if (mappedMetric != null)
        {
            await _metricRepository.CreateMetric(mappedMetric);
            return mappedMetric;
        }

        return null;
    }

    public async Task<bool> UpdateMetric(MetricDto metricDto)
    {
        Metric mappedMetric = _mapper.Map<Metric>(metricDto);
        if (mappedMetric == null)
        {
            return false;
        }

        if (_resourceRepository.ReadByServiceId(metricDto.ServiceId).Result == null)
        {
            return false;    
        }
        
        return await _metricRepository.UpdateMetric(mappedMetric);
    }

    public async Task<bool> DeleteMetric(int metricId)
    {
        return await _metricRepository.DeleteMetric(metricId);
    }

    public async Task<MetricDto?> GetMetricService(int serviceId)
    {
        Metric? metric = await _metricRepository.ReadMetricServiceId(serviceId);
        MetricDto mappedMetric = _mapper.Map<MetricDto>(metric);
        return mappedMetric;
    }

    public async Task<IEnumerable<MetricDto?>> GetAllMetricsService(int serviceId)
    {
        IEnumerable<Metric?> metrics = await _metricRepository.ReadAllMetricServiceId(serviceId);
        IEnumerable<MetricDto> mappedMetrics = metrics.Select(i => _mapper.Map<MetricDto>(i));
        return mappedMetrics;
    }

    public async Task<IEnumerable<MetricDto?>> GetAll()
    {
        IEnumerable<Metric?> metrics = await _metricRepository.ReadAll();
        IEnumerable<MetricDto> mappedMetrics = metrics.Select(i => _mapper.Map<MetricDto>(i));
        return mappedMetrics;
    }
}
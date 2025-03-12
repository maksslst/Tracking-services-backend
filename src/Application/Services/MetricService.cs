using Application.DTOs.Mappings;
using Domain.Entities;
using Infrastructure.Repositories;
using AutoMapper;

namespace Application.Services;

public class MetricService : IMetricService
{
    private readonly IMetricRepository _metricRepository;
    private readonly IMapper _mapper;

    public MetricService(IMetricRepository repository, IMapper mapper)
    {
        _metricRepository = repository;
        _mapper = mapper;
    }

    public async Task AddMetric(MetricDto metricDto)
    {
        Metric mappedMetric = _mapper.Map<Metric>(metricDto);
        if (mappedMetric != null)
        {
            await _metricRepository.CreateMetric(mappedMetric);
        }
    }

    public async Task<bool> UpdateMetric(MetricDto metricDto)
    {
        Metric mappedMetric = _mapper.Map<Metric>(metricDto);
        return await _metricRepository.UpdateMetric(mappedMetric);
    }

    public async Task<bool> DeleteMetric(int metricId)
    {
        return await _metricRepository.DeleteMetric(metricId);
    }

    public async Task<MetricDto?> GetMetricServiceId(int serviceId)
    {
        Metric? metric = await _metricRepository.ReadMetricServiceId(serviceId);
        MetricDto mappedMetric = _mapper.Map<MetricDto>(metric);
        return mappedMetric;
    }

    public async Task<List<MetricDto?>> GetAllMetricServiceId(int serviceId)
    {
        List<Metric?> metrics = await _metricRepository.ReadAllMetricServiceId(serviceId);
        List<MetricDto> mappedMetrics = metrics.Select(i => _mapper.Map<MetricDto>(i)).ToList();
        return mappedMetrics;
    }

    public async Task<List<MetricDto?>> GetAll()
    {
        List<Metric?> metrics = await _metricRepository.ReadAll();
        List<MetricDto> mappedMetrics = metrics.Select(i => _mapper.Map<MetricDto>(i)).ToList();
        return mappedMetrics;
    }
}
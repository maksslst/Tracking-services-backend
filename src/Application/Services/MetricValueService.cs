using Application.DTOs.Mappings;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.MetricRepository;
using Infrastructure.Repositories.MetricValueRepository;

namespace Application.Services;

public class MetricValueService : IMetricValueService
{
    private readonly IMetricValueRepository _metricValueRepository;
    private readonly IMapper _mapper;
    private readonly IMetricRepository _metricRepository;

    public MetricValueService(IMetricValueRepository metricValueRepository, IMapper mapper,
        IMetricRepository metricRepository)
    {
        _metricValueRepository = metricValueRepository;
        _mapper = mapper;
        _metricRepository = metricRepository;
    }

    public async Task<MetricValue?> AddMetricValue(MetricValueDto metricValueDto)
    {
        if (await _metricRepository.ReadMetricId(metricValueDto.MetricId) == null)
        {
            return null;
        }

        var mappedMetricValue = _mapper.Map<MetricValue>(metricValueDto);
        if (mappedMetricValue != null)
        {
            await _metricValueRepository.CreateMetricValue(mappedMetricValue);
            return mappedMetricValue;
        }

        return null;
    }

    public async Task<IEnumerable<MetricValueDto?>> GetAllMetricValuesForResource(int resourceId)
    {
        var metrics = await _metricRepository.ReadAllMetricValuesForResource(resourceId);
        if (metrics == null)
        {
            return null;
        }

        var metricsId = metrics.Select(i => i.Id);
        var metricValue = await _metricValueRepository.ReadAllMetricValuesForMetricsId(metricsId);

        var mappedMetricValues = metricValue.Select(i => _mapper.Map<MetricValueDto>(i));
        return mappedMetricValues;
    }

    public async Task<MetricValueDto?> GetMetricValue(int metricValueId)
    {
        var metricValue = await _metricValueRepository.ReadMetricValueId(metricValueId);
        var mappedMetricValue = _mapper.Map<MetricValueDto>(metricValue);
        return mappedMetricValue;
    }
}
using Application.DTOs.Mappings;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Services;

public class MetricValueService : IMetricValueService
{
    private readonly IMetricValueRepository _metricValueRepository;
    private readonly IMapper _mapper;

    public MetricValueService(IMetricValueRepository metricValueRepository, IMapper mapper)
    {
        _metricValueRepository = metricValueRepository;
        _mapper = mapper;
    }

    public async Task AddMetricValue(MetricValueDto metricValueDto)
    {
        MetricValue mappedMetricValue = _mapper.Map<MetricValue>(metricValueDto);
        if (mappedMetricValue != null)
        {
            await _metricValueRepository.CreateMetricValue(mappedMetricValue);
        }
    }

    public async Task<List<MetricValueDto?>> GetAllMetricValuesServiceId(int serviceId)
    {
        List<MetricValue?> metricValues = await _metricValueRepository.ReadAllMetricValuesServiceId(serviceId);
        List<MetricValueDto> mappedMetricValues = metricValues.Select(i => _mapper.Map<MetricValueDto>(i)).ToList();
        return mappedMetricValues;
    }
}
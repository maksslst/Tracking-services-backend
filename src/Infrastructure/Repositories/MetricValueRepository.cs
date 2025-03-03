using Domain.Entities;

namespace Infrastructure.Repositories;

public class MetricValueRepository : IMetricValueRepository
{
    private List<MetricValue> _metricValues;
    private readonly IMetricRepository _metricRepository;

    public MetricValueRepository(IMetricRepository metricRepository)
    {
        _metricValues = new List<MetricValue>();
        _metricRepository = metricRepository;
    }
    
    public Task CreateMetricValue(MetricValue metricValue)
    {
        _metricValues.Add(metricValue);
        return Task.CompletedTask;
    }

    public Task<List<MetricValue?>> ReadAllMetricValuesServiceId(int serviceId)
    {
        var metrics = _metricRepository.ReadAllMetricServiceId(serviceId).Result;
        if (metrics == null)
        {
            return Task.FromResult<List<MetricValue?>>(null);
        }

        List<MetricValue?> metricsValues = new List<MetricValue?>();
        foreach (var metric in metrics)
        {
            var metricValue = _metricValues.FindAll(i => i.MetricId == metric.Id);
            if(metricValue.Count > 0)
            {
                metricsValues.AddRange(metricValue);
            }
        }
        
        return Task.FromResult(metricsValues);
    }
}
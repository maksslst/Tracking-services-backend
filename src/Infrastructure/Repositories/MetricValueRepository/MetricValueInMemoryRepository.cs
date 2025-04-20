using System.Diagnostics.CodeAnalysis;
using Bogus;
using Domain.Entities;

namespace Infrastructure.Repositories.MetricValueRepository;

[ExcludeFromCodeCoverage]
public class MetricValueInMemoryRepository : IMetricValueRepository
{
    private List<MetricValue> _metricValues;

    public MetricValueInMemoryRepository()
    {
        _metricValues = new List<MetricValue>();
        DataGeneration();
    }

    public Task<int> CreateMetricValue(MetricValue metricValue)
    {
        _metricValues.Add(metricValue);
        return Task.FromResult(metricValue.Id);
    }

    public Task<MetricValue?> ReadMetricValueId(int metricValueId)
    {
        MetricValue? metricValue = _metricValues.Find(i => i.Id == metricValueId);
        if (metricValue == null)
        {
            return Task.FromResult<MetricValue?>(null);
        }
        
        return Task.FromResult(metricValue);
    }

    public Task<IEnumerable<MetricValue?>> ReadAllMetricValuesForMetricsId(IEnumerable<int> metricsId)
    {
        IEnumerable<MetricValue?> metricValues = _metricValues.Where(i => metricsId.Contains(i.MetricId));

        return Task.FromResult(metricValues);
    }

    private void DataGeneration()
    {
        var faker = new Faker();
        Random random = new Random();
        for (int i = 0; i < 10; i++)
        {
            MetricValue metricValue = new MetricValue()
            {
                Id = i + 1,
                MetricId = random.Next(1, 5),
                Value = faker.Random.Double(),
            };
            _metricValues.Add(metricValue);
        }
    }
}
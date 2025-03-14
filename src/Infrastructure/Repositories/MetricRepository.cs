using Domain.Entities;
using Bogus;

namespace Infrastructure.Repositories;

public class MetricRepository : IMetricRepository
{
    private List<Metric> _metrics;

    public MetricRepository()
    {
        _metrics = new List<Metric>();
        DataGeneration();
    }
    
    public Task<Metric> CreateMetric(Metric metric)
    {
        _metrics.Add(metric);
        return Task.FromResult(metric);
    }

    public Task<bool> UpdateMetric(Metric metric)
    {
        var metricToUpdate = _metrics.Find(i => i.Id == metric.Id);
        if (metricToUpdate == null)
        {
            return Task.FromResult(false);
        }
        
        metricToUpdate.ServiceId = metric.ServiceId;
        metricToUpdate.Resource = metric.Resource;
        metricToUpdate.Name = metric.Name;
        metricToUpdate.Unit = metric.Unit;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteMetric(int metricId)
    {
        var metricToDelete = _metrics.Find(i => i.Id == metricId);
        if (metricToDelete == null)
        {
            return Task.FromResult(false);
        }
        
        _metrics.Remove(metricToDelete);
        return Task.FromResult(true);
    }

    public Task<Metric?> ReadMetricServiceId(int serviceId)
    {
        var metric = _metrics.Find(i => i.ServiceId == serviceId);
        if (metric == null)
        {
            return Task.FromResult<Metric?>(null);
        }
        
        return Task.FromResult(metric);
    }

    public Task<IEnumerable<Metric?>> ReadAllMetricForServiceId(int serviceId)
    {
        var metric = _metrics.FindAll(i => i.ServiceId == serviceId);
        return Task.FromResult<IEnumerable<Metric?>>(metric);
    }

    public Task<IEnumerable<Metric?>> ReadAll()
    {
        return Task.FromResult<IEnumerable<Metric?>>(_metrics);
    }

    private void DataGeneration()
    {
        var faker = new Faker();
        Random random = new Random();
        for (int i = 0; i < 5; i++)
        {
            Metric metric = new Metric()
            {
                Id = i +1,
                Name = "Проверка доступности ping",
                ServiceId = random.Next(1,3),
                Created = DateTime.Now,
                Unit = "мс"
            };
            
            _metrics.Add(metric);
        }
    }
}
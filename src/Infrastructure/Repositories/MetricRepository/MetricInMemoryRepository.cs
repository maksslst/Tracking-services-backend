using System.Diagnostics.CodeAnalysis;
using Domain.Entities;

namespace Infrastructure.Repositories.MetricRepository;

[ExcludeFromCodeCoverage]
public class MetricInMemoryRepository : IMetricRepository
{
    private List<Metric> _metrics;

    public MetricInMemoryRepository()
    {
        _metrics = new List<Metric>();
        DataGeneration();
    }

    public Task<int> CreateMetric(Metric metric)
    {
        _metrics.Add(metric);
        return Task.FromResult(metric.Id);
    }

    public Task<bool> UpdateMetric(Metric metric)
    {
        var metricToUpdate = _metrics.Find(i => i.Id == metric.Id);
        if (metricToUpdate == null)
        {
            return Task.FromResult(false);
        }

        metricToUpdate.ResourceId = metric.ResourceId;
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

    public Task<Metric?> ReadMetricId(int metricId)
    {
        var metric = _metrics.Find(i => i.Id == metricId);
        if (metric == null)
        {
            return Task.FromResult<Metric?>(null);
        }

        return Task.FromResult(metric)!;
    }

    public Task<Metric?> ReadMetricByResourceId(int resourceId)
    {
        var metric = _metrics.Find(i => i.ResourceId == resourceId);
        if (metric == null)
        {
            return Task.FromResult<Metric?>(null);
        }

        return Task.FromResult(metric)!;
    }

    public Task<IEnumerable<Metric?>> ReadAllMetricValuesForResource(int resourceId)
    {
        var metric = _metrics.FindAll(i => i.ResourceId == resourceId);
        return Task.FromResult<IEnumerable<Metric?>>(metric);
    }

    public Task<IEnumerable<Metric?>> ReadAll()
    {
        return Task.FromResult<IEnumerable<Metric?>>(_metrics);
    }

    private void DataGeneration()
    {
        Random random = new Random();
        for (int i = 0; i < 5; i++)
        {
            Metric metric = new Metric()
            {
                Id = i + 1,
                Name = "Проверка доступности ping",
                ResourceId = random.Next(1, 3),
                Created = DateTime.Now,
                Unit = "мс"
            };

            _metrics.Add(metric);
        }
    }
}
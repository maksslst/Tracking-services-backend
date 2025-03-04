using Domain.Entities;

namespace Infrastructure.Repositories;

public class MetricRepository : IMetricRepository
{
    private List<Metric> _metrics;
    private readonly IServiceRepository _serviceRepository;

    public MetricRepository(IServiceRepository serviceRepository)
    {
        _metrics = new List<Metric>();
        _serviceRepository = serviceRepository;
    }
    
    public Task CreateMetric(Metric metric)
    {
        _metrics.Add(metric);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateMetric(Metric metric)
    {
        var metricToUpdate = _metrics.Find(i => i.Id == metric.Id);
        if (metricToUpdate == null)
        {
            return Task.FromResult(false);
        }

        if (_serviceRepository.ReadByServiceId(metric.ServiceId).Result == null)
        {
            return Task.FromResult(false);
        }
        
        metricToUpdate.ServiceId = metric.ServiceId;
        metricToUpdate.Service = metric.Service;
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

    public Task<List<Metric?>> ReadAllMetricServiceId(int serviceId)
    {
        var metric = _metrics.FindAll(i => i.ServiceId == serviceId);
        return Task.FromResult(metric);
    }

    public Task<List<Metric?>> ReadAll()
    {
        return Task.FromResult(_metrics);
    }
}
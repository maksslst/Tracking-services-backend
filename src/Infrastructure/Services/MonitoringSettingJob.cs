using Domain.Entities;
using Infrastructure.Repositories.MetricRepository;
using Infrastructure.Repositories.MetricValueRepository;
using Infrastructure.Repositories.MonitoringSettingRepository;
using Quartz;

namespace Infrastructure.Services;

public class MonitoringSettingJob : IJob
{
    private readonly IMonitoringSettingRepository _monitoringSettingRepository;
    private readonly IMetricRepository _metricRepository;
    private readonly IMetricValueRepository _metricValueRepository;

    public MonitoringSettingJob(IMonitoringSettingRepository monitoringSettingRepository,
        IMetricRepository metricRepository, IMetricValueRepository metricValueRepository)
    {
        _monitoringSettingRepository = monitoringSettingRepository;
        _metricRepository = metricRepository;
        _metricValueRepository = metricValueRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var monitoringSettings = _monitoringSettingRepository.ReadAll().Result;
        foreach (var monitoringSetting in monitoringSettings)
        {
            var metrics = _metricRepository.ReadAllMetricValuesForResource(monitoringSetting.ResourceId).Result;
            foreach (var metric in metrics)
            {
                MetricValue metricValue = new MetricValue
                {
                    Metric = metric,
                    MetricId = metric.Id,
                    // Value = 
                };
                _metricValueRepository.CreateMetricValue(metricValue);
            }
        }
    }
}
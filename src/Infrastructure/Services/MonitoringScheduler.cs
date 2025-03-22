using Domain.Entities;
using Infrastructure.Repositories.MonitoringSettingRepository;
using Quartz;
using Quartz.Impl;

namespace Infrastructure.Services;

public class MonitoringScheduler
{
    private readonly IMonitoringSettingRepository _monitoringSettingRepository;

    public MonitoringScheduler(IMonitoringSettingRepository monitoringSettingRepository)
    {
        _monitoringSettingRepository = monitoringSettingRepository;
    }

    public async Task StartMonitoring()
    {
        IEnumerable<MonitoringSetting> monitoringSettings = _monitoringSettingRepository.ReadAll().Result;
        foreach (var monitoringSetting in monitoringSettings)
        {
            MonitoringTaskSchedule(monitoringSetting);
        }
    }

    private async Task MonitoringTaskSchedule(MonitoringSetting monitoringSetting)
    {
        IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        IJobDetail job = JobBuilder.Create<MonitoringSettingJob>()
            .WithIdentity($"myJob_{monitoringSetting.Id}", "monitoringGroup")
            .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity($"myTrigger{monitoringSetting.Id}", "monitoringGroup")
            .WithCronSchedule(monitoringSetting.CheckInterval)
            .ForJob(job)
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}
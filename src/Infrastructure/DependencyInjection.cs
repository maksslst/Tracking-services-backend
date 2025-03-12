using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Repositories;
using Infrastructure.Services;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ICompanyRepository, CompanyRepository>();
        services.AddSingleton<IMetricRepository, MetricRepository>();
        services.AddSingleton<IMonitoringSettingRepository, MonitoringSettingRepository>();
        services.AddSingleton<IServiceRepository, ServiceRepository>();
        services.AddSingleton<ITaskRepository, TaskRepository>();
        services.AddSingleton<IMetricValueRepository, MetricValueRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<MonitoringScheduler>();

        return services;
    }
}
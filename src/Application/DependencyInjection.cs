using Application.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Application.Services;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddTransient<ICompanyService, CompanyService>();
        services.AddTransient<IMonitoringSettingService, MonitoringSettingService>();
        services.AddTransient<IResourceService, ResourceResourceService>();
        services.AddTransient<ITaskService, TaskService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IMetricService, MetricService>();
        services.AddTransient<IMetricValueService, MetricValueService>();

        return services;
    }
}
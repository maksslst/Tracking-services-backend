using System.Reflection;
using Application.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Application.Services;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IMonitoringSettingService, MonitoringSettingService>();
        services.AddScoped<IResourceService, ResourceService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMetricService, MetricService>();
        services.AddScoped<IMetricValueService, MetricValueService>();

        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
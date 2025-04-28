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
        services.AddTransient<ICompanyService, CompanyService>();
        services.AddTransient<IMonitoringSettingService, MonitoringSettingService>();
        services.AddTransient<IResourceService, ResourceService>();
        services.AddTransient<ITaskService, TaskService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IMetricService, MetricService>();
        services.AddTransient<IMetricValueService, MetricValueService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IPasswordHasher, BCryptHasher>();

        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
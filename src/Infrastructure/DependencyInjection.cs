using FluentMigrator.Runner;
using Infrastructure.Repositories.CompanyRepository;
using Infrastructure.Repositories.MetricRepository;
using Infrastructure.Repositories.MetricValueRepository;
using Infrastructure.Repositories.MonitoringSettingRepository;
using Infrastructure.Repositories.ResourceRepository;
using Infrastructure.Repositories.TaskRepository;
using Infrastructure.Repositories.UserRepository;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Reflection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("PostgresDB");
            return new NpgsqlDataSourceBuilder(connectionString).Build();
        });

        services.AddScoped(sp =>
        {
            var dataSource = sp.GetRequiredService<NpgsqlDataSource>();
            return dataSource.CreateConnection();
        });

        services.AddTransient<ICompanyRepository, CompanyPostgresRepository>();
        services.AddTransient<IMetricRepository, MetricPostgresRepository>();
        services.AddTransient<IMonitoringSettingRepository, MonitoringSettingPostgresRepository>();
        services.AddTransient<IResourceRepository, ResourcePostgresRepository>();
        services.AddTransient<ITaskRepository, TaskPostgresRepository>();
        services.AddTransient<IMetricValueRepository, MetricValuePostgresRepository>();
        services.AddTransient<IUserRepository, UserPostgresRepository>();
        services.AddTransient<MonitoringScheduler>();

        services
            .AddFluentMigratorCore()
            .ConfigureRunner(
                rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(sp =>
                    {
                        var config = sp.GetRequiredService<IConfiguration>();
                        return config.GetConnectionString("PostgresDB");
                    })
                    .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        services.AddScoped<Database.MigrationRunner>();

        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        return services;
    }
}
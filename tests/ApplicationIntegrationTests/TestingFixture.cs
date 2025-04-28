using System.Reflection;
using Application;
using Bogus;
using Domain.Entities;
using Domain.Enums;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Infrastructure;
using Infrastructure.Repositories.CompanyRepository;
using Infrastructure.Repositories.MetricRepository;
using Infrastructure.Repositories.MetricValueRepository;
using Infrastructure.Repositories.MonitoringSettingRepository;
using Infrastructure.Repositories.ResourceRepository;
using Infrastructure.Repositories.TaskRepository;
using Infrastructure.Repositories.UserRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Respawn;
using MigrationRunner = Infrastructure.Database.MigrationRunner;
using TaskStatus = Domain.Enums.TaskStatus;

namespace ApplicationIntegrationTests;

public sealed class TestingFixture : IAsyncLifetime
{
    private readonly Faker _faker;
    private Respawner _respawner = null!;

    public TestingFixture()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((config) => { config.AddJsonFile("appsettings.json"); })
            .ConfigureServices((context, services) =>
            {
                services.AddInfrastructure();
                services.AddApplication();
                var connectionString = context.Configuration.GetConnectionString("PostgresDBIntegration");
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new ApplicationException("PostgresDBIntegration connection string is empty");

                services.AddSingleton(_ => new NpgsqlDataSourceBuilder(connectionString).Build());

                services.AddTransient(sp =>
                {
                    var dataSource = sp.GetRequiredService<NpgsqlDataSource>();
                    return dataSource.CreateConnection();
                });

                services
                    .AddFluentMigratorCore()
                    .ConfigureRunner(rb => rb
                        .AddPostgres()
                        .WithGlobalConnectionString(connectionString)
                        .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
                    .Configure<SelectingProcessorAccessorOptions>(options => { options.ProcessorId = "Postgres"; });
            })
            .Build();

        ServiceProvider = host.Services;

        _faker = new Faker();
    }

    public IServiceProvider ServiceProvider { get; }

    public async Task InitializeAsync()
    {
        using var scope = ServiceProvider.CreateScope();
        var connection = scope.ServiceProvider.GetRequiredService<NpgsqlConnection>();
        await connection.OpenAsync();

        var migrationRunner = scope.ServiceProvider.GetRequiredService<MigrationRunner>();
        migrationRunner.Run();

        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = ["VersionInfo"]
        });
    }

    public async Task<User> CreateUser(int companyId)
    {
        using var scope = ServiceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var userId = await userRepository.CreateUser(new User
        {
            FirstName = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName(),
            Email = _faker.Random.Word() + _faker.Random.Word() + "@gmail.com",
            Username = $"user_{Guid.NewGuid().ToString("N")}",
            CompanyId = companyId,
            PasswordHash = _faker.Random.Word()
        });

        var user = await userRepository.ReadById(userId);

        if (user == null)
            throw new Exception("Can't create user");

        return user;
    }

    public async Task<Company> CreateCompany()
    {
        using var scope = ServiceProvider.CreateScope();
        var companyRepository = scope.ServiceProvider.GetRequiredService<ICompanyRepository>();

        var companyId = await companyRepository.CreateCompany(new Company
        {
            CompanyName = _faker.Company.CompanyName()
        });

        var company = await companyRepository.ReadByCompanyId(companyId);

        if (company == null)
            throw new Exception("Can't create company");

        return company;
    }

    public async Task<Resource> CreateResource(int companyId)
    {
        using var scope = ServiceProvider.CreateScope();
        var resourceRepository = scope.ServiceProvider.GetRequiredService<IResourceRepository>();

        var resourceId = await resourceRepository.CreateResource(new Resource
        {
            Name = _faker.Random.Word(),
            CompanyId = companyId,
            Type = _faker.Random.Word(),
            Source = _faker.Internet.Url(),
            Status = ResourceStatus.Active
        });

        var resource = await resourceRepository.ReadByResourceId(resourceId);

        if (resource == null)
            throw new Exception("Can't create resource");

        return resource;
    }

    public async Task<Metric> CreateMetric(int resourceId)
    {
        using var scope = ServiceProvider.CreateScope();
        var metricRepository = scope.ServiceProvider.GetRequiredService<IMetricRepository>();

        var metric = new Metric
        {
            Name = _faker.Random.Word(),
            Unit = "мс",
            Created = DateTime.Now,
            ResourceId = resourceId
        };

        var metricId = await metricRepository.CreateMetric(metric);
        var createdMetric = await metricRepository.ReadMetricId(metricId);

        if (createdMetric == null)
            throw new Exception("Can't create metric");

        return createdMetric;
    }

    public async Task<MetricValue> CreateMetricValue(int metricId)
    {
        using var scope = ServiceProvider.CreateScope();
        var metricValueRepository = scope.ServiceProvider.GetRequiredService<IMetricValueRepository>();

        var metricValueId = await metricValueRepository.CreateMetricValue(new MetricValue
        {
            MetricId = metricId,
            Value = _faker.Random.Double()
        });

        var metricValue = await metricValueRepository.ReadMetricValueId(metricValueId);

        if (metricValue == null)
            throw new Exception("Can't create metricValue");

        return metricValue;
    }

    public async Task<MonitoringSetting> CreateMonitoringSetting(int resourceId)
    {
        using var scope = ServiceProvider.CreateScope();
        var monitoringSettingRepository = scope.ServiceProvider.GetRequiredService<IMonitoringSettingRepository>();

        await monitoringSettingRepository.CreateSetting(new MonitoringSetting
        {
            ResourceId = resourceId,
            CheckInterval = "0 0/5 * * * ?",
            Mode = true
        });

        var monitoringSetting = await monitoringSettingRepository.ReadByResourceId(resourceId);

        if (monitoringSetting == null)
            throw new Exception("Can't create monitoringSetting");

        return monitoringSetting;
    }

    public async Task<ServiceTask> CreateServiceTask(int resourceId, int assignedUserId, int createdByUserId)
    {
        using var scope = ServiceProvider.CreateScope();
        var serviceTaskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();

        var serviceTaskId = await serviceTaskRepository.CreateTask(new ServiceTask
        {
            ResourceId = resourceId,
            Description = _faker.Random.Word(),
            AssignedUserId = assignedUserId,
            CreatedById = createdByUserId,
            StartTime = DateTime.Now,
            Status = TaskStatus.Opened
        });

        var serviceTask = await serviceTaskRepository.ReadTaskId(serviceTaskId);

        if (serviceTask == null)
            throw new Exception("Can't create serviceTask");

        return serviceTask;
    }

    public async Task DisposeAsync()
    {
        await ResetDatabase();
    }

    private async Task ResetDatabase()
    {
        using var scope = ServiceProvider.CreateScope();
        var connection = scope.ServiceProvider.GetRequiredService<NpgsqlConnection>();
        await connection.OpenAsync();

        await _respawner.ResetAsync(connection);
    }
}
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

    public async Task<User> CreateUser(int companyId = -1)
    {
        using var scope = ServiceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        if (companyId == -1)
        {
            var company = await CreateCompany();
            companyId = company.Id;
        }

        var userId = await userRepository.CreateUser(new User
        {
            FirstName = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName(),
            Email = _faker.Random.Word() + "@gmail.com",
            Username = _faker.Random.Word(),
            CompanyId = companyId
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

    public async Task<Resource> CreateResource(int companyId = -1)
    {
        using var scope = ServiceProvider.CreateScope();
        var resourceRepository = scope.ServiceProvider.GetRequiredService<IResourceRepository>();

        if (companyId == -1)
        {
            var company = await CreateCompany();
            companyId = company.Id;
        }

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

    public async Task<Metric> CreateMetric(int resourceId = -1)
    {
        using var scope = ServiceProvider.CreateScope();
        var metricRepository = scope.ServiceProvider.GetRequiredService<IMetricRepository>();

        if (resourceId == -1)
        {
            var resource = await CreateResource();
            resourceId = resource.Id;
        }

        var metricId = await metricRepository.CreateMetric(new Metric
        {
            Name = _faker.Random.Word(),
            Unit = "мс",
            Created = DateTime.Now,
            ResourceId = resourceId
        });

        var metric = await metricRepository.ReadMetricId(metricId);

        if (metric == null)
            throw new Exception("Can't create resource");

        return metric;
    }

    public async Task<MetricValue> CreateMetricValue(int metricId = -1)
    {
        using var scope = ServiceProvider.CreateScope();
        var metricValueRepository = scope.ServiceProvider.GetRequiredService<IMetricValueRepository>();

        if (metricId == -1)
        {
            var metric = await CreateMetric();
            metricId = metric.Id;
        }

        var metricValueId = await metricValueRepository.CreateMetricValue(new MetricValue
        {
            MetricId = metricId,
            Value = _faker.Random.Double()
        });

        var metricValue = await metricValueRepository.ReadMetricValueId(metricValueId);

        if (metricValue == null)
            throw new Exception("Can't create resource");

        return metricValue;
    }

    public async Task<MonitoringSetting> CreateMonitoringSetting(int resourceId = -1)
    {
        using var scope = ServiceProvider.CreateScope();
        var monitoringSettingRepository = scope.ServiceProvider.GetRequiredService<IMonitoringSettingRepository>();

        if (resourceId == -1)
        {
            var resource = await CreateResource();
            resourceId = resource.Id;
        }

        await monitoringSettingRepository.CreateSetting(new MonitoringSetting
        {
            ResourceId = resourceId,
            CheckInterval = _faker.Random.Word(),
            Mode = true
        });

        var monitoringSetting = await monitoringSettingRepository.ReadByResourceId(resourceId);

        if (monitoringSetting == null)
            throw new Exception("Can't create resource");

        return monitoringSetting;
    }

    public async Task<ServiceTask> CreateServiceTask()
    {
        using var scope = ServiceProvider.CreateScope();
        var serviceTaskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();

        var resource = await CreateResource();
        var createdByUser = await CreateUser();
        var assignedUser = await CreateUser();

        var serviceTaskId = await serviceTaskRepository.CreateTask(new ServiceTask
        {
            ResourceId = resource.Id,
            Description = _faker.Random.Word(),
            AssignedUserId = assignedUser.Id,
            CreatedById = createdByUser.Id,
            StartTime = DateTime.Now,
            Status = TaskStatus.Opened
        });

        var serviceTask = await serviceTaskRepository.ReadTaskId(serviceTaskId);

        if (serviceTask == null)
            throw new Exception("Can't create resource");

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
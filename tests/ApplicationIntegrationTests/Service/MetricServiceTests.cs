using Application.Exceptions;
using Application.Requests;
using Application.Services;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationIntegrationTests.Service;

public class MetricServiceTests : IClassFixture<TestingFixture>
{
    private readonly TestingFixture _fixture;
    private readonly IMetricService _metricService;
    private readonly Faker _faker;

    public MetricServiceTests(TestingFixture fixture)
    {
        _fixture = fixture;
        var scope = _fixture.ServiceProvider.CreateScope();
        _metricService = scope.ServiceProvider.GetRequiredService<IMetricService>();
        _faker = new Faker();
    }

    [Fact]
    public async Task AddMetric_ShouldCreateMetric()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);
        var request = new CreateMetricRequest { ResourceId = resource.Id, Name = _faker.Random.Word(), Unit = "мс" };

        // Act
        var result = await _metricService.AddMetric(request);

        // Assert
        result.Should().BeGreaterThan(0);
        var metric = await _metricService.GetMetricByResourceId(resource.Id);
        metric.Should().NotBeNull();
        metric.Name.Should().Be(request.Name);
        metric.Unit.Should().Be(request.Unit);
        metric.ResourceId.Should().Be(resource.Id);
    }

    [Fact]
    public async Task UpdateMetric_ShouldUpdateMetric()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);
        var metric = await _fixture.CreateMetric(resource.Id);
        var request = new UpdateMetricRequest
        {
            Id = metric.Id, 
            ResourceId = metric.ResourceId, 
            Name = _faker.Random.Word(), 
            Unit = "мс"
        };

        // Act
        await _metricService.UpdateMetric(request);
        var resultingMetric = await _metricService.GetMetricByResourceId(request.ResourceId);

        // Assert
        resultingMetric.Should().NotBeNull();
        resultingMetric.Name.Should().Be(request.Name);
        resultingMetric.Unit.Should().Be(request.Unit);
        resultingMetric.ResourceId.Should().Be(request.ResourceId);
    }

    [Fact]
    public async Task DeleteMetric_ShouldDeleteMetric()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);
        var metric = await _fixture.CreateMetric(resource.Id);

        // Act
       await _metricService.DeleteMetric(metric.Id);

        // Assert
        await _metricService.Invoking(i => i.GetMetricByResourceId(resource.Id))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("Metric not found");
    }

    [Fact]
    public async Task GetMetricByResourceId_ShouldReturnMetric()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);
        var metric = await _fixture.CreateMetric(resource.Id);

        // Act
        var result = await _metricService.GetMetricByResourceId(resource.Id);

        // Assert
        result.Should().NotBeNull();
        result.ResourceId.Should().Be(metric.ResourceId);
        result.Name.Should().Be(metric.Name);
        result.Unit.Should().Be(metric.Unit);
    }

    [Fact]
    public async Task GetAllMetricsByResourceId_ShouldReturnAllMetrics()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);
        await _fixture.CreateMetric(resource.Id);
        await _fixture.CreateMetric(resource.Id);

        // Act
        var result = (await _metricService.GetAllMetricsByResourceId(resource.Id)).ToList();

        // Assert
        result.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllMetrics()
    {
        // Arrange
        var company1 = await _fixture.CreateCompany();
        var resource1 = await _fixture.CreateResource(company1.Id);
        await _fixture.CreateMetric(resource1.Id);
        
        var company2 = await _fixture.CreateCompany();
        var resource2 = await _fixture.CreateResource(company2.Id);
        await _fixture.CreateMetric(resource2.Id);

        // Act
        var result = (await _metricService.GetAll()).ToList();

        // Assert
        result.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
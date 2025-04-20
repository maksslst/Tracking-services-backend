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
        var resource = await _fixture.CreateResource();
        var request = new CreateMetricRequest
            { ResourceId = resource.Id, Name = _faker.Random.Word(), Unit = "мс" };

        // Act
        var result = await _metricService.AddMetric(request);

        // Assert
        result.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task UpdateMetric_ShouldUpdateMetric()
    {
        // Arrange
        var metric = await _fixture.CreateMetric();
        var request = new UpdateMetricRequest
        {
            Id = metric.Id, ResourceId = metric.ResourceId, Name = _faker.Random.Word(), Unit = "мс"
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
        var metric = await _fixture.CreateMetric();

        // Act
        var act = async () => await _metricService.DeleteMetric(metric.Id);

        // Assert
        await act.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task GetMetricByResourceId_ShouldReturnMetric()
    {
        // Arrange
        var metric = await _fixture.CreateMetric();

        // Act
        var result = await _metricService.GetMetricByResourceId(metric.ResourceId);

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
        var resource = await _fixture.CreateResource();
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
        await _fixture.CreateMetric();
        await _fixture.CreateMetric();

        // Act
        var result = (await _metricService.GetAll()).ToList();

        // Assert
        result.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
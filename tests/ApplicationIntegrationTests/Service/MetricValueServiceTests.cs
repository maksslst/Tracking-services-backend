using Application.Requests;
using Application.Services;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationIntegrationTests.Service;

public class MetricValueServiceTests : IClassFixture<TestingFixture>
{
    private readonly TestingFixture _fixture;
    private readonly IMetricValueService _metricValueService;
    private readonly Faker _faker;

    public MetricValueServiceTests(TestingFixture fixture)
    {
        _fixture = fixture;
        var scope = fixture.ServiceProvider.CreateScope();
        _metricValueService = scope.ServiceProvider.GetRequiredService<IMetricValueService>();
        _faker = new Faker();
    }

    [Fact]
    public async Task AddMetricValue_ShouldCreateMetricValue()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);
        var metric = await _fixture.CreateMetric(resource.Id);
        var request = new CreateMetricValueRequest
        {
            MetricId = metric.Id,
            Value = _faker.Random.Double(1, 100)
        };

        // Act
        var metricValueId = await _metricValueService.AddMetricValue(request);
        // Assert
        var createdMetricValue = await _metricValueService.GetMetricValue(metricValueId);
        createdMetricValue.Should().NotBeNull();
        createdMetricValue.MetricId.Should().Be(metric.Id);
        createdMetricValue.Value.Should().Be(request.Value);
    }

    [Fact]
    public async Task GetMetricValue_ShouldReturnMetricValue()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);
        var metric = await _fixture.CreateMetric(resource.Id);
        var metricValue = await _fixture.CreateMetricValue(metric.Id);

        // Act
        var result = await _metricValueService.GetMetricValue(metricValue.Id);

        // Assert
        result.MetricId.Should().Be(metricValue.MetricId);
        result.Value.Should().Be(metricValue.Value);
    }

    [Fact]
    public async Task GetAllMetricValuesForResource_ShouldReturnMetricValues()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);
        var metric = await _fixture.CreateMetric(resource.Id);
        await _fixture.CreateMetricValue(metric.Id);
        await _fixture.CreateMetricValue(metric.Id);

        // Act
        var result = (await _metricValueService.GetAllMetricValuesForResource(resource.Id)).ToList();

        // Assert
        result.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
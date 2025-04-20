using Application.Exceptions;
using Application.Mappings;
using Application.Requests;
using Application.Services;
using AutoMapper;
using Bogus;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Repositories.MetricRepository;
using Infrastructure.Repositories.MetricValueRepository;
using Infrastructure.Repositories.ResourceRepository;
using Moq;
using Microsoft.Extensions.Logging;

namespace ApplicationUnitTests.Services;

public class MetricValueServiceTests
{
    private readonly Mock<IMetricValueRepository> _metricValueRepositoryMock;
    private readonly Mock<IMetricRepository> _metricRepositoryMock;
    private readonly Mock<IResourceRepository> _resourceRepositoryMock;
    private readonly IMetricValueService _metricValueService;
    private readonly Faker _faker;

    public MetricValueServiceTests()
    {
        _metricValueRepositoryMock = new Mock<IMetricValueRepository>();
        _metricRepositoryMock = new Mock<IMetricRepository>();
        _resourceRepositoryMock = new Mock<IResourceRepository>();
        _faker = new Faker();
        var loggerMock = new Mock<ILogger<MetricValueService>>();

        var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = mappingConfig.CreateMapper();

        _metricValueService = new MetricValueService(_metricValueRepositoryMock.Object, mapper,
            _metricRepositoryMock.Object, _resourceRepositoryMock.Object, loggerMock.Object);
    }

    #region AddTests

    [Fact]
    public async Task AddMetricValue_WhenValidRequest_ReturnsMetricValueId()
    {
        // Arrange
        var request = new CreateMetricValueRequest()
        {
            MetricId = _faker.Random.Int(),
            Value = _faker.Random.Double()
        };

        _metricValueRepositoryMock.Setup(i => i.CreateMetricValue(It.IsAny<MetricValue>())).ReturnsAsync(1);

        // Act
        var result = await _metricValueService.AddMetricValue(request);

        // Assert
        result.Should().Be(1);
        _metricValueRepositoryMock.Verify(i => i.CreateMetricValue(It.Is<MetricValue>(m =>
            m.MetricId == request.MetricId &&
            Math.Abs(m.Value - request.Value) < 0.0000001)), Times.Once);
    }

    #endregion

    #region GetTests

    [Fact]
    public async Task GetAllMetricValuesForResource_WhenMetricValuesExists_ReturnsMetricValuesResponse()
    {
        // Arrange
        int resourceId = _faker.Random.Int(1, 100);
        int metricId = _faker.Random.Int(1, 100);

        var resource = new Resource()
        {
            Id = resourceId,
            Name = _faker.Random.String(),
            CompanyId = _faker.Random.Int(1, 100),
            Source = _faker.Random.String(),
            Status = ResourceStatus.Active,
            Type = _faker.Random.String()
        };
        _resourceRepositoryMock.Setup(i => i.ReadByResourceId(resourceId)).ReturnsAsync(resource);

        var metrics = new List<Metric>()
        {
            new Metric()
            {
                Id = metricId,
                Name = _faker.Random.String(),
                ResourceId = resourceId,
                Unit = _faker.Random.String(),
                Created = DateTime.Now
            },
            new Metric()
            {
                Id = metricId,
                Name = _faker.Random.String(),
                ResourceId = resourceId,
                Unit = _faker.Random.String(),
                Created = DateTime.Now
            }
        };

        _metricRepositoryMock.Setup(i => i.ReadAllMetricValuesForResource(resourceId)).ReturnsAsync(metrics);

        var metricValues = new List<MetricValue>()
        {
            new MetricValue()
            {
                Id = _faker.Random.Int(1, 100),
                MetricId = metricId,
                Value = _faker.Random.Double()
            },
            new MetricValue()
            {
                Id = _faker.Random.Int(1, 100),
                MetricId = metricId,
                Value = _faker.Random.Double()
            },
        };

        var metricsIds = metrics.Select(i => i.Id);
        _metricValueRepositoryMock.Setup(i => i.ReadAllMetricValuesForMetricsId(metricsIds)).ReturnsAsync(metricValues);

        // Act
        var result = await _metricValueService.GetAllMetricValuesForResource(resourceId);

        // Assert
        result.Should().HaveCount(2);
        _metricRepositoryMock.Verify(i => i.ReadAllMetricValuesForResource(resourceId), Times.Once);
        _metricValueRepositoryMock.Verify(i => i.ReadAllMetricValuesForMetricsId(metricsIds), Times.Once);
    }

    [Fact]
    public async Task GetAllMetricValuesForResource_WhenMetricValuesNotFound_ShouldThrowNotFoundApplicationException()
    {
        // Arrange
        int resourceId = _faker.Random.Int(1, 100);
        _resourceRepositoryMock.Setup(i => i.ReadByResourceId(resourceId)).ReturnsAsync((Resource)null!);

        // Act & Assert
        await _metricValueService.Invoking(i => i.GetAllMetricValuesForResource(resourceId))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("Resource not found");

        _resourceRepositoryMock.Verify(i => i.ReadByResourceId(resourceId), Times.Once);
        _metricRepositoryMock.Verify(i => i.ReadAllMetricValuesForResource(resourceId), Times.Never);
        _metricValueRepositoryMock.Verify(i => i.ReadAllMetricValuesForMetricsId(It.IsAny<IEnumerable<int>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetMetricValue_WhenMetricValueExists_ReturnsMetricValueResponse()
    {
        // Arrange
        int metricValueId = _faker.Random.Int(1, 100);
        var metricValue = new MetricValue()
        {
            Id = metricValueId,
            MetricId = _faker.Random.Int(1, 100),
            Value = _faker.Random.Double()
        };

        _metricValueRepositoryMock.Setup(i => i.ReadMetricValueId(metricValueId)).ReturnsAsync(metricValue);

        // Act
        var result = await _metricValueService.GetMetricValue(metricValueId);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(metricValue.Value);
        result.MetricId.Should().Be(metricValue.MetricId);
    }

    [Fact]
    public async Task GetMetricValue_WhenMetricValueNotFound_ShouldThrowNotFoundApplicationException()
    {
        // Arrange
        int metricValueId = _faker.Random.Int(1, 100);
        _metricValueRepositoryMock.Setup(i => i.ReadMetricValueId(metricValueId)).ReturnsAsync((MetricValue)null!);

        // Act & Assert
        await _metricValueService.Invoking(i => i.GetMetricValue(metricValueId))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("MetricValue not found");

        _metricValueRepositoryMock.Verify(i => i.ReadMetricValueId(metricValueId), Times.Once);
    }

    #endregion
}
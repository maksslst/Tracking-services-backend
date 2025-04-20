using Application.Exceptions;
using Application.Mappings;
using Application.Requests;
using Application.Services;
using AutoMapper;
using Bogus;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Repositories.MetricRepository;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApplicationUnitTests.Services;

public class MetricServiceTests
{
    private readonly Mock<IMetricRepository> _metricRepositoryMock;
    private readonly IMetricService _metricService;
    private readonly Faker _faker;

    public MetricServiceTests()
    {
        _metricRepositoryMock = new Mock<IMetricRepository>();
        var loggerMock = new Mock<ILogger<MetricService>>();

        var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = mappingConfig.CreateMapper();

        _metricService = new MetricService(_metricRepositoryMock.Object, mapper, loggerMock.Object);
        _faker = new Faker();
    }

    #region AddTests

    [Fact]
    public async Task AddMetric_WhenValidRequest_ReturnsMetricId()
    {
        // Arrange
        var request = new CreateMetricRequest()
        {
            Name = _faker.Name.FullName(),
            ResourceId = 1,
            Unit = "мс"
        };

        _metricRepositoryMock.Setup(i => i.CreateMetric(It.IsAny<Metric>())).Returns(Task.FromResult(1));

        // Act
        var result = await _metricService.AddMetric(request);

        // Assert
        result.Should().Be(1);
        _metricRepositoryMock.Verify(i => i.CreateMetric(It.Is<Metric>(i =>
            i.Name == request.Name &&
            i.ResourceId == request.ResourceId &&
            i.Unit == request.Unit)), Times.Once);
    }

    #endregion

    #region UpdateTests

    [Fact]
    public async Task UpdateMetric_WhenMetricExists_UpdatesMetricSuccessfully()
    {
        // Arrange
        var request = new UpdateMetricRequest()
        {
            Id = 1,
            Name = _faker.Name.FullName(),
            ResourceId = 1,
            Unit = "мс",
        };

        var response = new Metric()
        {
            Id = 1,
            Name = _faker.Name.FullName(),
            ResourceId = 1,
            Unit = "%",
            Created = DateTime.Now
        };

        _metricRepositoryMock.Setup(i => i.ReadMetricId(request.Id)).ReturnsAsync(response);
        _metricRepositoryMock.Setup(i => i.UpdateMetric(It.IsAny<Metric>())).ReturnsAsync(true);

        // Act
        await _metricService.UpdateMetric(request);

        // Assert
        _metricRepositoryMock.Verify(i => i.ReadMetricId(request.Id), Times.Once);
        _metricRepositoryMock.Verify(i => i.UpdateMetric(It.Is<Metric>(i =>
            i.Id == request.Id &&
            i.ResourceId == request.ResourceId &&
            i.Unit == request.Unit)), Times.Once);
    }

    [Fact]
    public async Task UpdateMetric_WhenMetricDoesNotExists_ShouldThrowNotFoundApplicationException()
    {
        // Arrange
        var request = new UpdateMetricRequest()
        {
            Id = 1,
            Name = _faker.Name.FullName(),
            ResourceId = 1,
            Unit = "мс",
        };

        _metricRepositoryMock.Setup(i => i.ReadMetricId(request.Id)).ReturnsAsync((Metric)null);

        // Act & Assert
        await _metricService.Invoking(i => i.UpdateMetric(request))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("Metric not found");
        _metricRepositoryMock.Verify(i => i.ReadMetricId(request.Id), Times.Once);
    }

    [Fact]
    public async Task UpdateMetric_WhenMetricNotFound_ShouldThrowEntityUpdateException()
    {
        // Arrange
        var request = new UpdateMetricRequest()
        {
            Id = 1,
            Name = _faker.Name.FullName(),
            ResourceId = 1,
            Unit = "мс",
        };

        var response = new Metric()
        {
            Id = 1,
            Name = _faker.Name.FullName(),
            ResourceId = 1,
            Unit = "%",
            Created = DateTime.Now
        };

        _metricRepositoryMock.Setup(i => i.ReadMetricId(request.Id)).ReturnsAsync(response);
        _metricRepositoryMock.Setup(i => i.UpdateMetric(It.IsAny<Metric>())).ReturnsAsync(false);

        // Act & Assert
        await _metricService.Invoking(i => i.UpdateMetric(request))
            .Should()
            .ThrowAsync<EntityUpdateException>()
            .WithMessage("Couldn't update the metric");

        _metricRepositoryMock.Verify(i => i.ReadMetricId(request.Id), Times.Once);
        _metricRepositoryMock.Verify(i => i.UpdateMetric(It.Is<Metric>(i =>
            i.Id == request.Id &&
            i.ResourceId == request.ResourceId &&
            i.Unit == request.Unit &&
            i.Name == request.Name)), Times.Once);
    }

    #endregion

    #region DeleteTests

    [Fact]
    public async Task DeleteMetric_WhenMetricExists_ShouldDeleteMetricSuccessfully()
    {
        // Arrange
        int metricId = _faker.Random.Int(1, 100);
        _metricRepositoryMock.Setup(i => i.DeleteMetric(metricId)).ReturnsAsync(true);

        // Act
        await _metricService.DeleteMetric(metricId);

        // Assert
        _metricRepositoryMock.Verify(i => i.DeleteMetric(metricId), Times.Once);
    }

    [Fact]
    public async Task DeleteMetric_WhenDeletionFails_ShouldThrowEntityDeleteException()
    {
        // Arrange
        int metricId = _faker.Random.Int(1, 100);
        _metricRepositoryMock.Setup(i => i.DeleteMetric(metricId)).ReturnsAsync(false);

        // Act & Assert
        await _metricService.Invoking(i => i.DeleteMetric(metricId))
            .Should()
            .ThrowAsync<EntityDeleteException>()
            .WithMessage("Couldn't delete the metric");

        _metricRepositoryMock.Verify(i => i.DeleteMetric(metricId), Times.Once);
    }

    #endregion

    #region GetTests

    [Fact]
    public async Task GetMetricByResourceId_WhenMetricExists_ReturnsMetricResponse()
    {
        // Arrange
        var metric = new Metric()
        {
            Id = _faker.Random.Int(),
            Name = _faker.Name.FullName(),
            ResourceId = _faker.Random.Int(),
            Unit = _faker.Random.String(),
            Created = DateTime.Now
        };

        _metricRepositoryMock.Setup(i => i.ReadMetricByResourceId(metric.ResourceId)).ReturnsAsync(metric);

        // Act
        var result = await _metricService.GetMetricByResourceId(metric.ResourceId);

        // Assert
        result.Should().NotBeNull();
        result.ResourceId.Should().Be(metric.ResourceId);
        result.Name.Should().Be(metric.Name);
        result.Unit.Should().Be(metric.Unit);
        result.Created.Should().Be(metric.Created);
        _metricRepositoryMock.Verify(i => i.ReadMetricByResourceId(metric.ResourceId), Times.Once);
    }

    [Fact]
    public async Task GetMetricByResourceId_WhenMetricNotFound_ShouldThrowNotFoundApplicationException()
    {
        // Arrange
        int resourceId = _faker.Random.Int();
        _metricRepositoryMock.Setup(i => i.ReadMetricByResourceId(resourceId)).ReturnsAsync((Metric)null);

        // Act & Assert
        await _metricService.Invoking(i => i.GetMetricByResourceId(resourceId))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("Metric not found");

        _metricRepositoryMock.Verify(i => i.ReadMetricByResourceId(resourceId), Times.Once);
    }

    [Fact]
    public async Task GetAllMetricsByResourceId_WhenMetricsExists_ReturnsMetricsResponse()
    {
        // Arrange
        int resourceId = _faker.Random.Int();
        var metrics = new List<Metric>()
        {
            new Metric()
            {
                Id = _faker.Random.Int(),
                Name = _faker.Random.String(),
                ResourceId = resourceId,
                Unit = _faker.Random.String(),
                Created = DateTime.Now
            },
            new Metric()
            {
                Id = _faker.Random.Int(),
                Name = _faker.Random.String(),
                ResourceId = resourceId,
                Unit = _faker.Random.String(),
                Created = DateTime.Now
            }
        };

        _metricRepositoryMock.Setup(i => i.ReadAllMetricValuesForResource(resourceId)).ReturnsAsync(metrics);

        // Act
        var result = await _metricService.GetAllMetricsByResourceId(resourceId);

        // Assert
        result.Should().HaveCount(2);
        _metricRepositoryMock.Verify(i => i.ReadAllMetricValuesForResource(resourceId), Times.Once);
    }

    [Fact]
    public async Task GetAll_WhenMetricsExists_ReturnsMetricsResponse()
    {
        // Arrange
        var metrics = new List<Metric>()
        {
            new Metric()
            {
                Id = _faker.Random.Int(),
                Name = _faker.Random.String(),
                ResourceId = _faker.Random.Int(),
                Unit = _faker.Random.String(),
                Created = DateTime.Now
            },
            new Metric()
            {
                Id = _faker.Random.Int(),
                Name = _faker.Random.String(),
                ResourceId = _faker.Random.Int(),
                Unit = _faker.Random.String(),
                Created = DateTime.Now
            }
        };

        _metricRepositoryMock.Setup(i => i.ReadAll()).ReturnsAsync(metrics);

        // Act
        var result = await _metricService.GetAll();

        // Assert
        result.Should().HaveCount(2);
        _metricRepositoryMock.Verify(i => i.ReadAll(), Times.Once);
    }

    #endregion
}
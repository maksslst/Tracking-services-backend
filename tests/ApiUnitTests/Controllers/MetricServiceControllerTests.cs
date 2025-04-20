using Api.Controllers;
using Application.Requests;
using Application.Responses;
using Application.Services;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Controllers;

public class MetricServiceControllerTests
{
    private readonly Mock<IMetricService> _metricServiceMock;
    private readonly MetricServiceController _controller;
    private readonly Faker _faker;

    public MetricServiceControllerTests()
    {
        _metricServiceMock = new Mock<IMetricService>();
        _controller = new MetricServiceController(_metricServiceMock.Object);
        _faker = new Faker();
    }

    #region AddTests

    [Fact]
    public async Task AddMetric_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateMetricRequest()
        {
            ResourceId = _faker.Random.Int(1, 100),
            Name = _faker.Random.String(),
            Unit = _faker.Random.String(),
        };
        var metricId = _faker.Random.Int(1, 100);
        _metricServiceMock.Setup(x => x.AddMetric(request)).ReturnsAsync(metricId);

        // Act
        var result = await _controller.AddMetric(request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult?.ActionName.Should().Be(nameof(_controller.GetMetricByResourceId));
        createdResult?.RouteValues?["resourceId"].Should().Be(metricId);
        createdResult?.Value.Should().Be(metricId);
        _metricServiceMock.Verify(x => x.AddMetric(request), Times.Once());
    }

    #endregion

    #region UpdateTests

    [Fact]
    public async Task UpdateMetric_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new UpdateMetricRequest
        {
            Id = _faker.Random.Int(1, 100),
            ResourceId = _faker.Random.Int(1, 100),
            Name = _faker.Random.String(),
            Unit = _faker.Random.String()
        };
        _metricServiceMock.Setup(x => x.UpdateMetric(request)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateMetric(request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _metricServiceMock.Verify(x => x.UpdateMetric(request), Times.Once());
    }

    #endregion

    #region DeleteTests

    [Fact]
    public async Task DeleteMetric_ExistingMetric_ReturnsNoContent()
    {
        // Arrange
        var metricId = _faker.Random.Int(1, 100);
        _metricServiceMock.Setup(x => x.DeleteMetric(metricId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteMetric(metricId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _metricServiceMock.Verify(x => x.DeleteMetric(metricId), Times.Once());
    }

    #endregion

    #region GetTests

    [Fact]
    public async Task GetMetricByResourceId_ExistingMetric_ReturnsOkWithMetric()
    {
        // Arrange
        var resourceId = _faker.Random.Int(1, 100);
        var metric = new MetricResponse()
        {
            ResourceId = resourceId,
            Created = DateTime.Now,
            Unit = _faker.Random.String(),
            Name = _faker.Random.String(),
        };
        _metricServiceMock.Setup(x => x.GetMetricByResourceId(resourceId)).ReturnsAsync(metric);

        // Act
        var result = await _controller.GetMetricByResourceId(resourceId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(metric);
        _metricServiceMock.Verify(x => x.GetMetricByResourceId(resourceId), Times.Once());
    }

    [Fact]
    public async Task GetAllMetricServiceId_ExistingMetrics_ReturnsOkWithMetrics()
    {
        // Arrange
        var resourceId = _faker.Random.Int(1, 100);
        var metrics = new List<MetricResponse>
        {
            new MetricResponse
            {
                ResourceId = resourceId,
                Created = DateTime.Now,
                Unit = _faker.Random.String(),
                Name = _faker.Random.String(),
            },
            new MetricResponse
            {
                ResourceId = resourceId,
                Created = DateTime.Now,
                Unit = _faker.Random.String(),
                Name = _faker.Random.String(),
            }
        };
        _metricServiceMock.Setup(x => x.GetAllMetricsByResourceId(resourceId)).ReturnsAsync(metrics);

        // Act
        var result = await _controller.GetAllMetricServiceId(resourceId);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(metrics);
        _metricServiceMock.Verify(x => x.GetAllMetricsByResourceId(resourceId), Times.Once());
    }

    [Fact]
    public async Task GetAll_WithMetrics_ReturnsOkWithMetrics()
    {
        // Arrange
        var metrics = new List<MetricResponse>
        {
            new MetricResponse
            {
                ResourceId = _faker.Random.Int(1, 100),
                Created = DateTime.Now,
                Unit = _faker.Random.String(),
                Name = _faker.Random.String(),
            },
            new MetricResponse
            {
                ResourceId = _faker.Random.Int(1, 100),
                Created = DateTime.Now,
                Unit = _faker.Random.String(),
                Name = _faker.Random.String(),
            }
        };
        _metricServiceMock.Setup(x => x.GetAll()).ReturnsAsync(metrics);
        
        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(metrics);
        _metricServiceMock.Verify(x => x.GetAll(), Times.Once());
    }
    #endregion
}
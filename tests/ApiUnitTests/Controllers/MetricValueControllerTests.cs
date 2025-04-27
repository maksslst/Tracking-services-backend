using Api.Controllers;
using Application.Requests;
using Application.Responses;
using Application.Services;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Controllers;

public class MetricValueControllerTests
{
    private readonly Mock<IMetricValueService> _metricValueServiceMock;
    private readonly MetricValueController _controller;
    private readonly Faker _faker;

    public MetricValueControllerTests()
    {
        _metricValueServiceMock = new Mock<IMetricValueService>();
        _controller = new MetricValueController(_metricValueServiceMock.Object);
        _faker = new Faker();
    }

    #region AddTests

    [Fact]
    public async Task AddMetricValue_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateMetricValueRequest()
            { MetricId = _faker.Random.Int(1, 100), Value = _faker.Random.Double(0, 100) };
        var metricValueId = _faker.Random.Int(1, 100);
        _metricValueServiceMock.Setup(x => x.AddMetricValue(request)).ReturnsAsync(metricValueId);

        // Act
        var result = await _controller.AddMetricValue(request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult?.ActionName.Should().Be(nameof(_controller.GetByMetricValueId));
        createdResult?.RouteValues?["metricValueId"].Should().Be(metricValueId);
        createdResult?.Value.Should().Be(metricValueId);
        _metricValueServiceMock.Verify(x => x.AddMetricValue(request), Times.Once());
    }

    #endregion

    #region GetTests

    [Fact]
    public async Task GetAllMetricValuesByResourceId_WithMetricValues_ReturnsOkWithMetricValues()
    {
        // Arrange
        var resourceId = _faker.Random.Int(1, 100);
        var metricValues = new List<MetricValueResponse>
        {
            new MetricValueResponse { MetricId = _faker.Random.Int(1, 100), Value = _faker.Random.Double(0, 100) },
            new MetricValueResponse { MetricId = _faker.Random.Int(1, 100), Value = _faker.Random.Double(0, 100) }
        };
        _metricValueServiceMock.Setup(x => x.GetAllMetricValuesForResource(resourceId)).ReturnsAsync(metricValues);

        // Act
        var result = await _controller.GetAllMetricValuesByResourceId(resourceId);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(metricValues);
        _metricValueServiceMock.Verify(x => x.GetAllMetricValuesForResource(resourceId), Times.Once());
    }

    [Fact]
    public async Task GetByMetricValueId_ExistingMetricValue_ReturnsOkWithMetricValue()
    {
        // Arrange
        var metricValueId = _faker.Random.Int(1, 100);
        var metricValue = new MetricValueResponse { MetricId = _faker.Random.Int(1, 100), Value = _faker.Random.Double(0, 100) };
        _metricValueServiceMock.Setup(x => x.GetMetricValue(metricValueId)).ReturnsAsync(metricValue);

        // Act
        var result = await _controller.GetByMetricValueId(metricValueId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(metricValue);
        _metricValueServiceMock.Verify(x => x.GetMetricValue(metricValueId), Times.Once());
    }
    #endregion
}
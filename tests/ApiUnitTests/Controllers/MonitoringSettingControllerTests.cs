using Api.Controllers;
using Application.Requests;
using Application.Responses;
using Application.Services;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Controllers;

public class MonitoringSettingControllerTests
{
    private readonly Mock<IMonitoringSettingService> _monitoringSettingServiceMock;
    private readonly MonitoringSettingController _controller;
    private readonly Faker _faker;

    public MonitoringSettingControllerTests()
    {
        _monitoringSettingServiceMock = new Mock<IMonitoringSettingService>();
        _controller = new MonitoringSettingController(_monitoringSettingServiceMock.Object);
        _faker = new Faker();
    }

    #region AddTests

    [Fact]
    public async Task Add_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateMonitoringSettingRequest
            { ResourceId = _faker.Random.Int(1, 100), CheckInterval = _faker.Random.String(), Mode = true };
        var monitoringSettingId = _faker.Random.Int(1, 100);
        _monitoringSettingServiceMock.Setup(x => x.Add(request)).ReturnsAsync(monitoringSettingId);

        // Act
        var result = await _controller.Add(request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult?.ActionName.Should().Be(nameof(_controller.GetMonitoringSettingByResourceId));
        createdResult?.RouteValues?["resourceId"].Should().Be(monitoringSettingId);
        createdResult?.Value.Should().Be(monitoringSettingId);
        _monitoringSettingServiceMock.Verify(x => x.Add(request), Times.Once());
    }

    #endregion

    #region UpdateTests

    [Fact]
    public async Task Update_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new UpdateMonitoringSettingRequest
        {
            Id = _faker.Random.Int(1, 100),
            ResourceId = _faker.Random.Int(1, 100),
            CheckInterval = _faker.Random.String(),
            Mode = true
        };
        _monitoringSettingServiceMock.Setup(x => x.Update(request)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _monitoringSettingServiceMock.Verify(x => x.Update(request), Times.Once());
    }

    #endregion

    #region DeleteTests

    [Fact]
    public async Task Delete_ExistingMonitoringSetting_ReturnsNoContent()
    {
        // Arrange
        var monitoringSettingId = _faker.Random.Int(1, 100);
        _monitoringSettingServiceMock.Setup(x => x.Delete(monitoringSettingId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(monitoringSettingId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _monitoringSettingServiceMock.Verify(x => x.Delete(monitoringSettingId), Times.Once());
    }

    #endregion

    #region GetTests

    [Fact]
    public async Task GetMonitoringSettingByResourceId_ExistingMonitoringSetting_ReturnsOkWithMonitoringSetting()
    {
        // Arrange
        var resourceId = _faker.Random.Int(1, 100);
        var monitoringSetting = new MonitoringSettingResponse()
            { ResourceId = resourceId, CheckInterval = _faker.Random.String(), Mode = true };
        _monitoringSettingServiceMock.Setup(x => x.GetMonitoringSetting(resourceId)).ReturnsAsync(monitoringSetting);

        // Act
        var result = await _controller.GetMonitoringSettingByResourceId(resourceId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(monitoringSetting);
        _monitoringSettingServiceMock.Verify(x => x.GetMonitoringSetting(resourceId), Times.Once());
    }

    #endregion
}
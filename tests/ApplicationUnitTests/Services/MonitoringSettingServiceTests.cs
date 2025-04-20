using Application.Exceptions;
using Application.Mappings;
using Application.Requests;
using Application.Services;
using AutoMapper;
using Bogus;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Repositories.MonitoringSettingRepository;
using Moq;
using Microsoft.Extensions.Logging;

namespace ApplicationUnitTests.Services;

public class MonitoringSettingServiceTests
{
    private readonly Mock<IMonitoringSettingRepository> _monitoringSettingRepositoryMock;
    private readonly MonitoringSettingService _monitoringSettingService;
    private readonly Faker _faker;

    public MonitoringSettingServiceTests()
    {
        _monitoringSettingRepositoryMock = new Mock<IMonitoringSettingRepository>();
        var loggerMock = new Mock<ILogger<MonitoringSettingService>>();

        var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = mappingConfig.CreateMapper();

        _monitoringSettingService =
            new MonitoringSettingService(_monitoringSettingRepositoryMock.Object, mapper, loggerMock.Object);
        _faker = new Faker();
    }

    #region AddTests

    [Fact]
    public async Task Add_WhenValidRequest_ReturnsMonitoringSettingId()
    {
        // Arrange
        var request = new CreateMonitoringSettingRequest()
        {
            ResourceId = _faker.Random.Int(1, 100),
            Mode = true,
            CheckInterval = "0 0/5 * * * ?"
        };

        _monitoringSettingRepositoryMock.Setup(i => i.CreateSetting(It.IsAny<MonitoringSetting>())).ReturnsAsync(1);

        // Act
        var result = await _monitoringSettingService.Add(request);

        // Assert
        result.Should().Be(1);
        _monitoringSettingRepositoryMock.Verify(i => i.CreateSetting(It.Is<MonitoringSetting>(m =>
            m.ResourceId == request.ResourceId &&
            m.Mode == request.Mode &&
            m.CheckInterval == request.CheckInterval)), Times.Once);
    }

    #endregion

    #region UpdateTests

    [Fact]
    public async Task Update_WhenMonitoringSettingExists_UpdatesMonitoringSettingSuccessfully()
    {
        // Arrange
        var request = new UpdateMonitoringSettingRequest()
        {
            Id = _faker.Random.Int(1, 100),
            ResourceId = _faker.Random.Int(1, 100),
            Mode = true,
            CheckInterval = "0 0/5 * * * ?"
        };

        var response = new MonitoringSetting()
        {
            Id = request.Id,
            ResourceId = request.ResourceId,
            Mode = false,
            CheckInterval = "1 0/5 * * * ?"
        };

        _monitoringSettingRepositoryMock.Setup(i => i.ReadByResourceId(request.ResourceId)).ReturnsAsync(response);
        _monitoringSettingRepositoryMock.Setup(i => i.UpdateSetting(It.IsAny<MonitoringSetting>())).ReturnsAsync(true);

        // Act
        await _monitoringSettingService.Update(request);

        // Assert
        _monitoringSettingRepositoryMock.Verify(i => i.ReadByResourceId(request.ResourceId), Times.Once);
        _monitoringSettingRepositoryMock.Verify(i => i.UpdateSetting(It.Is<MonitoringSetting>(m =>
            m.Id == request.Id &&
            m.Mode == request.Mode &&
            m.ResourceId == request.ResourceId &&
            m.CheckInterval == request.CheckInterval)), Times.Once);
    }

    [Fact]
    public async Task Update_WhenMonitoringSettingDoesNotExists_ShouldThrowNotFoundApplicationException()
    {
        // Arrange
        int resourceId = _faker.Random.Int(1, 100);
        var monitoringSetting = new UpdateMonitoringSettingRequest()
        {
            Id = _faker.Random.Int(1, 100),
            ResourceId = resourceId,
            Mode = false,
            CheckInterval = "1 0/5 * * * ?"
        };
        _monitoringSettingRepositoryMock.Setup(i => i.ReadByResourceId(resourceId))
            .ReturnsAsync((MonitoringSetting)null!);

        // Act & Assert
        await _monitoringSettingService.Invoking(i => i.Update(monitoringSetting))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("MonitoringSetting not found");

        _monitoringSettingRepositoryMock.Verify(i => i.ReadByResourceId(resourceId), Times.Once);
    }

    [Fact]
    public async Task Update_WhenMonitoringSettingDoesNotExists_ShouldThrowEntityUpdateException()
    {
        // Arrange
        var request = new UpdateMonitoringSettingRequest()
        {
            Id = _faker.Random.Int(1, 100),
            ResourceId = _faker.Random.Int(1, 100),
            Mode = false,
            CheckInterval = "0 0/5 * * * ?"
        };

        var response = new MonitoringSetting()
        {
            Id = request.Id,
            ResourceId = request.ResourceId,
            Mode = true,
            CheckInterval = "1 0/5 * * * ?"
        };

        _monitoringSettingRepositoryMock.Setup(i => i.ReadByResourceId(request.ResourceId)).ReturnsAsync(response);
        _monitoringSettingRepositoryMock.Setup(i => i.UpdateSetting(It.IsAny<MonitoringSetting>())).ReturnsAsync(false);

        // Act & Assert
        await _monitoringSettingService.Invoking(i => i.Update(request))
            .Should()
            .ThrowAsync<EntityUpdateException>()
            .WithMessage("Couldn't update the setting");

        _monitoringSettingRepositoryMock.Verify(i => i.ReadByResourceId(request.ResourceId), Times.Once);
        _monitoringSettingRepositoryMock.Verify(i => i.UpdateSetting(It.Is<MonitoringSetting>(m =>
            m.Id == request.Id &&
            m.CheckInterval == request.CheckInterval &&
            m.Mode == request.Mode &&
            m.ResourceId == request.ResourceId)), Times.Once);
    }

    #endregion

    #region DeleteTests

    [Fact]
    public async Task Delete_WhenMonitoringSettingExists_DeletesMonitoringSettingSuccessfully()
    {
        // Arrange
        int monitoringSettingId = _faker.Random.Int(1, 100);
        _monitoringSettingRepositoryMock.Setup(i => i.DeleteSetting(monitoringSettingId)).ReturnsAsync(true);

        // Act
        await _monitoringSettingService.Delete(monitoringSettingId);

        // Assert
        _monitoringSettingRepositoryMock.Verify(i => i.DeleteSetting(monitoringSettingId), Times.Once);
    }

    [Fact]
    public async Task DeleteMetric_WhenMetricDoesNotExists_ShouldThrowEntityDeleteException()
    {
        // Arrange
        int monitoringSettingId = _faker.Random.Int(1, 100);
        _monitoringSettingRepositoryMock.Setup(i => i.DeleteSetting(monitoringSettingId)).ReturnsAsync(false);

        // Act & Assert
        await _monitoringSettingService.Invoking(i => i.Delete(monitoringSettingId))
            .Should()
            .ThrowAsync<EntityDeleteException>()
            .WithMessage("Couldn't delete the setting");

        _monitoringSettingRepositoryMock.Verify(i => i.DeleteSetting(monitoringSettingId), Times.Once);
    }

    #endregion

    #region GetTests

    [Fact]
    public async Task GetMonitoringSetting_WhenMonitoringSettingExists_ReturnsMonitoringSettingResponse()
    {
        // Arrange
        var monitoringSetting = new MonitoringSetting()
        {
            Id = _faker.Random.Int(1, 100),
            ResourceId = _faker.Random.Int(1, 100),
            Mode = true,
            CheckInterval = "0 0/5 * * * ?"
        };

        _monitoringSettingRepositoryMock.Setup(i => i.ReadByResourceId(monitoringSetting.ResourceId))
            .ReturnsAsync(monitoringSetting);

        // Act
        var result = await _monitoringSettingService.GetMonitoringSetting(monitoringSetting.ResourceId);

        // Assert
        result.Should().NotBeNull();
        result.ResourceId.Should().Be(monitoringSetting.ResourceId);
        result.Mode.Should().Be(monitoringSetting.Mode);
        result.CheckInterval.Should().Be(monitoringSetting.CheckInterval);
    }

    [Fact]
    public async Task GetMonitoringSetting_WhenMonitoringSettingDoesNotExists_ShouldThrowNotFoundApplicationException()
    {
        // Arrange
        int resourceId = _faker.Random.Int();
        _monitoringSettingRepositoryMock.Setup(i => i.ReadByResourceId(resourceId))
            .ReturnsAsync((MonitoringSetting)null!);

        // Act & Assert
        await _monitoringSettingService.Invoking(i => i.GetMonitoringSetting(resourceId))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("MonitoringSetting not found");

        _monitoringSettingRepositoryMock.Verify(i => i.ReadByResourceId(resourceId), Times.Once);
    }

    #endregion
}
using Application.Exceptions;
using Application.Requests;
using Application.Services;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationIntegrationTests.Service;

public class MonitoringSettingServiceTests : IClassFixture<TestingFixture>
{
    private readonly IMonitoringSettingService _monitoringSettingService;
    private readonly TestingFixture _fixture;
    private readonly Faker _faker;

    public MonitoringSettingServiceTests(TestingFixture fixture)
    {
        _fixture = fixture;
        var scope = fixture.ServiceProvider.CreateScope();
        _monitoringSettingService = scope.ServiceProvider.GetRequiredService<IMonitoringSettingService>();
        _faker = new Faker();
    }

    [Fact]
    public async Task Add_ShouldCreateMonitoringSetting()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);
        var request = new CreateMonitoringSettingRequest
        {
            ResourceId = resource.Id,
            CheckInterval = _faker.Random.Word(),
            Mode = true
        };

        // Act
        var id = await _monitoringSettingService.Add(request);

        // Assert
        var monitoringSetting = await _monitoringSettingService.GetMonitoringSetting(resource.Id);
        monitoringSetting.Should().NotBeNull();
        monitoringSetting.ResourceId.Should().Be(resource.Id);
        monitoringSetting.CheckInterval.Should().Be(request.CheckInterval);
        monitoringSetting.Mode.Should().Be(request.Mode);
    }

    [Fact]
    public async Task Update_ShouldUpdateMonitoringSetting()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);
        var monitoringSetting = await _fixture.CreateMonitoringSetting(resource.Id);

        var updateRequest = new UpdateMonitoringSettingRequest
        {
            Id = monitoringSetting.Id,
            ResourceId = resource.Id,
            CheckInterval = _faker.Random.Word(),
            Mode = false
        };

        // Act
        await _monitoringSettingService.Update(updateRequest);

        // Assert
        var response = await _monitoringSettingService.GetMonitoringSetting(resource.Id);
        response.Should().NotBeNull();
        response.CheckInterval.Should().Be(updateRequest.CheckInterval);
        response.Mode.Should().Be(updateRequest.Mode);
        response.ResourceId.Should().Be(updateRequest.ResourceId);
    }

    [Fact]
    public async Task Delete_ShouldRemoveMonitoringSetting()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);
        var monitoringSetting = await _fixture.CreateMonitoringSetting(resource.Id);

        // Act
        await _monitoringSettingService.Delete(monitoringSetting.Id);

        // Assert
        await _monitoringSettingService.Invoking(i => i.GetMonitoringSetting(monitoringSetting.ResourceId))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("MonitoringSetting not found");
    }

    [Fact]
    public async Task GetMonitoringSetting_ShouldReturnMonitoringSetting()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);
        var monitoringSetting = await _fixture.CreateMonitoringSetting(resource.Id);

        // Act
        var response = await _monitoringSettingService.GetMonitoringSetting(monitoringSetting.ResourceId);

        // Assert
        response.Should().NotBeNull();
        response.CheckInterval.Should().Be(monitoringSetting.CheckInterval);
        response.Mode.Should().Be(monitoringSetting.Mode);
        response.ResourceId.Should().Be(monitoringSetting.ResourceId);
    }
}
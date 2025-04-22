using Api.Controllers;
using Application.Requests;
using Application.Responses;
using Application.Services;
using Bogus;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Controllers;

public class ResourceControllerTests
{
    private readonly Mock<IResourceService> _resourceServiceMock;
    private readonly ResourceController _controller;
    private readonly Faker _faker;

    public ResourceControllerTests()
    {
        _resourceServiceMock = new Mock<IResourceService>();
        _controller = new ResourceController(_resourceServiceMock.Object);
        _faker = new Faker();
    }

    #region AddTests

    [Fact]
    public async Task Add_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateResourceRequest
        {
            Name = _faker.Random.String(),
            Type = _faker.Random.String(),
            Status = ResourceStatus.Active,
            Source = _faker.Random.String()
        };
        var resourceId = _faker.Random.Int(1, 100);
        _resourceServiceMock.Setup(x => x.Add(request)).ReturnsAsync(resourceId);

        // Act
        var result = await _controller.Add(request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult?.ActionName.Should().Be(nameof(_controller.GetByResourceId));
        createdResult?.RouteValues?["resourceId"].Should().Be(resourceId);
        createdResult?.Value.Should().Be(resourceId);
        _resourceServiceMock.Verify(x => x.Add(request), Times.Once());
    }

    [Fact]
    public async Task AddCompanyResource_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var companyId = _faker.Random.Int(1, 100);
        var request = new CreateResourceRequest
        {
            Name = _faker.Random.String(),
            Type = _faker.Random.String(),
            Status = ResourceStatus.Active,
            Source = _faker.Random.String(),
            CompanyId = companyId
        };
        _resourceServiceMock.Setup(x => x.AddCompanyResource(companyId, request)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddCompanyResource(companyId, request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _resourceServiceMock.Verify(x => x.AddCompanyResource(companyId, request), Times.Once());
    }

    #endregion

    #region UpdateTests

    [Fact]
    public async Task Update_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new UpdateResourceRequest
        {
            Id = _faker.Random.Int(1, 100),
            Name = _faker.Random.String(),
            Type = _faker.Random.String(),
            Status = ResourceStatus.Active,
            Source = _faker.Random.String()
        };
        _resourceServiceMock.Setup(x => x.Update(request)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _resourceServiceMock.Verify(x => x.Update(request), Times.Once());
    }

    [Fact]
    public async Task UpdateCompanyResource_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var companyId = _faker.Random.Int(1, 100);
        var resourceId = _faker.Random.Int(1, 100);
        var request = new UpdateResourceRequest()
        {
            Id = resourceId,
            Name = _faker.Random.String(),
            Type = _faker.Random.String(),
            Status = ResourceStatus.Active,
            Source = _faker.Random.String(),
            CompanyId = companyId
        };
        _resourceServiceMock.Setup(x => x.UpdateCompanyResource(companyId, request, resourceId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateCompanyResource(companyId, resourceId, request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _resourceServiceMock.Verify(x => x.UpdateCompanyResource(companyId, request, resourceId), Times.Once());
    }

    #endregion

    #region DeleteTests

    [Fact]
    public async Task Delete_ExistingResource_ReturnsNoContent()
    {
        // Arrange
        var resourceId = _faker.Random.Int(1, 100);
        _resourceServiceMock.Setup(x => x.Delete(resourceId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(resourceId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _resourceServiceMock.Verify(x => x.Delete(resourceId), Times.Once());
    }

    [Fact]
    public async Task DeleteCompanyResource_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var resourceId = _faker.Random.Int(1, 100);
        var companyId = _faker.Random.Int(1, 100);
        _resourceServiceMock.Setup(x => x.DeleteCompanyResource(resourceId, companyId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteCompanyResource(resourceId, companyId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _resourceServiceMock.Verify(x => x.DeleteCompanyResource(resourceId, companyId), Times.Once());
    }

    #endregion

    #region GetTests

    [Fact]
    public async Task GetByResourceId_ExistingResource_ReturnsOkWithResource()
    {
        // Arrange
        var resourceId = _faker.Random.Int(1, 100);
        var resource = CreatingResourceResponse(resourceId);
        _resourceServiceMock.Setup(x => x.GetResource(resourceId)).ReturnsAsync(resource);

        // Act
        var result = await _controller.GetByResourceId(resourceId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(resource);
        _resourceServiceMock.Verify(x => x.GetResource(resourceId), Times.Once());
    }

    [Fact]
    public async Task GetAllResources_ExistingResources_ReturnsOkWithResources()
    {
        // Arrange
        var resources = new List<ResourceResponse>
        {
            CreatingResourceResponse(_faker.Random.Int(1, 100)),
            CreatingResourceResponse(_faker.Random.Int(1, 100))
        };
        _resourceServiceMock.Setup(x => x.GetAllResources()).ReturnsAsync(resources);

        // Act
        var result = await _controller.GetAllResources();

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(resources);
        _resourceServiceMock.Verify(x => x.GetAllResources(), Times.Once());
    }

    [Fact]
    public async Task GetCompanyResources_ExistingResources_ReturnsOkWithResources()
    {
        // Arrange
        var companyId = _faker.Random.Int(1, 100);
        var resources = new List<ResourceResponse>
        {
            CreatingResourceResponse(companyId),
            CreatingResourceResponse(companyId)
        };
        _resourceServiceMock.Setup(x => x.GetCompanyResources(companyId)).ReturnsAsync(resources);

        // Act
        var result = await _controller.GetCompanyResources(companyId);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(resources);
        _resourceServiceMock.Verify(x => x.GetCompanyResources(companyId), Times.Once());
    }

    #endregion

    private ResourceResponse CreatingResourceResponse(int companyId)
    {
        var resource = new ResourceResponse()
        {
            CompanyId = companyId,
            Name = _faker.Random.String(),
            Type = _faker.Random.String(),
            Status = ResourceStatus.Active,
            Source = _faker.Random.String(),
        };

        return resource;
    }
}
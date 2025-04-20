using Application.Exceptions;
using Application.Requests;
using Application.Services;
using AutoMapper;
using Bogus;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Repositories.CompanyRepository;
using Infrastructure.Repositories.ResourceRepository;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApplicationUnitTests.Services;

public class ResourceServiceTests
{
    private readonly Mock<IResourceRepository> _resourceRepoMock;
    private readonly Mock<ICompanyRepository> _companyRepoMock;
    private readonly IResourceService _resourceService;
    private readonly Faker _faker;

    public ResourceServiceTests()
    {
        _resourceRepoMock = new Mock<IResourceRepository>();
        _companyRepoMock = new Mock<ICompanyRepository>();
        var loggerMock = new Mock<ILogger<ResourceService>>();
        _faker = new Faker();

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<Application.Mappings.MappingProfile>());
        var mapper = mapperConfig.CreateMapper();

        _resourceService = new ResourceService(_resourceRepoMock.Object, mapper, _companyRepoMock.Object,
            loggerMock.Object
        );
    }

    #region Add

    [Fact]
    public async Task Add_ValidRequest_ReturnsResourceId()
    {
        // Arrange
        var request = new CreateResourceRequest() { Name = _faker.Commerce.ProductName() };
        _resourceRepoMock.Setup(r => r.CreateResource(It.IsAny<Resource>())).ReturnsAsync(1);

        // Act
        var result = await _resourceService.Add(request);

        // Assert
        result.Should().Be(1);
        _resourceRepoMock.Verify(r => r.CreateResource(It.IsAny<Resource>()), Times.Once);
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_ValidRequest_UpdatesResource()
    {
        // Arrange
        var request = new UpdateResourceRequest { Id = 1, Name = _faker.Commerce.ProductName() };
        _resourceRepoMock.Setup(r => r.ReadByResourceId(request.Id)).ReturnsAsync(new Resource
        {
            Id = request.Id,
            Name = _faker.Commerce.ProductName(),
            Type = _faker.Commerce.ProductMaterial(),
            Source = _faker.Internet.Url(),
            Status = ResourceStatus.Active
        });

        _resourceRepoMock.Setup(r => r.UpdateResource(It.IsAny<Resource>())).ReturnsAsync(true);

        // Act
        await _resourceService.Update(request);

        // Assert
        _resourceRepoMock.Verify(r => r.UpdateResource(It.IsAny<Resource>()), Times.Once);
    }

    [Fact]
    public async Task Update_ResourceNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var request = new UpdateResourceRequest { Id = 1 };
        _resourceRepoMock.Setup(r => r.ReadByResourceId(request.Id)).ReturnsAsync((Resource)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundApplicationException>(() => _resourceService.Update(request));
    }

    [Fact]
    public async Task Update_UpdateFails_ThrowsEntityUpdateException()
    {
        // Arrange
        var request = new UpdateResourceRequest { Id = 1, Name = "Test" };
        _resourceRepoMock.Setup(r => r.ReadByResourceId(request.Id)).ReturnsAsync(new Resource
        {
            Id = request.Id,
            Name = _faker.Commerce.ProductName(),
            Type = _faker.Commerce.ProductMaterial(),
            Source = _faker.Internet.Url(),
            Status = ResourceStatus.Active
        });
        _resourceRepoMock.Setup(r => r.UpdateResource(It.IsAny<Resource>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityUpdateException>(() => _resourceService.Update(request));
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_ValidId_DeletesResource()
    {
        // Arrange
        int resourceId = 1;
        _resourceRepoMock.Setup(r => r.DeleteResource(resourceId)).ReturnsAsync(true);

        // Act
        await _resourceService.Delete(resourceId);

        // Assert
        _resourceRepoMock.Verify(r => r.DeleteResource(resourceId), Times.Once);
    }

    [Fact]
    public async Task Delete_DeleteFails_ThrowsEntityDeleteException()
    {
        // Arrange
        int resourceId = 1;
        _resourceRepoMock.Setup(r => r.DeleteResource(resourceId)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityDeleteException>(() => _resourceService.Delete(resourceId));
    }

    #endregion

    #region AddCompanyResource

    [Fact]
    public async Task AddCompanyResource_ValidRequest_AddsResource()
    {
        // Arrange
        var request = new CreateResourceRequest { Name = "Test" };
        var companyId = 1;

        _resourceRepoMock.Setup(r => r.CreateResource(It.IsAny<Resource>())).ReturnsAsync(10);
        _resourceRepoMock.Setup(r => r.AddCompanyResource(It.IsAny<Resource>())).ReturnsAsync(true);

        // Act
        await _resourceService.AddCompanyResource(companyId, request);

        // Assert
        _resourceRepoMock.Verify(r => r.AddCompanyResource(It.IsAny<Resource>()), Times.Once);
    }

    [Fact]
    public async Task AddCompanyResource_AddFails_ThrowsEntityCreateException()
    {
        // Arrange
        var request = new CreateResourceRequest();
        var companyId = 1;

        _resourceRepoMock.Setup(r => r.CreateResource(It.IsAny<Resource>())).ReturnsAsync(10);
        _resourceRepoMock.Setup(r => r.AddCompanyResource(It.IsAny<Resource>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityCreateException>(() => _resourceService.AddCompanyResource(companyId, request));
    }

    #endregion

    #region UpdateCompanyResource

    [Fact]
    public async Task UpdateCompanyResource_ValidRequest_UpdatesResource()
    {
        // Arrange
        var request = new UpdateResourceRequest { Id = 1, Name = "Updated" };
        _resourceRepoMock.Setup(r => r.ReadByResourceId(request.Id)).ReturnsAsync(new Resource
        {
            Id = request.Id,
            Name = _faker.Commerce.ProductName(),
            Type = _faker.Commerce.ProductMaterial(),
            Source = _faker.Internet.Url(),
            Status = ResourceStatus.Active
        });
        _resourceRepoMock.Setup(r => r.UpdateResource(It.IsAny<Resource>())).ReturnsAsync(true);

        // Act
        await _resourceService.UpdateCompanyResource(1, request, 1);

        // Assert
        _resourceRepoMock.Verify(r => r.UpdateResource(It.IsAny<Resource>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCompanyResource_ResourceNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var request = new UpdateResourceRequest();
        _resourceRepoMock.Setup(r => r.ReadByResourceId(1)).ReturnsAsync((Resource)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundApplicationException>(() =>
            _resourceService.UpdateCompanyResource(1, request, 1));
    }

    [Fact]
    public async Task UpdateCompanyResource_UpdateFails_ThrowsEntityUpdateException()
    {
        // Arrange
        var request = new UpdateResourceRequest { Id = 1 };
        _resourceRepoMock.Setup(r => r.ReadByResourceId(request.Id)).ReturnsAsync(new Resource
        {
            Id = request.Id,
            Name = _faker.Commerce.ProductName(),
            Type = _faker.Commerce.ProductMaterial(),
            Source = _faker.Internet.Url(),
            Status = ResourceStatus.Active
        });

        _resourceRepoMock.Setup(r => r.UpdateResource(It.IsAny<Resource>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityUpdateException>(() =>
            _resourceService.UpdateCompanyResource(1, request, 1));
    }

    #endregion

    #region DeleteCompanyResource

    [Fact]
    public async Task DeleteCompanyResource_ValidRequest_DeletesResource()
    {
        // Arrange
        _resourceRepoMock.Setup(r => r.DeleteCompanyResource(1, 1)).ReturnsAsync(true);

        // Act
        await _resourceService.DeleteCompanyResource(1, 1);

        // Assert
        _resourceRepoMock.Verify(r => r.DeleteCompanyResource(1, 1), Times.Once);
    }

    [Fact]
    public async Task DeleteCompanyResource_DeleteFails_ThrowsEntityDeleteException()
    {
        // Arrange
        _resourceRepoMock.Setup(r => r.DeleteCompanyResource(1, 1)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityDeleteException>(() => _resourceService.DeleteCompanyResource(1, 1));
    }

    #endregion

    #region GetResource

    [Fact]
    public async Task GetResource_ValidId_ReturnsResourceResponse()
    {
        // Arrange
        var resource = new Resource
        {
            Id = 1, Name = _faker.Commerce.ProductName(),
            Type = _faker.Commerce.ProductMaterial(),
            Source = _faker.Internet.Url(),
            Status = ResourceStatus.Active
        };
        _resourceRepoMock.Setup(r => r.ReadByResourceId(1)).ReturnsAsync(resource);

        // Act
        var result = await _resourceService.GetResource(1);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(resource.Name);
    }

    [Fact]
    public async Task GetResource_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _resourceRepoMock.Setup(r => r.ReadByResourceId(1)).ReturnsAsync((Resource)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundApplicationException>(() => _resourceService.GetResource(1));
    }

    #endregion

    #region GetAllResources

    [Fact]
    public async Task GetAllResources_ReturnsList()
    {
        // Arrange
        var list = new List<Resource>
        {
            new()
            {
                Id = 1, Name = _faker.Commerce.ProductName(),
                Type = _faker.Commerce.ProductMaterial(),
                Source = _faker.Internet.Url(),
                Status = ResourceStatus.Active
            },
            new()
            {
                Id = 2, Name = _faker.Commerce.ProductName(),
                Type = _faker.Commerce.ProductMaterial(),
                Source = _faker.Internet.Url(),
                Status = ResourceStatus.Active
            }
        };
        _resourceRepoMock.Setup(r => r.ReadAllResources()).ReturnsAsync(list);

        // Act
        var result = await _resourceService.GetAllResources();

        // Assert
        result.Should().HaveCount(2);
    }

    #endregion

    #region GetCompanyResources

    [Fact]
    public async Task GetCompanyResources_CompanyExists_ReturnsResources()
    {
        // Arrange
        var companyId = 1;
        _companyRepoMock.Setup(c => c.ReadByCompanyId(companyId)).ReturnsAsync(new Company()
        {
            Id = companyId,
            CompanyName = _faker.Company.CompanyName(),
            Resources = [],
            Users = []
        });
        _resourceRepoMock.Setup(r => r.ReadCompanyResources(companyId)).ReturnsAsync(new List<Resource>
        {
            new()
            {
                Id = 1, Name = _faker.Commerce.ProductName(),
                Type = _faker.Commerce.ProductMaterial(),
                Source = _faker.Internet.Url(),
                Status = ResourceStatus.Active
            },
            new()
            {
                Id = 2, Name = _faker.Commerce.ProductName(),
                Type = _faker.Commerce.ProductMaterial(),
                Source = _faker.Internet.Url(),
                Status = ResourceStatus.Active
            }
        });

        // Act
        var result = await _resourceService.GetCompanyResources(companyId);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCompanyResources_CompanyNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _companyRepoMock.Setup(c => c.ReadByCompanyId(1)).ReturnsAsync((Company)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundApplicationException>(() => _resourceService.GetCompanyResources(1));
    }

    #endregion
}
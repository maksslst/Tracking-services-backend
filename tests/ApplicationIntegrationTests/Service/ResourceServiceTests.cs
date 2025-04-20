using Application.Requests;
using Application.Services;
using Bogus;
using Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationIntegrationTests.Service;

public class ResourceServiceTests : IClassFixture<TestingFixture>
{
    private readonly IResourceService _resourceService;
    private readonly TestingFixture _fixture;
    private readonly Faker _faker;

    public ResourceServiceTests(TestingFixture fixture)
    {
        _fixture = fixture;
        var scope = fixture.ServiceProvider.CreateScope();
        _resourceService = scope.ServiceProvider.GetRequiredService<IResourceService>();
        _faker = new Faker();
    }

    [Fact]
    public async Task Add_ShouldCreateResource()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var request = new CreateResourceRequest
        {
            Name = _faker.Random.Word(),
            CompanyId = company.Id,
            Type = _faker.Random.Word(),
            Source = _faker.Random.Word(),
            Status = ResourceStatus.Active
        };

        // Act
        var id = await _resourceService.Add(request);

        // Assert
        var response = await _resourceService.GetResource(id);
        response.Name.Should().Be(request.Name);
        response.CompanyId.Should().Be(company.Id);
        response.Type.Should().Be(request.Type);
        response.Source.Should().Be(request.Source);
        response.Status.Should().Be(request.Status);
    }

    [Fact]
    public async Task Update_ShouldUpdateResource()
    {
        // Arrange
        var resource = await _fixture.CreateResource();

        var updateRequest = new UpdateResourceRequest
        {
            Id = resource.Id,
            Name = _faker.Random.Word(),
            CompanyId = resource.CompanyId,
            Type = _faker.Random.Word(),
            Source = _faker.Random.Word(),
            Status = ResourceStatus.Active
        };

        // Act
        Func<Task> act = async () => await _resourceService.Update(updateRequest);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Delete_ShouldRemoveResource()
    {
        // Arrange
        var resource = await _fixture.CreateResource();

        // Act
        Func<Task> act = async () => await _resourceService.Delete(resource.Id);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AddCompanyResource_ShouldCreateResourceAndLinkToCompany()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = new CreateResourceRequest
        {
            Name = _faker.Random.Word(),
            CompanyId = company.Id,
            Type = _faker.Random.Word(),
            Source = _faker.Random.Word(),
            Status = ResourceStatus.Active
        };

        // Act
        Func<Task> act = async () => await _resourceService.AddCompanyResource(company.Id, resource);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateCompanyResource_ShouldUpdateResourceForCompany()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);
        var updateRequest = new UpdateResourceRequest
        {
            Id = resource.Id,
            Name = _faker.Random.Word(),
            CompanyId = company.Id,
            Type = _faker.Random.Word(),
            Source = _faker.Random.Word(),
            Status = ResourceStatus.Active
        };

        // Act
        Func<Task> act = async () =>
            await _resourceService.UpdateCompanyResource(company.Id, updateRequest, resource.Id);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteCompanyResource_ShouldDeleteResourceFromCompany()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);

        // Act
        Func<Task> act = async () => await _resourceService.DeleteCompanyResource(resource.Id, company.Id);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task GetResource_ShouldReturnResource()
    {
        // Arrange
        var resource = await _fixture.CreateResource();

        // Act
        var result = await _resourceService.GetResource(resource.Id);

        // Assert
        result.Should().NotBeNull();
        result.Source.Should().Be(result.Source);
        result.Name.Should().Be(result.Name);
        result.CompanyId.Should().Be(result.CompanyId);
        result.Type.Should().Be(result.Type);
    }

    [Fact]
    public async Task GetAllResources_ShouldReturnAll()
    {
        // Arrange
        await _fixture.CreateResource();
        await _fixture.CreateResource();

        // Act
        var result = await _resourceService.GetAllResources();

        // Assert
        var resourceResponses = result.ToList();
        resourceResponses.Should().NotBeEmpty();
        resourceResponses.Count().Should().BeGreaterThanOrEqualTo(2);
    }
    
    [Fact]
    public async Task GetCompanyResources_ShouldReturnOnlyCompanyResources()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        await _fixture.CreateResource(company.Id);

        // Act
        var result = await _resourceService.GetCompanyResources(company.Id);

        // Assert
        var resourceResponses = result.ToList();
        resourceResponses.Should().NotBeEmpty();
        resourceResponses.All(r => r.CompanyId == company.Id).Should().BeTrue();
    }
}
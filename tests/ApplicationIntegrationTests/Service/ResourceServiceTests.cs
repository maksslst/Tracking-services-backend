using Application.Exceptions;
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
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);

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
        await _resourceService.Update(updateRequest);

        // Assert
        var response = await _resourceService.GetResource(updateRequest.Id);
        response.Should().NotBeNull();
        response.Name.Should().Be(updateRequest.Name);
        response.CompanyId.Should().Be(updateRequest.CompanyId);
        response.Type.Should().Be(updateRequest.Type);
        response.Source.Should().Be(updateRequest.Source);
        response.Status.Should().Be(updateRequest.Status);
    }

    [Fact]
    public async Task Delete_ShouldRemoveResource()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);

        // Act
        await _resourceService.Delete(resource.Id);

        // Assert
        await _resourceService.Invoking(i => i.GetResource(resource.Id))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("Resource not found");
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
        await _resourceService.AddCompanyResource(company.Id, resource);

        // Assert
        var resourcesCompany = (await _resourceService.GetCompanyResources(resource.CompanyId)).ToList();
        resourcesCompany.Should().NotBeNull();
        resourcesCompany.Should().Contain(i =>
            i.CompanyId == company.Id &&
            i.Type == resource.Type &&
            i.Source == resource.Source &&
            i.Status == resource.Status &&
            i.Name == resource.Name);
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
        await _resourceService.UpdateCompanyResource(company.Id, updateRequest, resource.Id);

        // Assert
        var resourceCompany = await _resourceService.GetResource(updateRequest.Id);
        resourceCompany.Should().NotBeNull();
        resourceCompany.Name.Should().Be(updateRequest.Name);
        resourceCompany.CompanyId.Should().Be(updateRequest.CompanyId);
        resourceCompany.Type.Should().Be(updateRequest.Type);
        resourceCompany.Source.Should().Be(updateRequest.Source);
        resourceCompany.Status.Should().Be(updateRequest.Status);
    }

    [Fact]
    public async Task DeleteCompanyResource_ShouldDeleteResourceFromCompany()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);

        // Act
        await _resourceService.DeleteCompanyResource(resource.Id, company.Id);

        // Assert
        var companyResources = (await _resourceService.GetCompanyResources(resource.CompanyId)).ToList();
        companyResources.Count.Should().Be(0);
    }

    [Fact]
    public async Task GetResource_ShouldReturnResource()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var resource = await _fixture.CreateResource(company.Id);

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
        var company1 = await _fixture.CreateCompany();
        await _fixture.CreateResource(company1.Id);
        var company2 = await _fixture.CreateCompany();
        await _fixture.CreateResource(company2.Id);

        // Act
        var result = await _resourceService.GetAllResources();

        // Assert
        var resourceResponses = result.ToList();
        resourceResponses.Should().NotBeEmpty();
        resourceResponses.Count.Should().BeGreaterThanOrEqualTo(2);
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
        resourceResponses.Should().Contain(r => r.CompanyId == company.Id);
    }
}
using Application.Exceptions;
using Application.Requests;
using Application.Services;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationIntegrationTests.Service;

public class CompanyServiceTests : IClassFixture<TestingFixture>
{
    private readonly TestingFixture _fixture;
    private readonly ICompanyService _companyService;
    private readonly Faker _faker;

    public CompanyServiceTests(TestingFixture fixture)
    {
        _fixture = fixture;
        var scope = fixture.ServiceProvider.CreateScope();
        _companyService = scope.ServiceProvider.GetRequiredService<ICompanyService>();
        _faker = new Faker();
    }

    [Fact]
    public async Task Add_ShouldCreateCompany()
    {
        // Arrange
        var request = new CreateCompanyRequest { CompanyName = _faker.Company.CompanyName() };

        // Act
        await _companyService.Add(request);

        // Assert
        var companies = (await _companyService.GetAllCompanies()).ToList();
        companies.Should().Contain(c => c.CompanyName == request.CompanyName);
    }

    [Fact]
    public async Task Update_ShouldUpdateCompany()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var request = new UpdateCompanyRequest { Id = company.Id, CompanyName = _faker.Company.CompanyName() };

        // Act
        await _companyService.Update(request);

        // Assert
        var result = await _companyService.GetCompany(request.Id);
        result.CompanyName.Should().Be(request.CompanyName);
    }

    [Fact]
    public async Task Delete_ShouldRemoveCompany()
    {
        // Arrange
        var company = await _fixture.CreateCompany();

        // Act
        await _companyService.Delete(company.Id);

        // Assert
        await _companyService.Invoking(i => i.GetCompany(company.Id))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("Company not found");
    }

    [Fact]
    public async Task AddUserToCompany_ShouldAddUser()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var user = await _fixture.CreateUser(company.Id);

        // Act
        await _companyService.AddUserToCompany(user.Id, company.Id);

        // Assert
        var users = (await _companyService.GetCompanyUsers(company.Id)).ToList();
        users.Should().Contain(u => u.Username == user.Username);
    }

    [Fact]
    public async Task DeleteUserFromCompany_ShouldRemoveUser()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        var user = await _fixture.CreateUser(company.Id);
        await _companyService.AddUserToCompany(user.Id, company.Id);

        // Act
        await _companyService.DeleteUserFromCompany(user.Id, company.Id);

        // Assert
        var usersCompany = (await _companyService.GetCompanyUsers(company.Id)).ToList();
        usersCompany.Should().NotContain(u => u.Username == user.Username);
    }

    [Fact]
    public async Task GetCompany_ShouldReturnCompany()
    {
        // Arrange
        var company = await _fixture.CreateCompany();

        // Act
        var response = await _companyService.GetCompany(company.Id);

        // Assert
        response.Should().NotBeNull();
        response.CompanyName.Should().Be(company.CompanyName);
    }

    [Fact]
    public async Task GetAllCompanies_ShouldReturnAllCompanies()
    {
        // Arrange
        var company1 = await _fixture.CreateCompany();
        var company2 = await _fixture.CreateCompany();

        // Act
        var companies = (await _companyService.GetAllCompanies()).ToList();

        // Assert
        companies.Should().HaveCountGreaterThanOrEqualTo(2);
        companies.Should().Contain(i => i.CompanyName == company2.CompanyName);
        companies.Should().Contain(i => i.CompanyName == company1.CompanyName);
    }

    [Fact]
    public async Task GetCompanyUsers_ShouldReturnUsers()
    {
        // Arrange
        var company = await _fixture.CreateCompany();
        
        var user1 = await _fixture.CreateUser(company.Id);
        await _companyService.AddUserToCompany(user1.Id, company.Id);
        
        var user2 = await _fixture.CreateUser(company.Id);
        await _companyService.AddUserToCompany(user2.Id, company.Id);

        // Act
        var users = (await _companyService.GetCompanyUsers(company.Id)).ToList();

        // Assert
        users.Should().HaveCountGreaterThanOrEqualTo(2);
        users.Should().Contain(u => u.Username == user1.Username);
        users.Should().Contain(u => u.Username == user2.Username);
    }
}
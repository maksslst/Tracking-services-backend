using Application.Exceptions;
using Application.Mappings;
using Application.Requests;
using Application.Services;
using AutoMapper;
using Bogus;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Repositories.CompanyRepository;
using Moq;
using Microsoft.Extensions.Logging;

namespace ApplicationUnitTests.Services;

public class CompanyServiceTests
{
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly ICompanyService _companyService;
    private readonly Faker _faker;

    public CompanyServiceTests()
    {
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        var loggerMock = new Mock<ILogger<CompanyService>>();

        var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = mappingConfig.CreateMapper();

        _companyService = new CompanyService(_companyRepositoryMock.Object, mapper, loggerMock.Object);
        _faker = new Faker();
    }

    #region AddTests

    [Fact]
    public async Task Add_WhenValidRequest_ReturnsCompanyId()
    {
        // Arrange
        var request = new CreateCompanyRequest()
        {
            CompanyName = _faker.Company.CompanyName(),
        };

        _companyRepositoryMock.Setup(i => i.CreateCompany(It.IsAny<Company>())).ReturnsAsync(1);

        // Act
        var result = await _companyService.Add(request);

        // Assert
        result.Should().Be(1);
        _companyRepositoryMock.Verify(i => i.CreateCompany(It.Is<Company>(x => x.CompanyName == request.CompanyName)),
            Times.Once);
    }

    [Fact]
    public async Task AddUserToCompany_WhenUserAndCompanyExist_AddsUserSuccessfully()
    {
        // Arrange
        _companyRepositoryMock.Setup(i => i.AddUserToCompany(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        // Act
        await _companyService.AddUserToCompany(1, 3);

        // Assert
        _companyRepositoryMock.Verify(i => i.AddUserToCompany(1, 3), Times.Once);
    }

    [Fact]
    public async Task AddUserToCompany_WhenUserOrCompanyNotFound_ShouldThrowEntityCreateException()
    {
        // Arrange
        _companyRepositoryMock.Setup(i => i.AddUserToCompany(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        // Act & Assert
        await _companyService.Invoking(i => i.AddUserToCompany(1, 3))
            .Should()
            .ThrowAsync<EntityCreateException>()
            .WithMessage("Couldn't add user to company");

        _companyRepositoryMock.Verify(i => i.AddUserToCompany(1, 3), Times.Once);
    }

    #endregion

    #region UpdateTests

    [Fact]
    public async Task Update_WhenCompanyExists_UpdatesCompanySuccessfully()
    {
        // Arrange
        var request = new UpdateCompanyRequest()
        {
            Id = 1,
            CompanyName = _faker.Company.CompanyName(),
        };

        var response = CompanyCreation(request.Id);

        _companyRepositoryMock.Setup(i => i.ReadByCompanyId(It.IsAny<int>())).ReturnsAsync(response);
        _companyRepositoryMock.Setup(i => i.UpdateCompany(It.IsAny<Company>())).ReturnsAsync(true);

        // Act
        await _companyService.Update(request);

        // Assert
        _companyRepositoryMock.Verify(i => i.ReadByCompanyId(request.Id), Times.Once);
        _companyRepositoryMock.Verify(i =>
                i.UpdateCompany(It.Is<Company>(x => x.Id == request.Id && x.CompanyName == request.CompanyName)),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenCompanyNotFound_ShouldThrowEntityUpdateException()
    {
        // Arrange
        var request = new UpdateCompanyRequest()
        {
            Id = 1,
            CompanyName = _faker.Company.CompanyName(),
        };

        var response = CompanyCreation(request.Id);
        request.CompanyName = request.CompanyName;

        _companyRepositoryMock.Setup(i => i.ReadByCompanyId(It.IsAny<int>())).ReturnsAsync(response);
        _companyRepositoryMock.Setup(i => i.UpdateCompany(It.IsAny<Company>())).ReturnsAsync(false);

        // Act & Assert
        await _companyService.Invoking(i => i.Update(request))
            .Should()
            .ThrowAsync<EntityUpdateException>()
            .WithMessage("Couldn't update company");

        _companyRepositoryMock.Verify(i => i.ReadByCompanyId(request.Id), Times.Once);
        _companyRepositoryMock.Verify(
            i => i.UpdateCompany(It.Is<Company>(c => c.Id == request.Id && c.CompanyName == request.CompanyName)),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenUserNotFound_ShouldThrowsNotFoundApplicationException()
    {
        // Arrange
        var request = new UpdateCompanyRequest()
        {
            Id = 1,
            CompanyName = _faker.Company.CompanyName(),
        };
        _companyRepositoryMock.Setup(i => i.ReadByCompanyId(It.IsAny<int>())).ReturnsAsync((Company)null!);

        // Act & Assert
        await _companyService.Invoking(i => i.Update(request))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("Company not found");

        _companyRepositoryMock.Verify(i => i.UpdateCompany(It.IsAny<Company>()), Times.Never);
    }

    #endregion

    #region DeleteTests

    [Fact]
    public async Task Delete_WhenCompanyExists_DeletesCompanySuccessfully()
    {
        // Arrange
        _companyRepositoryMock.Setup(i => i.DeleteCompany(It.IsAny<int>())).ReturnsAsync(true);

        // Act
        await _companyService.Delete(1);

        // Assert
        _companyRepositoryMock.Verify(i => i.DeleteCompany(1), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeletionFails_ShouldThrowEntityDeleteException()
    {
        // Arrange
        _companyRepositoryMock.Setup(i => i.DeleteCompany(It.IsAny<int>())).ReturnsAsync(false);

        // Act & Assert
        await _companyService.Invoking(i => i.Delete(1))
            .Should()
            .ThrowAsync<EntityDeleteException>()
            .WithMessage("Couldn't delete company");
    }

    [Fact]
    public async Task DeleteUserFromCompany_ShouldSuccessfullyDeleteUserFromCompany()
    {
        // Arrange
        _companyRepositoryMock.Setup(i => i.RemoveUserFromCompany(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        // Act
        await _companyService.DeleteUserFromCompany(1, 3);

        // Assert
        _companyRepositoryMock.Verify(i => i.RemoveUserFromCompany(1, 3), Times.Once);
    }

    [Fact]
    public async Task DeleteUserFromCompany_WhenUserOrCompanyNotFound_ShouldThrowEntityDeleteException()
    {
        // Arrange
        _companyRepositoryMock.Setup(i => i.RemoveUserFromCompany(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        // Act & Assert
        await _companyService.Invoking(i => i.DeleteUserFromCompany(1, 3))
            .Should()
            .ThrowAsync<EntityDeleteException>()
            .WithMessage("Couldn't delete user from company");

        _companyRepositoryMock.Verify(i => i.RemoveUserFromCompany(1, 3), Times.Once);
    }

    #endregion

    #region GetTests

    [Fact]
    public async Task GetCompanyGetAllMetricValuesForResource()
    {
        // Arrange
        var company = CompanyCreation(_faker.Random.Int(1, 100));

        _companyRepositoryMock.Setup(i => i.ReadByCompanyId(company.Id)).ReturnsAsync(company);

        // Act
        var result = await _companyService.GetCompany(company.Id);

        // Assert
        result.Should().NotBeNull();
        result.CompanyName.Should().Be(company.CompanyName);
        _companyRepositoryMock.Verify(i => i.ReadByCompanyId(company.Id), Times.Once);
    }

    [Fact]
    public async Task GetCompany_WhenCompanyNotFound_ShouldThrowNotFoundApplicationException()
    {
        // Arrange
        var companyId = new Random().Next(1, int.MaxValue);
        _companyRepositoryMock.Setup(i => i.ReadByCompanyId(It.IsAny<int>())).ReturnsAsync((Company)null!);

        // Act & Assert
        await _companyService.Invoking(i => i.GetCompany(companyId))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("Company not found");

        _companyRepositoryMock.Verify(i => i.ReadByCompanyId(companyId), Times.Once);
    }

    [Fact]
    public async Task GetAllCompanies_ShouldReturnsCompanyResponses()
    {
        // Arrange
        var companies = new List<Company>()
        {
            CompanyCreation(_faker.Random.Int(1)),
            CompanyCreation(_faker.Random.Int(2))
        };

        _companyRepositoryMock.Setup(i => i.ReadAllCompanies()).ReturnsAsync(companies);

        // Act
        var result = await _companyService.GetAllCompanies();

        // Assert
        result.Should().HaveCount(2);
        _companyRepositoryMock.Verify(i => i.ReadAllCompanies(), Times.Once);
    }

    [Fact]
    public async Task GetAllCompanies_WhenNoCompaniesExist_ReturnsEmptyCollection()
    {
        // Arrange
        _companyRepositoryMock.Setup(i => i.ReadAllCompanies()).ReturnsAsync(new List<Company>());

        // Act
        var result = await _companyService.GetAllCompanies();

        // Assert
        result.Should().BeEmpty();
        _companyRepositoryMock.Verify(i => i.ReadAllCompanies(), Times.Once);
    }

    [Fact]
    public async Task GetCompanyUsers_GetAllMetricValuesForResource()
    {
        // Arrange
        var company = CompanyCreation(_faker.Random.Int(1, 100));

        var users = new List<User>()
        {
            new User()
            {
                Id = 1,
                CompanyId = company.Id,
                Email = _faker.Person.Email,
                FirstName = _faker.Person.FirstName,
                LastName = _faker.Person.LastName,
                Username = _faker.Person.UserName,
                PasswordHash = _faker.Random.String()
            },
            new User()
            {
                Id = 2,
                CompanyId = company.Id,
                Email = _faker.Person.Email,
                FirstName = _faker.Person.FirstName,
                LastName = _faker.Person.LastName,
                Username = _faker.Person.UserName,
                PasswordHash = _faker.Random.String()
            }
        };

        _companyRepositoryMock.Setup(i => i.ReadByCompanyId(company.Id)).ReturnsAsync(company);
        _companyRepositoryMock.Setup(i => i.ReadCompanyUsers(It.IsAny<int>())).ReturnsAsync(users);

        // Act
        var result = (await _companyService.GetCompanyUsers(company.Id)).ToList();

        // Assert
        result.Should().HaveCount(2);
        _companyRepositoryMock.Verify(i => i.ReadByCompanyId(company.Id), Times.Once);
        _companyRepositoryMock.Verify(i => i.ReadCompanyUsers(company.Id), Times.Once);
    }

    [Fact]
    public async Task GetCompanyUsers_WhenCompanyNotFound_ShouldThrowNotFoundApplicationException()
    {
        // Arrange
        var companyId = 1;
        _companyRepositoryMock.Setup(i => i.ReadByCompanyId(It.IsAny<int>())).ReturnsAsync((Company)null!);

        // Act & Assert
        await _companyService.Invoking(i => i.GetCompanyUsers(companyId))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("Company not found");

        _companyRepositoryMock.Verify(i => i.ReadByCompanyId(companyId), Times.Once);
        _companyRepositoryMock.Verify(i => i.ReadCompanyUsers(companyId), Times.Never);
    }

    #endregion

    private Company CompanyCreation(int id)
    {
        var company = new Company()
        {
            Id = id,
            CompanyName = _faker.Company.CompanyName(),
            Resources = [],
            Users = []
        };

        return company;
    }
}
using Api.Controllers;
using Application.Requests;
using Application.Responses;
using Application.Services;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Controllers;

public class CompanyControllerTests
{
    private readonly Mock<ICompanyService> _companyServiceMock;
    private readonly CompanyController _controller;
    private readonly Faker _faker;

    public CompanyControllerTests()
    {
        _companyServiceMock = new Mock<ICompanyService>();
        _controller = new CompanyController(_companyServiceMock.Object);
        _faker = new Faker();
    }

    #region AddTests

    [Fact]
    public async Task Add_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateCompanyRequest() { CompanyName = _faker.Company.CompanyName() };
        var companyId = _faker.Random.Int(1, 100);
        _companyServiceMock.Setup(x => x.Add(request)).ReturnsAsync(companyId);

        // Act
        var result = await _controller.Add(request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult?.ActionName.Should().Be(nameof(_controller.GetByCompanyId));
        createdResult?.RouteValues?["companyId"].Should().Be(companyId);
        createdResult?.Value.Should().Be(companyId);
        _companyServiceMock.Verify(x => x.Add(request), Times.Once());
    }

    [Fact]
    public async Task AddUserToCompany_ValidIds_ReturnsNoContent()
    {
        // Arrange
        var userId = _faker.Random.Int(1, 100);
        var companyId = _faker.Random.Int(1, 100);
        _companyServiceMock.Setup(x => x.AddUserToCompany(userId, companyId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddUserToCompany(userId, companyId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _companyServiceMock.Verify(x => x.AddUserToCompany(userId, companyId), Times.Once());
    }

    #endregion

    #region GetTests

    [Fact]
    public async Task Update_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new UpdateCompanyRequest
            { Id = _faker.Random.Int(1, 100), CompanyName = _faker.Company.CompanyName() };
        _companyServiceMock.Setup(x => x.Update(request)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _companyServiceMock.Verify(x => x.Update(request), Times.Once());
    }

    #endregion

    #region DeleteTests

    [Fact]
    public async Task Delete_ExistingCompany_ReturnsNoContent()
    {
        // Arrange
        var companyId = _faker.Random.Int(1, 100);
        _companyServiceMock.Setup(x => x.Delete(companyId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(companyId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _companyServiceMock.Verify(x => x.Delete(companyId), Times.Once());
    }

    [Fact]
    public async Task DeleteUserFromCompany_ValidIds_ReturnsNoContent()
    {
        // Arrange
        var userId = _faker.Random.Int(1, 100);
        var companyId = _faker.Random.Int(1, 100);
        _companyServiceMock.Setup(x => x.DeleteUserFromCompany(userId, companyId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteUserFromCompany(userId, companyId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _companyServiceMock.Verify(x => x.DeleteUserFromCompany(userId, companyId), Times.Once());
    }

    #endregion

    #region GetTests

    [Fact]
    public async Task GetByCompanyId_ExistingCompany_ReturnsOkWithCompany()
    {
        // Arrange
        var companyId = _faker.Random.Int(1, 100);
        var company = new CompanyResponse() { CompanyName = _faker.Company.CompanyName() };
        _companyServiceMock.Setup(x => x.GetCompany(companyId)).ReturnsAsync(company);

        // Act
        var result = await _controller.GetByCompanyId(companyId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(company);
        _companyServiceMock.Verify(x => x.GetCompany(companyId), Times.Once());
    }

    [Fact]
    public async Task GetAllCompanies_WithCompanies_ReturnsOkWithCompanies()
    {
        // Arrange
        var companies = new List<CompanyResponse>
        {
            new CompanyResponse() { CompanyName = _faker.Company.CompanyName() },
            new CompanyResponse() { CompanyName = _faker.Company.CompanyName() }
        };
        _companyServiceMock.Setup(x => x.GetAllCompanies()).ReturnsAsync(companies);

        // Act
        var result = await _controller.GetAllCompanies();

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(companies);
        _companyServiceMock.Verify(x => x.GetAllCompanies(), Times.Once());
    }

    [Fact]
    public async Task GetCompanyUsers_WithUsers_ReturnsOkWithUsers()
    {
        // Arrange
        var companyId = _faker.Random.Int(1, 100);
        var users = new List<UserResponse>
        {
            new UserResponse()
            {
                CompanyId = companyId,
                Email = _faker.Person.Email,
                FirstName = _faker.Person.FirstName,
                LastName = _faker.Person.LastName,
                Username = _faker.Person.UserName
            },
            new UserResponse()
            {
                CompanyId = companyId,
                Email = _faker.Person.Email,
                FirstName = _faker.Person.FirstName,
                LastName = _faker.Person.LastName,
                Username = _faker.Person.UserName
            },
        };
        _companyServiceMock.Setup(x => x.GetCompanyUsers(companyId)).ReturnsAsync(users);

        // Act
        var result = await _controller.GetCompanyUsers(companyId);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(users);
        _companyServiceMock.Verify(x => x.GetCompanyUsers(companyId), Times.Once());
    }

    #endregion
}
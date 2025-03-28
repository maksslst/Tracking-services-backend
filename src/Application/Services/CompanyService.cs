using Application.DTOs.Mappings;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.CompanyRepository;
using Infrastructure.Repositories.UserRepository;
using Application.Requests;
using Application.Responses;

namespace Application.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CompanyService(ICompanyRepository companyRepository, IMapper mapper, IUserRepository userRepository)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<Company?> Add(CreateCompanyRequest request)
    {
        var campany = new Company()
        {
            CompanyName = request.CompanyName
        };

        await _companyRepository.CreateCompany(campany);
        return campany;
    }

    public async Task<bool> Update(UpdateCompanyRequest request)
    {
        if (await _companyRepository.ReadByCompanyId(request.CompanyId) == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var company = new Company()
        {
            Id = request.CompanyId,
            CompanyName = request.CompanyName
        };

        var users = company.Users;
        company.Users = users;
        var resources = company.Resources;
        company.Resources = resources;

        return await _companyRepository.UpdateCompany(company);
    }

    public async Task<bool> Delete(int companyId)
    {
        if (await _companyRepository.ReadByCompanyId(companyId) == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        return await _companyRepository.DeleteCompany(companyId);
    }

    public async Task<bool> AddUserToCompany(int userId, int companyId)
    {
        if (await _companyRepository.ReadByCompanyId(companyId) == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var user = _userRepository.ReadById(userId).Result;
        if (user == null)
        {
            throw new NotFoundApplicationException("User not found");
        }

        return await _companyRepository.AddUserToCompany(user, companyId);
    }

    public async Task<bool> DeleteUserFromCompany(int userId, int companyId)
    {
        if (await _companyRepository.ReadByCompanyId(companyId) == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var user = _userRepository.ReadById(userId).Result;
        if (user == null)
        {
            throw new NotFoundApplicationException("User not found");
        }

        return await _companyRepository.RemoveUserFromCompany(user, companyId);
    }

    public async Task<CompanyResponse?> GetCompany(int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var companyResponse = new CompanyResponse()
        {
            CompanyName = company.CompanyName
        };
        return companyResponse;
    }

    public async Task<IEnumerable<CompanyResponse?>> GetAllCompanies()
    {
        var companies = await _companyRepository.ReadAllCompanies();
        if (companies == null || companies.Count() == 0)
        {
            throw new NotFoundApplicationException("Companies not found");
        }

        var companiesResponse = new List<CompanyResponse>();
        foreach (var company in companies)
        {
            companiesResponse.Add(new CompanyResponse()
            {
                CompanyName = company.CompanyName
            });
        }

        return companiesResponse;
    }

    public async Task<IEnumerable<UserResponse?>> GetCompanyUsers(int companyId)
    {
        if (await _companyRepository.ReadByCompanyId(companyId) == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var users = await _companyRepository.ReadCompanyUsers(companyId);
        if (users == null || users.Count() == 0)
        {
            throw new NotFoundApplicationException("Users not found");
        }

        var usersResponse = new List<UserResponse>();
        foreach (var user in users)
        {
            usersResponse.Add(new UserResponse()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Patronymic = user.Patronymic,
                Username = user.Username,
                CompanyId = user.CompanyId
            });
        }

        return usersResponse;
    }
}
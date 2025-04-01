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
        var company = new Company()
        {
            CompanyName = request.CompanyName
        };

        await _companyRepository.CreateCompany(company);
        return company;
    }

    public async Task<bool> Update(UpdateCompanyRequest request)
    {
        var companyToUpdate = await _companyRepository.ReadByCompanyId(request.CompanyId);
        if (companyToUpdate == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var company = new Company()
        {
            Id = request.CompanyId,
            CompanyName = request.CompanyName
        };
        var users = companyToUpdate.Users;
        company.Users = users;
        var resources = companyToUpdate.Resources;
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

        return _mapper.Map<CompanyResponse>(company);
    }

    public async Task<IEnumerable<CompanyResponse?>> GetAllCompanies()
    {
        var companies = await _companyRepository.ReadAllCompanies();
        if (companies == null || companies.Count() == 0)
        {
            throw new NotFoundApplicationException("Companies not found");
        }

        var companiesResponse = companies.Select(i => _mapper.Map<CompanyResponse>(i));
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

        var usersResponse = users.Select(i => _mapper.Map<UserResponse>(i));
        return usersResponse;
    }
}
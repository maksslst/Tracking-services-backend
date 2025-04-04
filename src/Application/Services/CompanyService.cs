using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.CompanyRepository;
using Infrastructure.Repositories.UserRepository;
using Application.Requests;
using Application.Responses;
using Npgsql;

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

    public async Task<int> Add(CreateCompanyRequest request)
    {
        var company = _mapper.Map<Company>(request);
        return await _companyRepository.CreateCompany(company);
    }

    public async Task Update(UpdateCompanyRequest request)
    {
        var companyToUpdate = await _companyRepository.ReadByCompanyId(request.Id);
        if (companyToUpdate == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        companyToUpdate.CompanyName = request.CompanyName;
        bool isUpdated = await _companyRepository.UpdateCompany(companyToUpdate);
        if (!isUpdated)
        {
            throw new EntityUpdateException("Couldn't update company");
        }
    }

    public async Task Delete(int companyId)
    {
        bool isDeleted = await _companyRepository.DeleteCompany(companyId);
        if (!isDeleted)
        {
            throw new EntityDeleteException("Couldn't delete company");
        }
    }

    public async Task AddUserToCompany(int userId, int companyId)
    {
        bool isAdded = await _companyRepository.AddUserToCompany(userId, companyId);
        if (!isAdded)
        {
            throw new EntityCreateException("Couldn't add user to company");
        }
    }

    public async Task DeleteUserFromCompany(int userId, int companyId)
    {
        bool isDeleted = await _companyRepository.RemoveUserFromCompany(userId, companyId);
        if (!isDeleted)
        {
            throw new EntityDeleteException("Couldn't delete user from company");
        }
    }

    public async Task<CompanyResponse> GetCompany(int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        return _mapper.Map<CompanyResponse>(company);
    }

    public async Task<IEnumerable<CompanyResponse>> GetAllCompanies()
    {
        var companies = await _companyRepository.ReadAllCompanies();
        var companiesResponse = companies.Select(i => _mapper.Map<CompanyResponse>(i));
        return companiesResponse;
    }

    public async Task<IEnumerable<UserResponse>> GetCompanyUsers(int companyId)
    {
        if (await _companyRepository.ReadByCompanyId(companyId) == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var users = await _companyRepository.ReadCompanyUsers(companyId);
        var usersResponse = users.Select(i => _mapper.Map<UserResponse>(i));
        return usersResponse;
    }
}
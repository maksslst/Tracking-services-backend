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
        try
        {
            var company = new Company()
            {
                CompanyName = request.CompanyName
            };

            return await _companyRepository.CreateCompany(company);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't create a company");
        }
    }

    public async Task<bool> Update(UpdateCompanyRequest request)
    {
        try
        {
            var companyToUpdate = await _companyRepository.ReadByCompanyId(request.CompanyId);
            if (companyToUpdate == null)
            {
                throw new NotFoundApplicationException("Company not found");
            }

            companyToUpdate.CompanyName = request.CompanyName;
            return await _companyRepository.UpdateCompany(companyToUpdate);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't update a company");
        }
    }

    public async Task<bool> Delete(int companyId)
    {
        try
        {
            bool isDeleted = await _companyRepository.DeleteCompany(companyId);
            if (!isDeleted)
            {
                throw new NotFoundApplicationException("Company not found");
            }

            return true;

        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't delete a company");
        }
    }

    public async Task<bool> AddUserToCompany(int userId, int companyId)
    {
        try
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
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't add user to company");
        }
    }

    public async Task<bool> DeleteUserFromCompany(int userId, int companyId)
    {
        try
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
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't delete user from company");
        }
    }

    public async Task<CompanyResponse> GetCompany(int companyId)
    {
        try
        {
            var company = await _companyRepository.ReadByCompanyId(companyId);
            if (company == null)
            {
                throw new NotFoundApplicationException("Company not found");
            }

            return _mapper.Map<CompanyResponse>(company);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't find a company");
        }
    }

    public async Task<IEnumerable<CompanyResponse>> GetAllCompanies()
    {
        try
        {
            var companies = await _companyRepository.ReadAllCompanies();
            if (companies == null || companies.Count() == 0)
            {
                return new CompanyResponse[] { };
            }

            var companiesResponse = companies.Select(i => _mapper.Map<CompanyResponse>(i));
            return companiesResponse;
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't find companies");
        }
    }

    public async Task<IEnumerable<UserResponse>> GetCompanyUsers(int companyId)
    {
        try
        {
            if (await _companyRepository.ReadByCompanyId(companyId) == null)
            {
                throw new NotFoundApplicationException("Company not found");
            }

            var users = await _companyRepository.ReadCompanyUsers(companyId);
            if (users == null || users.Count() == 0)
            {
                return new UserResponse[] { };
            }

            var usersResponse = users.Select(i => _mapper.Map<UserResponse>(i));
            return usersResponse;
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't find company users");
        }
    }
}
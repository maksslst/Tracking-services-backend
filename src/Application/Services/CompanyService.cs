using Application.DTOs.Mappings;
using Domain.Entities;
using Infrastructure.Repositories;
using AutoMapper;

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

    public async Task<Company?> Add(CompanyDto companyDto)
    {
        Company mappedCompany = _mapper.Map<Company>(companyDto);
        if (mappedCompany == null)
        {
            return null;
        }

        Company company = await _companyRepository.CreateCompany(mappedCompany);
        return company;
    }

    public async Task<bool> Update(CompanyDto companyDto)
    {
        Company mappedCompany = _mapper.Map<Company>(companyDto);
        if (mappedCompany == null)
        {
            return false;
        }

        List<UserDto> users = companyDto.Users;
        foreach (var user in users)
        {
            if (_userRepository.ReadById(user.Id).Result != null)
            {
                return false;
            }
        }

        return await _companyRepository.UpdateCompany(mappedCompany);
    }

    public Task<bool> Delete(int companyId)
    {
        return _companyRepository.DeleteCompany(companyId);
    }

    public async Task<bool> AddUserToCompany(int userId, int companyId)
    {
        User? user = _userRepository.ReadById(userId).Result;
        if (user == null)
        {
            return false;
        }

        return await _companyRepository.AddUserToCompany(user, companyId);
    }

    public async Task<bool> DeleteUserFromCompany(int userId, int companyId)
    {
        User? user = _userRepository.ReadById(userId).Result;
        if (user == null)
        {
            return false;
        }

        return await _companyRepository.RemoveUserFromCompany(user, companyId);
    }

    public async Task<CompanyDto?> GetCompany(int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        CompanyDto mappedCompany = _mapper.Map<CompanyDto>(company);
        return mappedCompany;
    }

    public async Task<IEnumerable<CompanyDto?>> GetAllCompanies()
    {
        IEnumerable<Company?> companies = await _companyRepository.ReadAllCompanies();
        IEnumerable<CompanyDto> mappedCompanies = companies.Select(i => _mapper.Map<CompanyDto>(i));
        return mappedCompanies;
    }

    public async Task<IEnumerable<UserDto?>> GetCompanyUsers(int companyId)
    {
        IEnumerable<User?> users = await _companyRepository.ReadCompanyUsers(companyId);
        IEnumerable<UserDto> mappedUsers = users.Select(i => _mapper.Map<UserDto>(i));
        return mappedUsers;
    }
}
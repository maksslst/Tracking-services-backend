using Application.DTOs.Mappings;
using Domain.Entities;
using Infrastructure.Repositories;
using AutoMapper;

namespace Application.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public CompanyService(ICompanyRepository companyRepository, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task Add(CompanyDto companyDto)
    {
        Company mappedCompany = _mapper.Map<Company>(companyDto);
        if (mappedCompany != null)
        {
            await _companyRepository.CreateCompany(mappedCompany);
        }
    }

    public async Task<bool> Update(CompanyDto companyDto)
    {
        Company mappedCompany = _mapper.Map<Company>(companyDto);
        return await _companyRepository.UpdateCompany(mappedCompany);
    }

    public Task<bool> Delete(int companyId)
    {
        return _companyRepository.DeleteCompany(companyId);
    }

    public Task<bool> AddUserToCompany(int userId, int companyId)
    {
        return _companyRepository.AddUserToCompany(userId, companyId);
    }

    public Task<bool> DeleteUserFromCompany(int userId, int companyId)
    {
        return _companyRepository.RemoveUserFromCompany(userId, companyId);
    }

    public async Task<CompanyDto?> GetByCompanyId(int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        CompanyDto mappedCompany = _mapper.Map<CompanyDto>(company);
        return mappedCompany;
    }

    public async Task<List<CompanyDto?>> GetAllCompany()
    {
        List<Company?> companies = await _companyRepository.ReadAllCompanies();
        List<CompanyDto> mappedCompany = companies.Select(i => _mapper.Map<CompanyDto>(i)).ToList();
        return mappedCompany;
    }

    public async Task<List<UserDto?>> GetCompanyUsers(int companyId)
    {
        List<User?> users = await _companyRepository.ReadCompanyUsers(companyId);
        List<UserDto> mappedUsers = users.Select(i => _mapper.Map<UserDto>(i)).ToList();
        return mappedUsers;
    }
}
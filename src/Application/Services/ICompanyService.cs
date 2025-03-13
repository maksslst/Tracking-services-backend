using Application.DTOs.Mappings;
using Domain.Entities;

namespace Application.Services;

public interface ICompanyService
{
    public Task<Company?> Add(CompanyDto companyDto);
    public Task<bool> Update(CompanyDto companyDto);
    public Task<bool> Delete(int companyId);
    public Task<bool> AddUserToCompany(int userId, int companyId);
    public Task<bool> DeleteUserFromCompany(int userId, int companyId);
    public Task<CompanyDto?> GetCompany(int companyId);
    public Task<IEnumerable<CompanyDto?>> GetAllCompanies();
    public Task<IEnumerable<UserDto?>> GetCompanyUsers(int companyId);
}
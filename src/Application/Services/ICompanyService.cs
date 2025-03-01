using Application.DTOs.Mappings;

namespace Application.Services;

public interface ICompanyService
{
    public Task Add(CompanyDto companyDto);
    public Task<bool> Update(CompanyDto companyDto);
    public Task<bool> Delete(int companyId);
    public Task<bool> AddUserToCompany(int userId, int companyId);
    public Task<bool> DeleteUserFromCompany(int userId, int companyId);
    public Task<CompanyDto?> GetByCompanyId(int companyId);
    public Task<List<CompanyDto?>> GetAllCompany();
    public Task<List<UserDto?>> GetCompanyUsers(int companyId);
}
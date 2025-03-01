using Application.DTOs.Mappings;

namespace Application.Services;

public class CompanyService : ICompanyService
{
    public Task Add(CompanyDto companyDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(CompanyDto companyDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(int companyId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddUserToCompany(int userId, int companyId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUserFromCompany(int userId, int companyId)
    {
        throw new NotImplementedException();
    }

    public Task<CompanyDto?> GetByCompanyId(int companyId)
    {
        throw new NotImplementedException();
    }

    public Task<List<CompanyDto?>> GetAllCompany()
    {
        throw new NotImplementedException();
    }

    public Task<List<UserDto?>> GetCompanyUsers(int companyId)
    {
        throw new NotImplementedException();
    }
}
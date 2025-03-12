using Domain.Entities;

namespace Infrastructure.Repositories;

public interface ICompanyRepository
{
    public Task CreateCompany(Company company);
    public Task<bool> UpdateCompany(Company company);
    public Task<bool> DeleteCompany(int companyId);
    public Task<bool> AddUserToCompany(int userId, int companyId);
    public Task<bool> RemoveUserFromCompany(int userId, int companyId);
    public Task<Company?> ReadByCompanyId(int? companyId);
    public Task<List<Company?>> ReadAllCompanies();
    public Task<List<User?>> ReadCompanyUsers(int companyId);
}
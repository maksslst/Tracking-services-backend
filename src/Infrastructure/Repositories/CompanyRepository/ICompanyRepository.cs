using Domain.Entities;

namespace Infrastructure.Repositories.CompanyRepository;

public interface ICompanyRepository
{
    public Task<int> CreateCompany(Company company);
    public Task<bool> UpdateCompany(Company company);
    public Task<bool> DeleteCompany(int companyId);
    public Task<bool> AddUserToCompany(int userId, int companyId);
    public Task<bool> RemoveUserFromCompany(int userId, int companyId);
    public Task<Company?> ReadByCompanyId(int? companyId);
    public Task<IEnumerable<Company?>> ReadAllCompanies();
    public Task<IEnumerable<User?>> ReadCompanyUsers(int companyId);
}
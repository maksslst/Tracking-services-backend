using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface ICompanyService
{
    public Task<int> Add(CreateCompanyRequest request);
    public Task Update(UpdateCompanyRequest request);
    public Task Delete(int companyId);
    public Task AddUserToCompany(int userId, int companyId);
    public Task DeleteUserFromCompany(int userId, int companyId);
    public Task<CompanyResponse> GetCompany(int companyId);
    public Task<IEnumerable<CompanyResponse>> GetAllCompanies();
    public Task<IEnumerable<UserResponse>> GetCompanyUsers(int companyId);
}
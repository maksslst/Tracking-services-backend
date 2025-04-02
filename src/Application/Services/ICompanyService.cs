using Application.DTOs.Mappings;
using Domain.Entities;
using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface ICompanyService
{
    public Task<int> Add(CreateCompanyRequest request);
    public Task<bool> Update(UpdateCompanyRequest request);
    public Task<bool> Delete(int companyId);
    public Task<bool> AddUserToCompany(int userId, int companyId);
    public Task<bool> DeleteUserFromCompany(int userId, int companyId);
    public Task<CompanyResponse> GetCompany(int companyId);
    public Task<IEnumerable<CompanyResponse>> GetAllCompanies();
    public Task<IEnumerable<UserResponse>> GetCompanyUsers(int companyId);
}
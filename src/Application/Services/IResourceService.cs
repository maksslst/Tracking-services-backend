using Application.DTOs.Mappings;
using Domain.Entities;

namespace Application.Services;

public interface IResourceService
{
    public Task<Resource?> Add(ResourceDto resourceDto);
    public Task<bool> Update(ResourceDto resourceDto);
    public Task<bool> Delete(int serviceId);
    public Task<bool> AddCompanyService(int companyId, ResourceDto? serviceDto = null);
    public Task<bool> UpdateCompanyService(int companyId, ResourceDto resourceDto, int serviceUpdateId);
    public Task<bool> DeleteCompanyService(int serviceId, int companyId);
    public Task<ResourceDto?> GetService(int serviceId);
    public Task<IEnumerable<ResourceDto?>> GetAllServices();
    public Task<IEnumerable<ResourceDto?>> GetCompanyServices(int companyId);
}
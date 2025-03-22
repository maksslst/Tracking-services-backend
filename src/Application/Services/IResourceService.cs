using Application.DTOs.Mappings;
using Domain.Entities;

namespace Application.Services;

public interface IResourceService
{
    public Task<int?> Add(ResourceDto resourceDto);
    public Task<bool> Update(ResourceDto resourceDto);
    public Task<bool> Delete(int resourceId);
    public Task<bool> AddCompanyResource(int companyId, ResourceDto? resourceDto = null);
    public Task<bool> UpdateCompanyResource(int companyId, ResourceDto resourceDto, int resourceUpdateId);
    public Task<bool> DeleteCompanyResource(int resourceId, int companyId);
    public Task<ResourceDto?> GetResource(int resourceId);
    public Task<IEnumerable<ResourceDto?>> GetAllResources();
    public Task<IEnumerable<ResourceDto?>> GetCompanyResources(int companyId);
}
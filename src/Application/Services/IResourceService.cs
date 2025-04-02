using Application.DTOs.Mappings;
using Domain.Entities;
using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface IResourceService
{
    public Task<int> Add(CreateResourceRequest request);
    public Task<bool> Update(UpdateResourceRequest request);
    public Task<bool> Delete(int resourceId);
    public Task<bool> AddCompanyResource(int companyId, CreateResourceRequest request);
    public Task<bool> UpdateCompanyResource(int companyId, UpdateResourceRequest request, int resourceUpdateId);
    public Task<bool> DeleteCompanyResource(int resourceId, int companyId);
    public Task<ResourceResponse> GetResource(int resourceId);
    public Task<IEnumerable<ResourceResponse>> GetAllResources();
    public Task<IEnumerable<ResourceResponse>> GetCompanyResources(int companyId);
}
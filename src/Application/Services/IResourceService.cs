using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface IResourceService
{
    public Task<int> Add(CreateResourceRequest request);
    public Task Update(UpdateResourceRequest request);
    public Task Delete(int resourceId);
    public Task AddCompanyResource(int companyId, CreateResourceRequest request);
    public Task UpdateCompanyResource(int companyId, UpdateResourceRequest request, int resourceUpdateId);
    public Task DeleteCompanyResource(int resourceId, int companyId);
    public Task<ResourceResponse> GetResource(int resourceId);
    public Task<IEnumerable<ResourceResponse>> GetAllResources();
    public Task<IEnumerable<ResourceResponse>> GetCompanyResources(int companyId);
}
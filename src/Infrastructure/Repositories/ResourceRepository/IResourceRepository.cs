using Domain.Entities;

namespace Infrastructure.Repositories.ResourceRepository;

public interface IResourceRepository
{
    public Task<int> CreateResource(Resource resource);
    public Task<bool> UpdateResource(Resource resource);
    public Task<bool> DeleteResource(int resourceId);
    public Task<bool> AddCompanyResource(Resource resource);
    public Task<bool> DeleteCompanyResource(int resourceId, int companyId);
    public Task<Resource?> ReadByResourceId(int resourceId);
    public Task<IEnumerable<Resource?>> ReadAllResources();
    public Task<IEnumerable<Resource?>> ReadCompanyResources(int companyId);
}
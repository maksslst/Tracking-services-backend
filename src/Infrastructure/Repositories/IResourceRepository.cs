using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IResourceRepository
{
    public Task<Resource> CreateResource(Resource resource);
    public Task<bool> UpdateResource(Resource resource);
    public Task<bool> DeleteResource(int resourceId);
    public Task<bool> AddCompanyResource(Company company, Resource? resource = null);
    //public Task<bool> UpdateCompanyResource(Company company, Resource resource, int resourceUpdateId);
    public Task<bool> DeleteCompanyResource(int resourceId, Company company);
    public Task<Resource?> ReadByResourceId(int resourceId);
    public Task<IEnumerable<Resource?>> ReadAllResources();
    public Task<IEnumerable<Resource?>> ReadCompanyResources(Company company);
}
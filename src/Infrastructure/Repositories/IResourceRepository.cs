using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IResourceRepository
{
    public Task<Resource> CreateService(Resource resource);
    public Task<bool> UpdateService(Resource resource);
    public Task<bool> DeleteService(int serviceId);
    public Task<bool> AddCompanyService(Company company, Resource? service = null);
    public Task<bool> UpdateCompanyService(Company company, Resource resource, int serviceUpdateId);
    public Task<bool> DeleteCompanyService(int serviceId, Company company);
    public Task<Resource?> ReadByServiceId(int serviceId);
    public Task<IEnumerable<Resource?>> ReadAllServices();
    public Task<IEnumerable<Resource?>> ReadCompanyServices(Company company);
}
using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IServiceRepository
{
    public Task AddService(Service service);
    public Task<bool> UpdateService(Service service);
    public Task<bool> DeleteService(int serviceId);
    public Task<bool> AddCompanyService(int companyId, int serviceId = -1, Service? service = null);
    public Task<bool> UpdateCompanyService(int companyId, Service service, int serviceUpdateId);
    public Task<bool> DeleteCompanyService(int serviceId, int companyId);
    public Task<Service?> ReadByServiceId(int serviceId);
    public Task<List<Service?>> ReadAllServices();
    public Task<List<Service?>> ReadCompanyServices(int companyId);
}
using Application.DTOs.Mappings;

namespace Application.Services;

public class ServiceService : IServiceService
{
    public Task Add(ServiceDto serviceDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(ServiceDto serviceDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(int serviceId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddCompanyService(int companyId, int serviceId = -1, ServiceDto? serviceDto = default(ServiceDto?))
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateCompanyService(int companyId, ServiceDto serviceDto, int serviceUpdateId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteCompanyService(int serviceId, int companyId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceDto?> ReadByServiceId(int serviceId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ServiceDto?>> GetAllServices()
    {
        throw new NotImplementedException();
    }

    public Task<List<ServiceDto?>> GetCompanyServices(int companyId)
    {
        throw new NotImplementedException();
    }
}
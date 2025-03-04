using Application.DTOs.Mappings;

namespace Application.Services;

public interface IServiceService
{
    public Task Add(ServiceDto serviceDto);
    public Task<bool> Update(ServiceDto serviceDto);
    public Task<bool> Delete(int serviceId);
    public Task<bool> AddCompanyService(int companyId, int serviceId = -1, ServiceDto? serviceDto = null);
    public Task<bool> UpdateCompanyService(int companyId, ServiceDto serviceDto, int serviceUpdateId);
    public Task<bool> DeleteCompanyService(int serviceId, int companyId);
    public Task<ServiceDto?> ReadByServiceId(int serviceId);
    public Task<List<ServiceDto?>> GetAllServices();
    public Task<List<ServiceDto?>> GetCompanyServices(int companyId);
}
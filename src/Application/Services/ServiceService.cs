using Application.DTOs.Mappings;
using Infrastructure.Repositories;
using Domain.Entities;
using AutoMapper;

namespace Application.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IMapper _mapper;

    public ServiceService(IServiceRepository serviceRepository, IMapper mapper)
    {
        _serviceRepository = serviceRepository;
        _mapper = mapper;
    }

    public async Task Add(ServiceDto serviceDto)
    {
        Service mappedService = _mapper.Map<Service>(serviceDto);
        if (mappedService != null)
        {
            await _serviceRepository.AddService(mappedService);
        }
    }

    public async Task<bool> Update(ServiceDto serviceDto)
    {
        Service mappedService = _mapper.Map<Service>(serviceDto);
        return await _serviceRepository.UpdateService(mappedService);
    }

    public async Task<bool> Delete(int serviceId)
    {
        return await _serviceRepository.DeleteService(serviceId);
    }

    public async Task<bool> AddCompanyService(int companyId, int serviceId = -1, ServiceDto? serviceDto = null)
    {
        if (serviceDto != null)
        {
            Service mappedService = _mapper.Map<Service>(serviceDto);
            return await _serviceRepository.AddCompanyService(companyId, serviceId, mappedService);
        }

        return await _serviceRepository.AddCompanyService(companyId, serviceId);
    }

    public async Task<bool> UpdateCompanyService(int companyId, ServiceDto serviceDto, int serviceUpdateId)
    {
        Service mappedService = _mapper.Map<Service>(serviceDto);
        return await _serviceRepository.UpdateCompanyService(companyId, mappedService, serviceUpdateId);
    }

    public async Task<bool> DeleteCompanyService(int serviceId, int companyId)
    {
        return await _serviceRepository.DeleteCompanyService(serviceId, companyId);
    }

    public async Task<ServiceDto?> GetByServiceId(int serviceId)
    {
        Service? service = await _serviceRepository.ReadByServiceId(serviceId);
        ServiceDto mappedService = _mapper.Map<ServiceDto>(service);
        return mappedService;
    }

    public async Task<List<ServiceDto?>> GetAllServices()
    {
        List<Service?> services = await _serviceRepository.ReadAllServices();
        List<ServiceDto> mappedServices = services.Select(i => _mapper.Map<ServiceDto>(i)).ToList();
        return mappedServices;
    }

    public async Task<List<ServiceDto?>> GetCompanyServices(int companyId)
    {
        List<Service?> servicesCompany = await _serviceRepository.ReadCompanyServices(companyId);
        List<ServiceDto> mappedServicesCompany = servicesCompany.Select(i => _mapper.Map<ServiceDto>(i)).ToList();
        return mappedServicesCompany;
    }
}
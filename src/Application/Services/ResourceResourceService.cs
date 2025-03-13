using Application.DTOs.Mappings;
using Infrastructure.Repositories;
using Domain.Entities;
using AutoMapper;

namespace Application.Services;

public class ResourceResourceService : IResourceService
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IMapper _mapper;
    private readonly ICompanyRepository _companyRepository;

    public ResourceResourceService(IResourceRepository resourceRepository, IMapper mapper, ICompanyRepository companyRepository)
    {
        _resourceRepository = resourceRepository;
        _mapper = mapper;
        _companyRepository = companyRepository;
    }

    public async Task<Resource?> Add(ResourceDto resourceDto)
    {
        Resource mappedResource = _mapper.Map<Resource>(resourceDto);
        if (mappedResource != null)
        {
            await _resourceRepository.CreateService(mappedResource);
            return mappedResource;
        }

        return null;
    }

    public async Task<bool> Update(ResourceDto resourceDto)
    {
        Resource mappedResource = _mapper.Map<Resource>(resourceDto);
        if (mappedResource == null)
        {
            return false;
        }

        Company? company = await _companyRepository.ReadByCompanyId(resourceDto.CompanyId);
        if (company == null)
        {
            return false;
        }

        return await _resourceRepository.UpdateService(mappedResource);
    }

    public async Task<bool> Delete(int serviceId)
    {
        return await _resourceRepository.DeleteService(serviceId);
    }

    public async Task<bool> AddCompanyService(int companyId, ResourceDto? serviceDto)
    {
        Company? company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            return false;
        }

        Resource mappedResource = _mapper.Map<Resource>(serviceDto);
        if (serviceDto.Id == null)
        {
            mappedResource = await _resourceRepository.CreateService(mappedResource);
        }

        return await _resourceRepository.AddCompanyService(company, mappedResource);
    }

    public async Task<bool> UpdateCompanyService(int companyId, ResourceDto resourceDto, int serviceUpdateId)
    {
        Resource mappedResource = _mapper.Map<Resource>(resourceDto);
        if (mappedResource == null)
        {
            return false;
        }

        Company? company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            return false;
        }

        return await _resourceRepository.UpdateCompanyService(company, mappedResource, serviceUpdateId);
    }

    public async Task<bool> DeleteCompanyService(int serviceId, int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            return false;
        }

        return await _resourceRepository.DeleteCompanyService(serviceId, company);
    }

    public async Task<ResourceDto?> GetService(int serviceId)
    {
        Resource? service = await _resourceRepository.ReadByServiceId(serviceId);
        ResourceDto mappedResource = _mapper.Map<ResourceDto>(service);
        return mappedResource;
    }

    public async Task<IEnumerable<ResourceDto?>> GetAllServices()
    {
        IEnumerable<Resource?> services = await _resourceRepository.ReadAllServices();
        List<ResourceDto> mappedServices = services.Select(i => _mapper.Map<ResourceDto>(i)).ToList();
        return mappedServices;
    }

    public async Task<IEnumerable<ResourceDto?>> GetCompanyServices(int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            return null;
        }

        IEnumerable<Resource?> servicesCompany = await _resourceRepository.ReadCompanyServices(company);
        IEnumerable<ResourceDto> mappedServicesCompany = servicesCompany.Select(i => _mapper.Map<ResourceDto>(i));
        return mappedServicesCompany;
    }
}
using Application.DTOs.Mappings;
using Infrastructure.Repositories;
using Domain.Entities;
using AutoMapper;
using Domain.Enums;

namespace Application.Services;

public class ResourceService : IResourceService
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IMapper _mapper;
    private readonly ICompanyRepository _companyRepository;

    public ResourceService(IResourceRepository resourceRepository, IMapper mapper, ICompanyRepository companyRepository)
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
            await _resourceRepository.CreateResource(mappedResource);
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

        return await _resourceRepository.UpdateResource(mappedResource);
    }

    public async Task<bool> Delete(int resourceId)
    {
        return await _resourceRepository.DeleteResource(resourceId);
    }

    public async Task<bool> AddCompanyResource(int companyId, ResourceDto? resourceDto)
    {
        Company? company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            return false;
        }

        Resource mappedResource = _mapper.Map<Resource>(resourceDto);
        if (mappedResource == null)
        {
            return false;
        }

        if (mappedResource.Id == 0)
        {
            mappedResource = await _resourceRepository.CreateResource(mappedResource);
        }

        return await _resourceRepository.AddCompanyResource(company, mappedResource);
    }

    public async Task<bool> UpdateCompanyResource(int companyId, ResourceDto resourceDto, int resourceUpdateId)
    {
        Resource? resourseToUpdate = await _resourceRepository.ReadByResourceId(resourceUpdateId);
        if (resourseToUpdate == null || await _companyRepository.ReadByCompanyId(companyId) == null ||
            resourseToUpdate.CompanyId != companyId)
        {
            return false;
        }

        resourseToUpdate.Name = resourceDto.Name;
        resourseToUpdate.Type = resourceDto.Type;
        resourseToUpdate.Status = (ServiceStatus)resourceDto.Status;
        resourseToUpdate.Source = resourceDto.Source;

        return await _resourceRepository.UpdateResource(resourseToUpdate);
    }

    public async Task<bool> DeleteCompanyResource(int resourceId, int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            return false;
        }

        Resource? resource = await _resourceRepository.ReadByResourceId(resourceId);
        if (resource == null || resource.CompanyId != companyId)
        {
            return false;
        }

        return await _resourceRepository.DeleteCompanyResource(resourceId, company);
    }

    public async Task<ResourceDto?> GetResource(int resourceId)
    {
        Resource? service = await _resourceRepository.ReadByResourceId(resourceId);
        ResourceDto mappedResource = _mapper.Map<ResourceDto>(service);
        return mappedResource;
    }

    public async Task<IEnumerable<ResourceDto?>> GetAllResources()
    {
        IEnumerable<Resource?> services = await _resourceRepository.ReadAllResources();
        List<ResourceDto> mappedServices = services.Select(i => _mapper.Map<ResourceDto>(i)).ToList();
        return mappedServices;
    }

    public async Task<IEnumerable<ResourceDto?>> GetCompanyResources(int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            return null;
        }

        IEnumerable<Resource?> servicesCompany = await _resourceRepository.ReadCompanyResources(company);
        IEnumerable<ResourceDto> mappedServicesCompany = servicesCompany.Select(i => _mapper.Map<ResourceDto>(i));
        return mappedServicesCompany;
    }
}
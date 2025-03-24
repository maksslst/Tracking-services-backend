using Application.DTOs.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.CompanyRepository;
using Infrastructure.Repositories.ResourceRepository;

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

    public async Task<int?> Add(ResourceDto resourceDto)
    {
        var mappedResource = _mapper.Map<Resource>(resourceDto);
        if (mappedResource != null)
        {
            int resourceId = await _resourceRepository.CreateResource(mappedResource);
            return resourceId;
        }

        return null;
    }

    public async Task<bool> Update(ResourceDto resourceDto)
    {
        var mappedResource = _mapper.Map<Resource>(resourceDto);
        if (mappedResource == null)
        {
            return false;
        }

        var company = await _companyRepository.ReadByCompanyId(resourceDto.CompanyId);
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
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            return false;
        }

        var mappedResource = _mapper.Map<Resource>(resourceDto);
        if (mappedResource == null)
        {
            return false;
        }

        if (mappedResource.Id == 0)
        {
            int resourceId = await _resourceRepository.CreateResource(mappedResource);
            mappedResource = await _resourceRepository.ReadByResourceId(resourceId);
        }

        return await _resourceRepository.AddCompanyResource(company, mappedResource);
    }

    public async Task<bool> UpdateCompanyResource(int companyId, ResourceDto resourceDto, int resourceUpdateId)
    {
        var resourceToUpdate = await _resourceRepository.ReadByResourceId(resourceUpdateId);
        if (resourceToUpdate == null || await _companyRepository.ReadByCompanyId(companyId) == null ||
            resourceToUpdate.CompanyId != companyId)
        {
            return false;
        }

        resourceToUpdate.Name = resourceDto.Name;
        resourceToUpdate.Type = resourceDto.Type;
        resourceToUpdate.Status = (ResourceStatus)resourceDto.Status;
        resourceToUpdate.Source = resourceDto.Source;

        return await _resourceRepository.UpdateResource(resourceToUpdate);
    }

    public async Task<bool> DeleteCompanyResource(int resourceId, int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            return false;
        }

        var resource = await _resourceRepository.ReadByResourceId(resourceId);
        if (resource == null || resource.CompanyId != companyId)
        {
            return false;
        }

        return await _resourceRepository.DeleteCompanyResource(resourceId, company);
    }

    public async Task<ResourceDto?> GetResource(int resourceId)
    {
        var resource = await _resourceRepository.ReadByResourceId(resourceId);
        var mappedResource = _mapper.Map<ResourceDto>(resource);
        return mappedResource;
    }

    public async Task<IEnumerable<ResourceDto?>> GetAllResources()
    {
        var resource = await _resourceRepository.ReadAllResources();
        var mappedResource = resource.Select(i => _mapper.Map<ResourceDto>(i)).ToList();
        return mappedResource;
    }

    public async Task<IEnumerable<ResourceDto?>> GetCompanyResources(int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            return null;
        }

        var resourcesCompany = await _resourceRepository.ReadCompanyResources(company);
        var mappedResourcesCompany = resourcesCompany.Select(i => _mapper.Map<ResourceDto>(i));
        return mappedResourcesCompany;
    }
}
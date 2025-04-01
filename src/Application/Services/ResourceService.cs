using Application.DTOs.Mappings;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.CompanyRepository;
using Infrastructure.Repositories.ResourceRepository;
using Application.Requests;
using Application.Responses;
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

    public async Task<Resource?> Add(CreateResourceRequest request)
    {
        if (request.CompanyId != null && await _companyRepository.ReadByCompanyId(request.CompanyId) == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var resource = new Resource()
        {
            Name = request.Name,
            Status = request.Status,
            CompanyId = request.CompanyId,
            Type = request.Type,
            Source = request.Source
        };

        await _resourceRepository.CreateResource(resource);
        return resource;
    }

    public async Task<bool> Update(UpdateResourceRequest request)
    {
        if (await _resourceRepository.ReadByResourceId(request.ResourceId) == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        if (request.CompanyId != null && await _companyRepository.ReadByCompanyId(request.CompanyId) == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var resource = new Resource()
        {
            Id = request.ResourceId,
            CompanyId = request.CompanyId,
            Name = request.Name,
            Status = request.Status,
            Type = request.Type,
            Source = request.Source
        };

        return await _resourceRepository.UpdateResource(resource);
    }

    public async Task<bool> Delete(int resourceId)
    {
        if (await _resourceRepository.ReadByResourceId(resourceId) == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        return await _resourceRepository.DeleteResource(resourceId);
    }

    public async Task<bool> AddCompanyResource(int companyId, CreateResourceRequest? request)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var resource = new Resource()
        {
            Name = request.Name,
            Status = request.Status,
            CompanyId = request.CompanyId,
            Type = request.Type,
            Source = request.Source
        };

        if (await _resourceRepository.ReadByResourceId(resource.Id) == null)
        {
            int resourceId = await _resourceRepository.CreateResource(resource);
            resource = await _resourceRepository.ReadByResourceId(resourceId);
        }

        return await _resourceRepository.AddCompanyResource(company, resource);
    }

    public async Task<bool> UpdateCompanyResource(int companyId, UpdateResourceRequest request, int resourceUpdateId)
    {
        var resourceToUpdate = await _resourceRepository.ReadByResourceId(resourceUpdateId);
        if (resourceToUpdate == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        if (await _companyRepository.ReadByCompanyId(companyId) == null || resourceToUpdate.CompanyId != companyId)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        resourceToUpdate.Name = request.Name;
        resourceToUpdate.Type = request.Type;
        resourceToUpdate.Status = request.Status;
        resourceToUpdate.Source = request.Source;

        return await _resourceRepository.UpdateResource(resourceToUpdate);
    }

    public async Task<bool> DeleteCompanyResource(int resourceId, int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var resource = await _resourceRepository.ReadByResourceId(resourceId);
        if (resource == null || resource.CompanyId != companyId)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        return await _resourceRepository.DeleteCompanyResource(resourceId, company);
    }

    public async Task<ResourceResponse?> GetResource(int resourceId)
    {
        var resource = await _resourceRepository.ReadByResourceId(resourceId);
        if (resource == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        return _mapper.Map<ResourceResponse>(resource);
    }

    public async Task<IEnumerable<ResourceResponse?>> GetAllResources()
    {
        var resources = await _resourceRepository.ReadAllResources();
        if (resources.Count() == 0 || resources == null)
        {
            throw new NotFoundApplicationException("resources not found");
        }

        var resourcesResponse = resources.Select(i => _mapper.Map<ResourceResponse>(i));
        return resourcesResponse;
    }

    public async Task<IEnumerable<ResourceResponse?>> GetCompanyResources(int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var resourcesCompany = await _resourceRepository.ReadCompanyResources(company);
        if (resourcesCompany.Count() == 0 || resourcesCompany == null)
        {
            throw new NotFoundApplicationException("Resources not found");
        }

        var resourcesResponse = resourcesCompany.Select(i => _mapper.Map<ResourceResponse>(i));
        return resourcesResponse;
    }
}
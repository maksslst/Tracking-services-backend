using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.CompanyRepository;
using Infrastructure.Repositories.ResourceRepository;
using Application.Requests;
using Application.Responses;
using Npgsql;

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

    public async Task<int> Add(CreateResourceRequest request)
    {
        var resource = _mapper.Map<Resource>(request);
        return await _resourceRepository.CreateResource(resource);
    }

    public async Task Update(UpdateResourceRequest request)
    {
        var resource = await _resourceRepository.ReadByResourceId(request.Id);
        if (resource == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        resource = _mapper.Map<Resource>(request);
        bool isUpdated = await _resourceRepository.UpdateResource(resource);
        if (!isUpdated)
        {
            throw new EntityUpdateException("Failed to update the resource");
        }
    }

    public async Task Delete(int resourceId)
    {
        bool isDeleted = await _resourceRepository.DeleteResource(resourceId);
        if (!isDeleted)
        {
            throw new EntityDeleteException("Couldn't delete the resource");
        }
    }

    public async Task AddCompanyResource(int companyId, CreateResourceRequest request)
    {
        var resource = _mapper.Map<Resource>(request);
        resource.CompanyId = companyId;

        int resourceId = await _resourceRepository.CreateResource(resource);
        resource.Id = resourceId;

        bool isAdded = await _resourceRepository.AddCompanyResource(resource);
        if (!isAdded)
        {
            throw new EntityCreateException("Couldn't add company resource");
        }
    }

    public async Task UpdateCompanyResource(int companyId, UpdateResourceRequest request, int resourceUpdateId)
    {
        var resourceToUpdate = await _resourceRepository.ReadByResourceId(resourceUpdateId);
        if (resourceToUpdate == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        resourceToUpdate = _mapper.Map<Resource>(request);
        bool isUpdated = await _resourceRepository.UpdateResource(resourceToUpdate);
        if (!isUpdated)
        {
            throw new EntityUpdateException("Failed to update the resource");
        }
    }

    public async Task DeleteCompanyResource(int resourceId, int companyId)
    {
        bool isDeleted = await _resourceRepository.DeleteCompanyResource(resourceId, companyId);
        if (!isDeleted)
        {
            throw new EntityDeleteException("Couldn't delete a resource from the company");
        }
    }

    public async Task<ResourceResponse> GetResource(int resourceId)
    {
        var resource = await _resourceRepository.ReadByResourceId(resourceId);
        if (resource == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        return _mapper.Map<ResourceResponse>(resource);
    }

    public async Task<IEnumerable<ResourceResponse>> GetAllResources()
    {
        var resources = await _resourceRepository.ReadAllResources();
        var resourcesResponse = resources.Select(i => _mapper.Map<ResourceResponse>(i));
        return resourcesResponse;
    }

    public async Task<IEnumerable<ResourceResponse>> GetCompanyResources(int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var resourcesCompany = await _resourceRepository.ReadCompanyResources(companyId);
        var resourcesResponse = resourcesCompany.Select(i => _mapper.Map<ResourceResponse>(i));
        return resourcesResponse;
    }
}
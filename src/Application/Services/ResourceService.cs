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

    public async Task<bool> Update(UpdateResourceRequest request)
    {
        var resource = await _resourceRepository.ReadByResourceId(request.Id);
        if (resource == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        resource = _mapper.Map<Resource>(request);
        return await _resourceRepository.UpdateResource(resource);
    }

    public async Task<bool> Delete(int resourceId)
    {
        bool isDeleted = await _resourceRepository.DeleteResource(resourceId);
        if (!isDeleted)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        return true;
    }

    public async Task<bool> AddCompanyResource(int companyId, CreateResourceRequest request)
    {
        var resource = _mapper.Map<Resource>(request);
        int resourceId = await _resourceRepository.CreateResource(resource);
        resource.Id = resourceId;
        return await _resourceRepository.AddCompanyResource(resource);
    }

    public async Task<bool> UpdateCompanyResource(int companyId, UpdateResourceRequest request, int resourceUpdateId)
    {
        var resourceToUpdate = await _resourceRepository.ReadByResourceId(resourceUpdateId);
        if (resourceToUpdate == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        resourceToUpdate = _mapper.Map<Resource>(request);
        return await _resourceRepository.UpdateResource(resourceToUpdate);
    }

    public async Task<bool> DeleteCompanyResource(int resourceId, int companyId)
    {
        var resource = await _resourceRepository.ReadByResourceId(resourceId);
        if (resource == null || resource.CompanyId != companyId)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        var company = await _companyRepository.ReadByCompanyId(companyId);
        return await _resourceRepository.DeleteCompanyResource(resourceId, company);
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
        if (resources.Count() == 0 || resources == null)
        {
            throw new NotFoundApplicationException("resources not found");
        }

        var resourcesResponse = resources.Select(i => _mapper.Map<ResourceResponse>(i));
        return resourcesResponse;
    }

    public async Task<IEnumerable<ResourceResponse>> GetCompanyResources(int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        var resourcesCompany = await _resourceRepository.ReadCompanyResources(company);
        if (resourcesCompany.Count() == 0 || resourcesCompany == null)
        {
            throw new NotFoundApplicationException("Resources not found");
        }

        var resourcesResponse = resourcesCompany.Select(i => _mapper.Map<ResourceResponse>(i));
        return resourcesResponse;
    }
}
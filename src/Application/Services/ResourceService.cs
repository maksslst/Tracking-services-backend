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
        try
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

            return await _resourceRepository.CreateResource(resource);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't add resource");
        }
    }

    public async Task<bool> Update(UpdateResourceRequest request)
    {
        try
        {
            var resource = await _resourceRepository.ReadByResourceId(request.ResourceId);
            if (resource == null)
            {
                throw new NotFoundApplicationException("Resource not found");
            }

            if (request.CompanyId != null && await _companyRepository.ReadByCompanyId(request.CompanyId) == null)
            {
                throw new NotFoundApplicationException("Company not found");
            }

            resource.Name = request.Name;
            resource.Status = request.Status;
            resource.Type = request.Type;
            resource.Source = request.Source;
            resource.CompanyId = request.CompanyId;

            return await _resourceRepository.UpdateResource(resource);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't update resource");
        }
    }

    public async Task<bool> Delete(int resourceId)
    {
        try
        {
            bool isDeleted = await _resourceRepository.DeleteResource(resourceId);
            if (!isDeleted)
            {
                throw new NotFoundApplicationException("Resource not found");
            }

            return true;
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't delete resource");
        }
    }

    public async Task<bool> AddCompanyResource(int companyId, CreateResourceRequest request)
    {
        try
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
                CompanyId = companyId,
                Type = request.Type,
                Source = request.Source
            };

            int resourceId = await _resourceRepository.CreateResource(resource);
            resource.Id = resourceId;
            return await _resourceRepository.AddCompanyResource(resource);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't add company resource");
        }
    }

    public async Task<bool> UpdateCompanyResource(int companyId, UpdateResourceRequest request, int resourceUpdateId)
    {
        try
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
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't update company resource");
        }
    }

    public async Task<bool> DeleteCompanyResource(int resourceId, int companyId)
    {
        try
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
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't delete company resource");
        }
    }

    public async Task<ResourceResponse> GetResource(int resourceId)
    {
        try
        {
            var resource = await _resourceRepository.ReadByResourceId(resourceId);
            if (resource == null)
            {
                throw new NotFoundApplicationException("Resource not found");
            }

            return _mapper.Map<ResourceResponse>(resource);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't get resource");
        }
    }

    public async Task<IEnumerable<ResourceResponse>> GetAllResources()
    {
        try
        {
            var resources = await _resourceRepository.ReadAllResources();
            if (resources.Count() == 0 || resources == null)
            {
                throw new NotFoundApplicationException("resources not found");
            }

            var resourcesResponse = resources.Select(i => _mapper.Map<ResourceResponse>(i));
            return resourcesResponse;
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't get resources");
        }
    }

    public async Task<IEnumerable<ResourceResponse>> GetCompanyResources(int companyId)
    {
        try
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
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't get company resources");
        }
    }
}
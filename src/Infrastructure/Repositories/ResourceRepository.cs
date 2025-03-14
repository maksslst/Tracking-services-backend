using Domain.Entities;
using Bogus;
using Domain.Enums;

namespace Infrastructure.Repositories;

public class ResourceRepository : IResourceRepository
{
    private List<Resource> _resources;

    public ResourceRepository()
    {
        _resources = new List<Resource>();
        DataGeneration();
    }

    public Task<Resource> CreateResource(Resource resource)
    {
        _resources.Add(resource);
        return Task.FromResult(resource);
    }

    public Task<bool> UpdateResource(Resource resource)
    {
        var serviceToUpdate = _resources.Find(i => i.Id == resource.Id);
        if (serviceToUpdate == null)
        {
            return Task.FromResult(false);
        }

        serviceToUpdate.Name = resource.Name;
        serviceToUpdate.Type = resource.Type;
        serviceToUpdate.Source = resource.Source;
        serviceToUpdate.Status = resource.Status;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteResource(int resourceId)
    {
        var service = _resources.Find(i => i.Id == resourceId);
        if (service == null)
        {
            return Task.FromResult(false);
        }

        _resources.Remove(service);
        return Task.FromResult(true);
    }

    public Task<bool> AddCompanyResource(Company company, Resource? resource = null)
    {
        bool isCorrect = DataVerification(company, resource);
        if (isCorrect)
        {
            company.Resources.Add(resource);
        }

        return Task.FromResult(isCorrect);
    }
    
    // вместо этого метода в UpdateCompanyResource (ResourceService) вызываю UpdateResource
    // public Task<bool> UpdateCompanyResource(Company company, Resource resource, int resourceUpdateId)
    // {
    //     if (resource.CompanyId != company.Id)
    //     {
    //         return Task.FromResult(false);
    //     }
    //
    //     var resourceToUpdate = company.Resources.Find(i => i.Id == resourceUpdateId);
    //     if (resourceToUpdate == null)
    //     {
    //         return Task.FromResult(false);
    //     }
    //
    //     resourceToUpdate.Name = resource.Name;
    //     resourceToUpdate.Type = resource.Type;
    //     resourceToUpdate.Status = resource.Status;
    //     resourceToUpdate.Source = resource.Source;
    //     
    //     resourceToUpdate.CompanyId = company.Id;
    //     resourceToUpdate.Company = company;
    //
    //     return Task.FromResult(true);
    // }

    public Task<bool> DeleteCompanyResource(int resourceId, Company company)
    {
        var serviceToDelete = company.Resources.Find(i => i.Id == resourceId);
        if (serviceToDelete == null)
        {
            return Task.FromResult(false);
        }

        company.Resources.Remove(serviceToDelete);
        return Task.FromResult(true);
    }

    public Task<Resource?> ReadByResourceId(int resourceId)
    {
        var service = _resources.Find(i => i.Id == resourceId);
        return Task.FromResult(service);
    }

    public Task<IEnumerable<Resource?>> ReadAllResources()
    {
        return Task.FromResult<IEnumerable<Resource?>>(_resources);
    }

    public Task<IEnumerable<Resource?>> ReadCompanyResources(Company company)
    {
        var services = company.Resources;
        return Task.FromResult<IEnumerable<Resource?>>(services);
    }

    private void DataGeneration()
    {
        var faker = new Faker();
        for (int i = 0; i < 5; i++)
        {
            Resource resource = new Resource()
            {
                Id = i + 1,
                Name = faker.Internet.DomainName(),
                Type = "website",
                Source = faker.Internet.Ip(),
                Status = faker.PickRandom<ServiceStatus>()
            };

            _resources.Add(resource);
        }
    }

    private bool DataVerification(Company company, Resource? resource)
    {
        if (resource == null)
        {
            return false;
        }

        Resource? addedResource = _resources.Find(i => i.Id == resource.Id);
        if (addedResource == null)
        {
            return false;
        }

        if (company.Resources.Any(i => i.Id == addedResource.Id))
        {
            return false;
        }

        return true;
    }
}
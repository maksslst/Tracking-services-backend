using Bogus;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Repositories.ResourceRepository;

public class ResourceInMemoryRepository : IResourceRepository
{
    private List<Resource> _resources;

    public ResourceInMemoryRepository()
    {
        _resources = new List<Resource>();
        DataGeneration();
    }

    public Task<int> CreateResource(Resource resource)
    {
        _resources.Add(resource);
        return Task.FromResult(resource.Id);
    }

    public Task<bool> UpdateResource(Resource resource)
    {
        var resourceToUpdate = _resources.Find(i => i.Id == resource.Id);
        if (resourceToUpdate == null)
        {
            return Task.FromResult(false);
        }

        resourceToUpdate.Name = resource.Name;
        resourceToUpdate.Type = resource.Type;
        resourceToUpdate.Source = resource.Source;
        resourceToUpdate.Status = resource.Status;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteResource(int resourceId)
    {
        var resource = _resources.Find(i => i.Id == resourceId);
        if (resource == null)
        {
            return Task.FromResult(false);
        }

        _resources.Remove(resource);
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
    
    public Task<bool> DeleteCompanyResource(int resourceId, Company company)
    {
        var resourceToDelete = company.Resources.Find(i => i.Id == resourceId);
        if (resourceToDelete == null)
        {
            return Task.FromResult(false);
        }

        company.Resources.Remove(resourceToDelete);
        return Task.FromResult(true);
    }

    public Task<Resource?> ReadByResourceId(int resourceId)
    {
        var resource = _resources.Find(i => i.Id == resourceId);
        return Task.FromResult(resource);
    }

    public Task<IEnumerable<Resource?>> ReadAllResources()
    {
        return Task.FromResult<IEnumerable<Resource?>>(_resources);
    }

    public Task<IEnumerable<Resource?>> ReadCompanyResources(Company company)
    {
        var resource = company.Resources;
        return Task.FromResult<IEnumerable<Resource?>>(resource);
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
                Status = faker.PickRandom<ResourceStatus>()
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
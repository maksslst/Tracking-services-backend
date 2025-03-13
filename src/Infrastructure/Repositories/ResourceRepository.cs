using Domain.Entities;
using Bogus;
using Domain.Enums;

namespace Infrastructure.Repositories;

public class ResourceRepository : IResourceRepository
{
    private List<Resource> _services;

    public ResourceRepository()
    {
        _services = new List<Resource>();
        DataGeneration();
    }

    public Task<Resource> CreateService(Resource resource)
    {
        _services.Add(resource);
        return Task.FromResult(resource);
    }

    public Task<bool> UpdateService(Resource resource)
    {
        var serviceToUpdate = _services.Find(i => i.Id == resource.Id);
        if (serviceToUpdate == null)
        {
            return Task.FromResult(false);
        }

        serviceToUpdate.CompanyId = resource.CompanyId;
        serviceToUpdate.Company = resource.Company;
        serviceToUpdate.Name = resource.Name;
        serviceToUpdate.Type = resource.Type;
        serviceToUpdate.Source = resource.Source;
        serviceToUpdate.Status = resource.Status;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteService(int serviceId)
    {
        var service = _services.Find(i => i.Id == serviceId);
        if (service == null)
        {
            return Task.FromResult(false);
        }

        _services.Remove(service);
        return Task.FromResult(true);
    }

    public Task<bool> AddCompanyService(Company company, Resource? service = null)
    {
        bool isCorrect = DataVerification(company, service);
        return Task.FromResult(isCorrect);
    }

    public Task<bool> UpdateCompanyService(Company company, Resource resource, int serviceUpdateId)
    {
        if (resource.CompanyId != company.Id)
        {
            return Task.FromResult(false);
        }

        var serviceToUpdate = company.Services.Find(i => i.Id == serviceUpdateId);
        if (serviceToUpdate == null)
        {
            return Task.FromResult(false);
        }

        serviceToUpdate.Name = resource.Name;
        serviceToUpdate.Type = resource.Type;
        serviceToUpdate.Status = resource.Status;
        serviceToUpdate.Source = resource.Source;
        serviceToUpdate.CompanyId = company.Id;
        serviceToUpdate.Company = company;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteCompanyService(int serviceId, Company company)
    {
        var serviceToDelete = company.Services.Find(i => i.Id == serviceId);
        if (serviceToDelete == null)
        {
            return Task.FromResult(false);
        }

        company.Services.Remove(serviceToDelete);
        return Task.FromResult(true);
    }

    public Task<Resource?> ReadByServiceId(int serviceId)
    {
        var service = _services.Find(i => i.Id == serviceId);
        return Task.FromResult(service);
    }

    public Task<IEnumerable<Resource?>> ReadAllServices()
    {
        return Task.FromResult<IEnumerable<Resource?>>(_services);
    }

    public Task<IEnumerable<Resource?>> ReadCompanyServices(Company company)
    {
        var services = company.Services;
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

            _services.Add(resource);
        }
    }

    private bool DataVerification(Company company, Resource? service)
    {
        if (service == null)
        {
            return false;
        }
        
        Resource? addedService = _services.Find(i => i.Id == service.Id);
        if (addedService == null)
        {
            return false;
        }

        if (!company.Services.Any(i => i.Id == addedService.Id))
        {
            AddingService(company,addedService);
        }

        return true;
    }

    private void AddingService(Company company, Resource addedResource)
    {
        company.Services.Add(addedResource);
    }
}
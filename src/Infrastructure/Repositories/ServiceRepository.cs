using Domain.Entities;

namespace Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private List<Service> _services;
    private readonly ICompanyRepository _companyRepository;

    public ServiceRepository(ICompanyRepository companyRepository)
    {
        _services = new List<Service>();
        _companyRepository = companyRepository;
    }

    public Task AddService(Service service)
    {
        _services.Add(service);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateService(Service service)
    {
        var serviceToUpdate = _services.Find(i => i.Id == service.Id);
        if (serviceToUpdate == null)
        {
            return Task.FromResult(false);
        }

        var company = _companyRepository.ReadByCompanyId(service.CompanyId).Result;
        if (company == null)
        {
            return Task.FromResult(false);
        }

        serviceToUpdate.CompanyId = service.CompanyId;
        serviceToUpdate.Company = service.Company;
        serviceToUpdate.Name = service.Name;
        serviceToUpdate.Type = service.Type;
        serviceToUpdate.Source = service.Source;
        serviceToUpdate.Status = service.Status;

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

    public Task<bool> AddCompanyService(int companyId, int serviceId = -1, Service? service = null)
    {
        Service? addedService = null;
        Company? company = null;
        if (serviceId != -1)
        {
            addedService = _services.Find(i => i.Id == serviceId);
            if (addedService == null)
            {
                return Task.FromResult(false);
            }

            company = _companyRepository.ReadByCompanyId(companyId).Result;
            if (company == null)
            {
                return Task.FromResult(false);
            }

            if (!company.Services.Any(i => i.Id == addedService.Id))
            {
                company.Services.Add(addedService);
            }

            return Task.FromResult(true);
        }

        if (service != null)
        {
            addedService = _services.Find(i => i.Id == service.Id);
            if (addedService == null)
            {
                return Task.FromResult(false);
            }

            company = _companyRepository.ReadByCompanyId(companyId).Result;
            if (company == null)
            {
                return Task.FromResult(false);
            }

            if (!company.Services.Any(i => i.Id == addedService.Id))
            {
                company.Services.Add(addedService);
            }

            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<bool> UpdateCompanyService(int companyId, Service service, int serviceUpdateId)
    {
        Company company = _companyRepository.ReadByCompanyId(companyId).Result;
        if (company == null)
        {
            return Task.FromResult(false);
        }

        if (service.CompanyId != company.Id)
        {
            return Task.FromResult(false);
        }
        
        var serviceToUpdate = company.Services.Find(i => i.Id == serviceUpdateId);
        if (serviceToUpdate == null)
        {
            return Task.FromResult(false);
        }
        
        serviceToUpdate.Name = service.Name;
        serviceToUpdate.Type = service.Type;
        serviceToUpdate.Status = service.Status;
        serviceToUpdate.Source = service.Source;
        serviceToUpdate.CompanyId = companyId;
        serviceToUpdate.Company = company;
        
        return Task.FromResult(true);
    }

    public Task<bool> DeleteCompanyService(int serviceId, int companyId)
    {
        var company = _companyRepository.ReadByCompanyId(companyId).Result;
        if (company == null)
        {
            return Task.FromResult(false);
        }
        
        var serviceToDelete = company.Services.Find(i => i.Id == serviceId);
        if (serviceToDelete == null)
        {
            return Task.FromResult(false);
        }
        
        company.Services.Remove(serviceToDelete);
        return Task.FromResult(true);
    }

    public Task<Service?> ReadByServiceId(int serviceId)
    {
        var service = _services.Find(i => i.Id == serviceId);
        return Task.FromResult(service);
    }

    public Task<List<Service?>> ReadAllServices()
    {
        return Task.FromResult(_services);
    }

    public Task<List<Service?>> ReadCompanyServices(int companyId)
    {
        var company = _companyRepository.ReadByCompanyId(companyId).Result;
        if (company == null)
        {
            return Task.FromResult(new List<Service>());
        }

        var services = company.Services;
        return Task.FromResult(services);
    }
}
using System.Diagnostics.CodeAnalysis;
using Bogus;
using Domain.Entities;

namespace Infrastructure.Repositories.CompanyRepository;

[ExcludeFromCodeCoverage]
public class CompanyInMemoryRepository : ICompanyRepository
{
    private List<Company> _companies;

    public CompanyInMemoryRepository()
    {
        _companies = new List<Company>();
        DataGeneration();
    }

    public Task<int> CreateCompany(Company company)
    {
        _companies.Add(company);
        return Task.FromResult(company.Id);
    }

    public Task<bool> UpdateCompany(Company company)
    {
        var companyToUpdate = _companies.Find(i => i.Id == company.Id);
        if (companyToUpdate == null)
        {
            return Task.FromResult(false);
        }

        companyToUpdate.CompanyName = company.CompanyName;
        companyToUpdate.Users = company.Users;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteCompany(int companyId)
    {
        var company = _companies.Find(i => i.Id == companyId);
        if (company == null)
        {
            return Task.FromResult(false);
        }

        _companies.Remove(company);
        return Task.FromResult(true);
    }

    public Task<bool> AddUserToCompany(int userId, int companyId)
    {
        var company = _companies.Find(i => i.Id == companyId);
        if (company == null)
        {
            return Task.FromResult(false);
        }

        // if (!company.Users.Any(i => i.Id == userId))
        // {
        //     user.CompanyId = companyId;
        //     user.Company = company;
        //     company.Users.Add(user);
        // }

        return Task.FromResult(true);
    }

    public Task<bool> RemoveUserFromCompany(int userId, int companyId)
    {
        var company = _companies.Find(i => i.Id == companyId);
        if (company == null)
        {
            return Task.FromResult(false);
        }

        if (!company.Users.Any(i => i.Id ==userId))
        {
            return Task.FromResult(false);
        }

        // company.Users.Remove(user);
        return Task.FromResult(true);
    }

    public Task<Company?> ReadByCompanyId(int? companyId)
    {
        var company = _companies.Find(i => i.Id == companyId);
        return Task.FromResult(company);
    }

    public Task<IEnumerable<Company?>> ReadAllCompanies()
    {
        return Task.FromResult<IEnumerable<Company?>>(_companies);
    }

    public Task<IEnumerable<User?>> ReadCompanyUsers(int companyId)
    {
        var company = _companies.Find(i => i.Id == companyId);
        if (company == null)
        {
            throw new ArgumentException();
        }

        return Task.FromResult<IEnumerable<User?>>(company.Users);
    }

    private void DataGeneration()
    {
        var faker = new Faker();
        for (int i = 0; i < 3; i++)
        {
            Company company = new Company()
            {
                Id = i + 1,
                CompanyName = faker.Company.CompanyName()
            };

            _companies.Add(company);
        }
    }
}
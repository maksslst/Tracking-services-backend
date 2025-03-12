using Domain.Entities;
using Bogus;

namespace Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private List<Company> _companies;
    private readonly IUserRepository _userRepository;

    public CompanyRepository(IUserRepository userRepository)
    {
        _companies = new List<Company>();
        _userRepository = userRepository;
        DataGeneration();
    }

    public Task CreateCompany(Company company)
    {
        _companies.Add(company);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateCompany(Company company)
    {
        var companyToUpdate = _companies.Find(i => i.Id == company.Id);
        if (companyToUpdate == null)
        {
            return Task.FromResult(false);
        }

        var users = company.Users.ToList();
        foreach (var user in users)
        {
            if (_userRepository.ReadById(user.Id).Result == null)
            {
                return Task.FromResult(false);
            }
        }

        companyToUpdate.CompanyName = company.CompanyName;
        companyToUpdate.Users = users;
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

        var user = _userRepository.ReadById(userId).Result;
        if (user == null)
        {
            return Task.FromResult(false);
        }

        user.CompanyId = companyId;
        user.Company = company;

        if (!company.Users.Any(i => i.Id == userId))
        {
            company.Users.Add(user);
        }

        return Task.FromResult(true);
    }

    public Task<bool> RemoveUserFromCompany(int userId, int companyId)
    {
        var company = _companies.Find(i => i.Id == companyId);
        if (company == null)
        {
            return Task.FromResult(false);
        }

        var user = _userRepository.ReadById(userId).Result;
        if (user == null)
        {
            return Task.FromResult(false);
        }

        if (!company.Users.Any(i => i.Id == userId))
        {
            return Task.FromResult(false);
        }

        company.Users.Remove(user);
        return Task.FromResult(true);
    }

    public Task<Company?> ReadByCompanyId(int? companyId)
    {
        var company = _companies.Find(i => i.Id == companyId);
        return Task.FromResult(company);
    }

    public Task<List<Company?>> ReadAllCompanies()
    {
        return Task.FromResult(_companies);
    }

    public Task<List<User?>> ReadCompanyUsers(int companyId)
    {
        var company = _companies.Find(i => i.Id == companyId);
        if (company == null)
        {
            throw new ArgumentException();
        }

        return Task.FromResult(company.Users);
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
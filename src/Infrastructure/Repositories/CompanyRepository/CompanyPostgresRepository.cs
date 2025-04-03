using Dapper;
using Domain.Entities;
using Npgsql;

namespace Infrastructure.Repositories.CompanyRepository;

public class CompanyPostgresRepository : ICompanyRepository
{
    private readonly NpgsqlConnection _connection;

    public CompanyPostgresRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> CreateCompany(Company company)
    {
        var companyId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO companies (company_name)
                VALUES (@companyName)
                RETURNING id", new { company.CompanyName });

        return companyId;
    }

    public async Task<bool> UpdateCompany(Company company)
    {
        var companyToUpdate = await _connection.ExecuteAsync(
            @"UPDATE companies
                SET company_name = @companyName
                WHERE id = @Id", company);

        return companyToUpdate > 0;
    }

    public async Task<bool> DeleteCompany(int companyId)
    {
        var companyToDelete = await _connection.ExecuteAsync(
            @"DELETE FROM companies
                WHERE id = @Id", new { Id = companyId });

        return companyToDelete > 0;
    }

    public async Task<bool> AddUserToCompany(User user, int companyId)
    {
        var addedUser = await _connection.ExecuteAsync(
            @"UPDATE users
                SET company_id = @CompanyId
                WHERE id = @Id AND company_id != @CompanyId",
            new
            {
                CompanyId = companyId,
                Id = user.Id
            });

        return addedUser > 0;
    }

    public async Task<bool> RemoveUserFromCompany(User user, int companyId)
    {
        var deletedUser = await _connection.ExecuteAsync(
            @"UPDATE users
                SET company_id = null
                WHERE id = @Id and company_id = @CompanyId",
            new
            {
                Id = user.Id,
                CompanyId = companyId
            });

        return deletedUser > 0;
    }

    public async Task<Company?> ReadByCompanyId(int? companyId)
    {
        var company = await _connection.QueryFirstOrDefaultAsync<Company>(
            @"SELECT id, company_name
                FROM companies
                WHERE id = @Id", new { Id = companyId });

        return company;
    }

    public async Task<IEnumerable<Company?>> ReadAllCompanies()
    {
        var companies = await _connection.QueryAsync<Company>(
            @"SELECT id, company_name
                FROM companies");

        return companies;
    }

    public async Task<IEnumerable<User?>> ReadCompanyUsers(int companyId)
    {
       var users = await _connection.QueryAsync<User>(
            @"SELECT id, username, first_name, last_name, patronymic, email, company_id
                FROM users
                WHERE company_id = @CompanyId", new { CompanyId = companyId });

        return users;
    }
}
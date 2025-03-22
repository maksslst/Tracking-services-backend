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
        await _connection.OpenAsync();

        var companyId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO companies (company_name)
                VALUES (@companyName)
                RETURNING id", new { company.CompanyName });
        
        await _connection.CloseAsync();
        return companyId;
    }

    public async Task<bool> UpdateCompany(Company company)
    {
        await _connection.OpenAsync();

        var companyToUpdate = await _connection.ExecuteAsync(
            @"UPDATE companies
                SET company_name = @companyName
                WHERE id = @Id", company);
        
        await _connection.CloseAsync();
        return companyToUpdate > 0;
    }

    public async Task<bool> DeleteCompany(int companyId)
    {
        await _connection.OpenAsync();
        
        var companyToDelete = await _connection.ExecuteAsync(
            @"DELETE FROM companies
                WHERE id = @Id", new {Id = companyId});
        
        await _connection.CloseAsync();
        return companyToDelete > 0;
    }

    public async Task<bool> AddUserToCompany(User user, int companyId)
    {
        await _connection.OpenAsync();
        
        var company = await _connection.QueryFirstOrDefaultAsync<Company>(
            @"SELECT id 
                FROM companies 
                WHERE id = @Companyid", new { CompanyId = companyId });

        if (company == null)
        {
            return false;
        }

        var addedUser = await _connection.ExecuteAsync(
            @"UPDATE users
                SET company_id = @CompanyId
                WHERE id = @Id AND company_id != @CompanyId",
            new
            {
                CompanyId = companyId,
                Id = user.Id
            });
        
        await _connection.CloseAsync();
        return addedUser > 0;
    }

    public async Task<bool> RemoveUserFromCompany(User user, int companyId)
    {
        await _connection.OpenAsync();
        
        var company = await _connection.QueryFirstOrDefaultAsync<Company>(
            @"SELECT id 
                FROM companies 
                WHERE id = @company", new { company = companyId });

        if (company == null)
        {
            return false;
        }

        var deletedUser = await _connection.ExecuteAsync(
            @"UPDATE users
                SET company_id = null
                WHERE id = @Id and company_id = @CompanyId",
            new
            {
                Id = user.Id,
                CompanyId = companyId
            });
        
        await _connection.CloseAsync();
        return deletedUser > 0;
    }

    public async Task<Company?> ReadByCompanyId(int? companyId)
    {
        await _connection.OpenAsync();

        Company company = await _connection.QueryFirstOrDefaultAsync<Company>(
            @"SELECT id, company_name as CompanyName
                FROM companies
                WHERE id = @Id", new {Id = companyId});
        
        await _connection.CloseAsync();
        return company;
    }

    public async Task<IEnumerable<Company?>> ReadAllCompanies()
    {
        await _connection.OpenAsync();
        
        var companies = await _connection.QueryAsync<Company>(
            @"SELECT id, company_name as CompanyName
                FROM companies");
        
        await _connection.CloseAsync();
        return companies;
    }

    public async Task<IEnumerable<User?>> ReadCompanyUsers(int companyId)
    {
        await _connection.OpenAsync();
        
        var company = await _connection.QueryFirstOrDefaultAsync<Company>(
            @"SELECT id
                FROM companies 
                WHERE id = @Id", new { Id = companyId });

        if (company == null)
        {
            return null;
        }
        
        var users = await _connection.QueryAsync<User>(
            @"SELECT id, username, ""firstName"", ""lastName"", patronymic, email, company_id as CompanyId
                FROM users
                WHERE company_id = @CompanyId", new { CompanyId = companyId });
        
        await _connection.CloseAsync();
        return users;
    }
}
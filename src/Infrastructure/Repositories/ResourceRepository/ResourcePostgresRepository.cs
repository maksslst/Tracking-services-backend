using Domain.Entities;
using Dapper;
using Npgsql;

namespace Infrastructure.Repositories.ResourceRepository;

public class ResourcePostgresRepository : IResourceRepository
{
    private readonly NpgsqlConnection _connection;

    public ResourcePostgresRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }
    
    public async Task<int> CreateResource(Resource resource)
    {
        await _connection.OpenAsync();
        
        var resourceId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO resources(company_id, name, type, source, status)
                VALUES(@CompanyId, @Name, @Type, @Source, @Status)
                RETURNING id",
            new {resource.CompanyId, resource.Name, resource.Type, resource.Source, resource.Status});
        
        await _connection.CloseAsync();
        return resourceId;
    }

    public async Task<bool> UpdateResource(Resource resource)
    {
        await _connection.OpenAsync();

        var resourceToUpdate = await _connection.ExecuteAsync(
            @"UPDATE resources
                SET company_id = @CompanyId, 
                    name = @Name, 
                    type = @Type, 
                    source = @Source, 
                    status = @Status
                WHERE id=@Id", resource);
        
        await _connection.CloseAsync();
        return resourceToUpdate > 0;
    }

    public async Task<bool> DeleteResource(int resourceId)
    {
        await _connection.OpenAsync();
        
        var resourceToDelete = await _connection.ExecuteAsync(
            @"DELETE FROM resources
                WHERE id=@Id", new {Id = resourceId});
        
        await _connection.CloseAsync();
        return resourceToDelete > 0;
    }

    public async Task<bool> AddCompanyResource(Company company, Resource? resource = null)
    {
        bool isCorrect = await DataVerification(company, resource);
        if (isCorrect)
        {
            await _connection.OpenAsync();

            var addedResource = await _connection.ExecuteAsync(
                @"UPDATE resources
                    SET company_id = @CompanyId
                    WHERE id = @Id", 
                new
                {
                    Id = resource.Id,
                    CompanyId = company.Id
                });
            
            await _connection.CloseAsync();
            return addedResource > 0;
        }

        return isCorrect;
    }

    public async Task<bool> DeleteCompanyResource(int resourceId, Company company)
    {
        await _connection.OpenAsync();

        var deletedResource = await _connection.ExecuteAsync(
            @"UPDATE resources
                    SET company_id = null
                    WHERE id = @Id and company_id = @CompanyId", 
            new
            {
                Id = resourceId,
                CompanyId = company.Id
            });
            
        await _connection.CloseAsync();
        return deletedResource > 0;
    }

    public async Task<Resource?> ReadByResourceId(int resourceId)
    {
        await _connection.OpenAsync();
        
        Resource resource = await _connection.QueryFirstOrDefaultAsync<Resource>(
            @"SELECT id, company_id as CompanyId, name, type, source, status
                FROM resources
                WHERE id = @Id", new {Id = resourceId});
        
        await _connection.CloseAsync();
        return resource;
    }

    public async Task<IEnumerable<Resource?>> ReadAllResources()
    {
        await _connection.OpenAsync();

        var resources = await _connection.QueryAsync<Resource>(
            @"SELECT id, company_id as CompanyId, name, type, source, status
                FROM resources");
        
        await _connection.CloseAsync();
        return resources;
    }

    public async Task<IEnumerable<Resource?>> ReadCompanyResources(Company company)
    {
        await _connection.OpenAsync();
        
        var companyResources = await _connection.QueryAsync<Resource>(
            @"SELECT id, company_id as CompanyId, name, type, source, status
                FROM resources
                WHERE company_id = @CompanyId", new {CompanyId = company.Id});
        
        await _connection.CloseAsync();
        return companyResources;
    }
    
    private async Task<bool> DataVerification(Company company, Resource? resource)
    {
        if (resource == null)
        {
            return false;
        }

        var addedResource = await _connection.QueryFirstOrDefaultAsync<Resource>(
            @"SELECT id, company_id, name, type, source, status
                FROM resources
                WHERE id = @Id", new {Id = resource.Id});
        
        if (addedResource == null)
        {
            return false;
        }

        var companyResources = await ReadCompanyResources(company);
        if (companyResources.Any(i => i.Id == addedResource.Id))
        {
            return false;
        }

        return true;
    }
}
using Dapper;
using Domain.Entities;
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
        var resourceId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO resources(company_id, name, type, source, status)
                VALUES(@CompanyId, @Name, @Type, @Source, @Status)
                RETURNING id", resource);

        return resourceId;
    }

    public async Task<bool> UpdateResource(Resource resource)
    {
        var resourceToUpdate = await _connection.ExecuteAsync(
            @"UPDATE resources
                SET company_id = @CompanyId, 
                    name = @Name, 
                    type = @Type, 
                    source = @Source, 
                    status = @Status
                WHERE id=@Id", resource);

        return resourceToUpdate > 0;
    }

    public async Task<bool> DeleteResource(int resourceId)
    {
        var resourceToDelete = await _connection.ExecuteAsync(
            @"DELETE FROM resources
                WHERE id=@Id", new { Id = resourceId });

        return resourceToDelete > 0;
    }

    public async Task<bool> AddCompanyResource(Resource resource)
    {
        var addedResource = await _connection.ExecuteAsync(
            @"UPDATE resources
                    SET company_id = @CompanyId
                    WHERE id = @Id",
            new
            {
                Id = resource.Id,
                CompanyId = resource.CompanyId
            });

        return addedResource > 0;
    }

    public async Task<bool> DeleteCompanyResource(int resourceId, int companyId)
    {
        var deletedResource = await _connection.ExecuteAsync(
            @"UPDATE resources
                    SET company_id = null
                    WHERE id = @Id and company_id = @CompanyId",
            new
            {
                Id = resourceId,
                CompanyId = companyId
            });

        return deletedResource > 0;
    }

    public async Task<Resource?> ReadByResourceId(int resourceId)
    {
        var resource = await _connection.QueryFirstOrDefaultAsync<Resource>(
            @"SELECT id, company_id, name, type, source, status
                FROM resources
                WHERE id = @Id", new { Id = resourceId });

        return resource;
    }

    public async Task<IEnumerable<Resource?>> ReadAllResources()
    {
        var resources = await _connection.QueryAsync<Resource>(
            @"SELECT id, company_id, name, type, source, status
                FROM resources");

        return resources;
    }

    public async Task<IEnumerable<Resource?>> ReadCompanyResources(int companyId)
    {
        var companyResources = await _connection.QueryAsync<Resource>(
            @"SELECT id, company_id as CompanyId, name, type, source, status
                FROM resources
                WHERE company_id = @CompanyId", new { CompanyId = companyId });

        return companyResources;
    }
}
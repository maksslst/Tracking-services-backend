using Dapper;
using Domain.Entities;
using Npgsql;

namespace Infrastructure.Repositories.TaskRepository;

public class TaskPostgresRepository : ITaskRepository
{
    private readonly NpgsqlConnection _connection;

    public TaskPostgresRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }
    
    public async Task<int> CreateTask(ServiceTask serviceTask)
    {
        await _connection.OpenAsync();
        
        var taskId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO ""serviceTasks""(resource_id, description, assigned_user_id, created_by_id, start_time, completion_time, status)
                VALUES(@ResourceId, @Description, @AssignedUserId, @CreatedById, @StartTime, @CompletionTime, @Status)
                RETURNING id",
                serviceTask);
        
        await _connection.CloseAsync();
        return taskId;
    }

    public async Task<bool> UpdateTask(ServiceTask serviceTask, User assignedUserToUpdate)
    {
        await _connection.OpenAsync();
        
        var taskToUpdate = await _connection.ExecuteAsync(
            @"UPDATE ""serviceTasks"" 
                SET resource_id = @ResourceId,
                    description = @Description,
                    assigned_user_id = @AssignedUserId,
                    created_by_id = @CreatedById,
                    start_time = @StartTime,
                    completion_time = @CompletionTime,
                    status = @Status
                WHERE Id = @Id", serviceTask);
        
        await _connection.CloseAsync();
        return taskToUpdate > 0;
    }

    public async Task<bool> DeleteTask(int serviceTaskId)
    {
        await _connection.OpenAsync();
        
        var taskToDelete = await _connection.ExecuteAsync(
            @"DELETE FROM ""serviceTasks"" 
                WHERE id = @Id", serviceTaskId);
        
        await _connection.CloseAsync();
        return taskToDelete > 0;
    }

    public async Task<ServiceTask?> ReadTaskId(int taskId)
    {
        await _connection.OpenAsync();
        
        ServiceTask serviceTask = await _connection.QueryFirstOrDefaultAsync<ServiceTask>(
            @"SELECT id, resource_id as ResourceId, description, assigned_user_id as AssignedUserId, created_by_id as CreatedById , start_time, completion_time, status
                FROM ""serviceTasks""
                WHERE id=@Id", new {Id = taskId});
        
        await _connection.CloseAsync();
        return serviceTask;
    }

    public async Task<IEnumerable<ServiceTask?>> ReadAllTasksCompanyId(IEnumerable<Resource> companyResources)
    {
        var companyTasks = await _connection.QueryAsync<ServiceTask>(
            @"SELECT id, resource_id as ResourceId, description, assigned_user_id as AssignedUserId, created_by_id as CreatedById, start_time, completion_time, status
                FROM ""serviceTasks""
                WHERE resource_id = any(@resources)", new { resources = companyResources.Select(i => i.Id).ToList() });
        
        await _connection.CloseAsync();
        return companyTasks;
    }

    public async Task<bool> AssignTaskToUser(User user, int taskId)
    {
        await _connection.OpenAsync();

        var taskToUpdate = await _connection.ExecuteAsync(
            @"UPDATE ""serviceTasks""
                SET assigned_user_id = @AssignedUserId
                WHERE id = @Id", new { AssignedUserId = user.Id, Id = taskId });
        
        await _connection.CloseAsync();
        return taskToUpdate > 0;
    }

    public async Task<bool> DeleteTaskToUser(User user, int taskId)
    {
        await _connection.OpenAsync();
        
        var taskToDelete = await _connection.ExecuteAsync(
            @"UPDATE ""serviceTasks""
                SET assigned_user_id = null
                WHERE id = @Id", new { Id = taskId });
        
        await _connection.CloseAsync();
        return taskToDelete > 0;
    }

    public async Task<bool> ReassignTaskToUser(int oldUserId, User newUserId, int taskId)
    {
        await _connection.OpenAsync();
        
        var taskToUpdate = await _connection.ExecuteAsync(
            @"UPDATE ""serviceTasks""
                SET assigned_user_id = @AssignedUserId
                WHERE id = @Id AND assigned_user_id = @OldUserId", 
            new
            {
                Id = taskId,
                AssignedUserId = newUserId,
                OldUserId = oldUserId
                
            });
        
        await _connection.CloseAsync();
        return taskToUpdate > 0;
    }

    public async Task<ServiceTask?> ReadTaskUser(int userId, int taskId)
    {
        ServiceTask serviceTask = await _connection.QueryFirstOrDefaultAsync<ServiceTask>(
            @"SELECT id, resource_id as ResourceId, description, assigned_user_id as AssignedUserId, created_by_id as CreatedById, start_time, completion_time, status
                FROM ""serviceTasks""
                WHERE id = @Id and assigned_user_id = @AssignedUserId",
            new
            {
                Id = taskId,
                AssignedUserId = userId
            });
        
        await _connection.CloseAsync();
        return serviceTask;
    }

    public async Task<IEnumerable<ServiceTask?>> ReadAllUserTasks(int userId)
    {
        await _connection.OpenAsync();
        
        var tasks = await _connection.QueryAsync<ServiceTask>(
            @"SELECT id, resource_id, description, assigned_user_id, created_by_id, start_time, completion_time, status
                FROM serviceTasks
                WHERE assigned_user_id =@AssignedUserId", new {AssAssignedUserId = userId});
        
        await _connection.CloseAsync();
        return tasks;
    }
}
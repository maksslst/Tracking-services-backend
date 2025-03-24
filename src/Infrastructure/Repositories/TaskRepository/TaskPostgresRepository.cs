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
        var taskId = await _connection.QuerySingleAsync<int>(
            @"INSERT INTO ""service_tasks""(resource_id, description, assigned_user_id, created_by_id, start_time, completion_time, status)
                VALUES(@ResourceId, @Description, @AssignedUserId, @CreatedById, @StartTime, @CompletionTime, @Status)
                RETURNING id",
                serviceTask);

        return taskId;
    }

    public async Task<bool> UpdateTask(ServiceTask serviceTask, User assignedUserToUpdate)
    {
        var taskToUpdate = await _connection.ExecuteAsync(
            @"UPDATE ""service_tasks"" 
                SET resource_id = @ResourceId,
                    description = @Description,
                    assigned_user_id = @AssignedUserId,
                    created_by_id = @CreatedById,
                    start_time = @StartTime,
                    completion_time = @CompletionTime,
                    status = @Status
                WHERE Id = @Id", serviceTask);

        return taskToUpdate > 0;
    }

    public async Task<bool> DeleteTask(int serviceTaskId)
    {
        var taskToDelete = await _connection.ExecuteAsync(
            @"DELETE FROM ""service_tasks"" 
                WHERE id = @Id", serviceTaskId);

        return taskToDelete > 0;
    }

    public async Task<ServiceTask?> ReadTaskId(int taskId)
    {
        var serviceTask = await _connection.QueryFirstOrDefaultAsync<ServiceTask>(
            @"SELECT id, resource_id as ResourceId, description, assigned_user_id as AssignedUserId, created_by_id as CreatedById , start_time, completion_time, status
                FROM ""service_tasks""
                WHERE id=@Id", new { Id = taskId });

        return serviceTask;
    }

    public async Task<IEnumerable<ServiceTask?>> ReadAllTasksCompanyId(int companyId)
    {
        var companyTasks = await _connection.QueryAsync<ServiceTask>(
            @"SELECT s.id, s.resource_id as ResourceId, s.description, s.assigned_user_id as AssignedUserId, s.created_by_id as CreatedById, s.start_time, s.completion_time, s.status
                FROM ""service_tasks"" s join ""resources"" r on s.resource_id = r.id and r.company_id = @CompanyId", 
            new { CompanyId = companyId});

        return companyTasks;
    }

    public async Task<bool> AssignTaskToUser(User user, int taskId)
    {
        var taskToUpdate = await _connection.ExecuteAsync(
            @"UPDATE ""service_tasks""
                SET assigned_user_id = @AssignedUserId
                WHERE id = @Id", new { AssignedUserId = user.Id, Id = taskId });

        return taskToUpdate > 0;
    }

    public async Task<bool> DeleteTaskToUser(User user, int taskId)
    {
        var taskToDelete = await _connection.ExecuteAsync(
            @"UPDATE ""service_tasks""
                SET assigned_user_id = null
                WHERE id = @Id", new { Id = taskId });

        return taskToDelete > 0;
    }

    public async Task<bool> ReassignTaskToUser(int oldUserId, User newUserId, int taskId)
    {
        var taskToUpdate = await _connection.ExecuteAsync(
            @"UPDATE ""service_tasks""
                SET assigned_user_id = @AssignedUserId
                WHERE id = @Id AND assigned_user_id = @OldUserId",
            new
            {
                Id = taskId,
                AssignedUserId = newUserId,
                OldUserId = oldUserId

            });

        return taskToUpdate > 0;
    }

    public async Task<ServiceTask?> ReadTaskUser(int userId, int taskId)
    {
        var serviceTask = await _connection.QueryFirstOrDefaultAsync<ServiceTask>(
            @"SELECT id, resource_id as ResourceId, description, assigned_user_id as AssignedUserId, created_by_id as CreatedById, start_time, completion_time, status
                FROM ""service_tasks""
                WHERE id = @Id and assigned_user_id = @AssignedUserId",
            new
            {
                Id = taskId,
                AssignedUserId = userId
            });

        return serviceTask;
    }

    public async Task<IEnumerable<ServiceTask?>> ReadAllUserTasks(int userId)
    {
        var tasks = await _connection.QueryAsync<ServiceTask>(
            @"SELECT id, resource_id, description, assigned_user_id, created_by_id, start_time, completion_time, status
                FROM service_tasks
                WHERE assigned_user_id =@AssignedUserId", new { AssAssignedUserId = userId });

        return tasks;
    }
}
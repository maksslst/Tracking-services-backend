using Domain.Entities;

namespace Infrastructure.Repositories;

public interface ITaskRepository
{
    public Task CreateTask(ServiceTask serviceTask);
    public Task<bool> UpdateTask(ServiceTask serviceTask);
    public Task<bool> DeleteTask(int serviceTaskId);
    public Task<ServiceTask?> ReadTaskId(int taskId);
    public Task<List<ServiceTask?>> ReadAllTasksCompanyId (int companyId);
    public Task<bool> AssignTaskToUser(int userId, int taskId);
    public Task<bool> DeleteTaskToUser (int userId, int taskId);
    public Task<bool> ReassignTaskToUser (int oldUserId, int newUserId, int taskId);
    public Task<ServiceTask?> ReadTaskUser (int userId, int taskId);
    public Task<List<ServiceTask?>> ReadAllUserTasks (int userId);
}
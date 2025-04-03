using Domain.Entities;

namespace Infrastructure.Repositories.TaskRepository;

public interface ITaskRepository
{
    public Task<int> CreateTask(ServiceTask serviceTask);
    public Task<bool> UpdateTask(ServiceTask serviceTask, User assignedUserToUpdate);
    public Task<bool> DeleteTask(int serviceTaskId);
    public Task<ServiceTask?> ReadTaskId(int taskId);
    public Task<IEnumerable<ServiceTask?>> ReadAllTasksCompanyId(int companyId);
    public Task<bool> AssignTaskToUser(int userId, int taskId);
    public Task<bool> DeleteTaskToUser(int userId, int taskId);
    public Task<bool> ReassignTaskToUser(int oldUserId, int newUserId, int taskId);
    public Task<ServiceTask?> ReadTaskUser(int userId, int taskId);
    public Task<IEnumerable<ServiceTask?>> ReadAllUserTasks(int userId);
}
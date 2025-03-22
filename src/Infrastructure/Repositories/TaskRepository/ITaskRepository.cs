using Domain.Entities;

namespace Infrastructure.Repositories.TaskRepository;

public interface ITaskRepository
{
    public Task<int> CreateTask(ServiceTask serviceTask);
    public Task<bool> UpdateTask(ServiceTask serviceTask, User assignedUserToUpdate);
    public Task<bool> DeleteTask(int serviceTaskId);
    public Task<ServiceTask?> ReadTaskId(int taskId);
    public Task<IEnumerable<ServiceTask?>> ReadAllTasksCompanyId(IEnumerable<Resource> companyServices);
    public Task<bool> AssignTaskToUser(User user, int taskId);
    public Task<bool> DeleteTaskToUser(User user, int taskId);
    public Task<bool> ReassignTaskToUser(int oldUserId, User newUserId, int taskId);
    public Task<ServiceTask?> ReadTaskUser(int userId, int taskId);
    public Task<IEnumerable<ServiceTask?>> ReadAllUserTasks(int userId);
}
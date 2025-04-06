using Domain.Entities;

namespace Infrastructure.Repositories.TaskRepository;

public interface ITaskRepository
{
    public Task<int> CreateTask(ServiceTask serviceTask);
    public Task<bool> UpdateTask(ServiceTask serviceTask, User assignedUserToUpdate);
    public Task<bool> DeleteTask(int serviceTaskId);
    public Task<ServiceTask?> ReadTaskId(int taskId);
    public Task<IEnumerable<ServiceTask?>> ReadAllTasksCompanyId(int companyId);
    public Task<bool> SetTaskAssignment(int userId, int taskId, bool assign);
    public Task<IEnumerable<ServiceTask?>> ReadAllUserTasks(int userId);
}
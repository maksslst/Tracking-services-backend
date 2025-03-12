using Application.DTOs.Mappings;

namespace Application.Services;

public interface ITaskService
{
    public Task Add(ServiceTaskDto serviceTaskDto);
    public Task<bool> Update(ServiceTaskDto serviceTaskDto);
    public Task<bool> Delete(int serviceTaskId);
    public Task<ServiceTaskDto?> GetTaskId(int taskId);
    public Task<List<ServiceTaskDto?>> GetAllTasksCompanyId (int companyId);
    public Task<bool> AssignTaskToUser(int userId, int taskId);
    public Task<bool> DeleteTaskToUser (int userId, int taskId);
    public Task<bool> ReassignTaskToUser (int oldUserId, int newUserId, int taskId);
    public Task<ServiceTaskDto?> GetTaskUser (int userId, int taskId);
    public Task<List<ServiceTaskDto?>> GetAllUserTasks (int userId);
}
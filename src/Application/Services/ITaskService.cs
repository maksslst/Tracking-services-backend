using Application.DTOs.Mappings;
using Domain.Entities;

namespace Application.Services;

public interface ITaskService
{
    public Task<ServiceTask?> Add(ServiceTaskDto serviceTaskDto);
    public Task<bool> Update(ServiceTaskDto serviceTaskDto);
    public Task<bool> Delete(int serviceTaskId);
    public Task<ServiceTaskDto?> GetTask(int taskId);
    public Task<IEnumerable<ServiceTaskDto?>> GetAllCompanyTasks (int companyId);
    public Task<bool> AssignTaskToUser(int userId, int taskId);
    public Task<bool> DeleteTaskForUser (int userId, int taskId);
    public Task<bool> ReassignTaskToUser (int oldUserId, int newUserId, int taskId);
    public Task<ServiceTaskDto?> GetTaskForUser (int userId, int taskId);
    public Task<IEnumerable<ServiceTaskDto?>> GetAllUserTasks (int userId);
}
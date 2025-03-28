using Application.DTOs.Mappings;
using Domain.Entities;
using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface ITaskService
{
    public Task<ServiceTask?> Add(CreateTaskRequest request);
    public Task<bool> Update(UpdateTaskRequest request);
    public Task<bool> Delete(int serviceTaskId);
    public Task<TaskResponse?> GetTask(int taskId);
    public Task<IEnumerable<TaskResponse?>> GetAllCompanyTasks(int companyId);
    public Task<bool> AssignTaskToUser(int userId, int taskId);
    public Task<bool> DeleteTaskForUser(int userId, int taskId);
    public Task<bool> ReassignTaskToUser(int oldUserId, int newUserId, int taskId);
    public Task<TaskResponse?> GetTaskForUser(int userId, int taskId);
    public Task<IEnumerable<TaskResponse?>> GetAllUserTasks(int userId);
}
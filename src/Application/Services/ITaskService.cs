using Application.Requests;
using Application.Responses;

namespace Application.Services;

public interface ITaskService
{
    public Task<int> Add(CreateTaskRequest request);
    public Task Update(UpdateTaskRequest request);
    public Task Delete(int serviceTaskId);
    public Task<TaskResponse> GetTask(int taskId);
    public Task<IEnumerable<TaskResponse>> GetAllCompanyTasks(int companyId);
    public Task AssignTaskToUser(int userId, int taskId);
    public Task DeleteTaskForUser(int userId, int taskId);
    public Task ReassignTaskToUser(int newUserId, int taskId);
    public Task<TaskResponse> GetTaskForUser(int userId, int taskId);
    public Task<IEnumerable<TaskResponse>> GetAllUserTasks(int userId);
}
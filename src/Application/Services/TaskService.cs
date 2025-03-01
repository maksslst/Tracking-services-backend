using Application.DTOs.Mappings;

namespace Application.Services;

public class TaskService : ITaskService
{
    public Task Add(ServiceTaskDto serviceTaskDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(ServiceTaskDto serviceTaskDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(ServiceTaskDto serviceTaskDto)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceTaskDto?> GetTaskId(int taskId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ServiceTaskDto?>> GetAllTasksCompanyId(int companyId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AssignTaskToUser(int userId, int taskId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteTaskToUser(int userId, int taskId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ReassignTaskToUser(int oldUserId, int newUserId, int taskId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceTaskDto?> GetTaskUser(int userId, int taskId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ServiceTaskDto?>> GetAllUserTasks(int userId)
    {
        throw new NotImplementedException();
    }
}
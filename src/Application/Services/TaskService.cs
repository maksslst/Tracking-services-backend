using Application.DTOs.Mappings;
using Infrastructure.Repositories;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;

    public TaskService(ITaskRepository taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
    }

    public async Task Add(ServiceTaskDto serviceTaskDto)
    {
        ServiceTask mappedTask = _mapper.Map<ServiceTask>(serviceTaskDto);
        if (mappedTask != null)
        {
            await _taskRepository.CreateTask(mappedTask);
        }
    }

    public async Task<bool> Update(ServiceTaskDto serviceTaskDto)
    {
        ServiceTask mappedTask = _mapper.Map<ServiceTask>(serviceTaskDto);
        return await _taskRepository.UpdateTask(mappedTask);
    }

    public async Task<bool> Delete(int serviceTaskId)
    {
        return await _taskRepository.DeleteTask(serviceTaskId);
    }

    public async Task<ServiceTaskDto?> GetTaskId(int taskId)
    {
        ServiceTask? serviceTask = await _taskRepository.ReadTaskId(taskId);
        ServiceTaskDto mappedTask = _mapper.Map<ServiceTaskDto>(serviceTask);
        return mappedTask;
    }

    public async Task<List<ServiceTaskDto?>> GetAllTasksCompanyId(int companyId)
    {
        List<ServiceTask?> serviceTasks = await _taskRepository.ReadAllTasksCompanyId(companyId);
        List<ServiceTaskDto> mappedServiceTask = serviceTasks.Select(i => _mapper.Map<ServiceTaskDto>(i)).ToList();
        return mappedServiceTask;
    }

    public async Task<bool> AssignTaskToUser(int userId, int taskId)
    {
        return await _taskRepository.AssignTaskToUser(userId, taskId);
    }

    public async Task<bool> DeleteTaskToUser(int userId, int taskId)
    {
        return await _taskRepository.DeleteTaskToUser(userId, taskId);
    }

    public async Task<bool> ReassignTaskToUser(int oldUserId, int newUserId, int taskId)
    {
        return await _taskRepository.ReassignTaskToUser(oldUserId, newUserId, taskId);
    }

    public async Task<ServiceTaskDto?> GetTaskUser(int userId, int taskId)
    {
        ServiceTask? serviceTask = await _taskRepository.ReadTaskUser(userId, taskId);
        ServiceTaskDto mappedTask = _mapper.Map<ServiceTaskDto>(serviceTask);
        return mappedTask;
    }

    public async Task<List<ServiceTaskDto?>> GetAllUserTasks(int userId)
    {
        List<ServiceTask?> serviceTasksUser = await _taskRepository.ReadAllUserTasks(userId);
        List<ServiceTaskDto> mappedTasksUser = serviceTasksUser.Select(i => _mapper.Map<ServiceTaskDto>(i)).ToList();
        return mappedTasksUser;
    }
}
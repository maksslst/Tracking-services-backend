using Application.DTOs.Mappings;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.CompanyRepository;
using Infrastructure.Repositories.ResourceRepository;
using Infrastructure.Repositories.TaskRepository;
using Infrastructure.Repositories.UserRepository;

namespace Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;
    private readonly IResourceRepository _resourceRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;

    public TaskService(ITaskRepository taskRepository, IMapper mapper, IResourceRepository resourceRepository,
        IUserRepository userRepository, ICompanyRepository companyRepository)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _resourceRepository = resourceRepository;
        _userRepository = userRepository;
        _companyRepository = companyRepository;
    }

    public async Task<ServiceTask?> Add(ServiceTaskDto serviceTaskDto)
    {
        if (await _resourceRepository.ReadByResourceId(serviceTaskDto.ResourceId) == null)
        {
            return null;
        }

        var mappedTask = _mapper.Map<ServiceTask>(serviceTaskDto);
        if (mappedTask != null)
        {
            await _taskRepository.CreateTask(mappedTask);
            return mappedTask;
        }

        return null;
    }

    public async Task<bool> Update(ServiceTaskDto serviceTaskDto)
    {
        var mappedTask = _mapper.Map<ServiceTask>(serviceTaskDto);
        if (mappedTask == null)
        {
            return false;
        }

        if (await _resourceRepository.ReadByResourceId(serviceTaskDto.ResourceId) == null)
        {
            return false;
        }

        var assignedUserToUpdate = _userRepository.ReadById(serviceTaskDto.AssignedUserId).Result;
        if (assignedUserToUpdate == null || assignedUserToUpdate.CompanyId !=
            _userRepository.ReadById(serviceTaskDto.AssignedUserId).Result.CompanyId)
        {
            return false;
        }

        return await _taskRepository.UpdateTask(mappedTask, assignedUserToUpdate);
    }

    public async Task<bool> Delete(int serviceTaskId)
    {
        return await _taskRepository.DeleteTask(serviceTaskId);
    }

    public async Task<ServiceTaskDto?> GetTask(int taskId)
    {
        var serviceTask = await _taskRepository.ReadTaskId(taskId);
        var mappedTask = _mapper.Map<ServiceTaskDto>(serviceTask);
        return mappedTask;
    }

    public async Task<IEnumerable<ServiceTaskDto?>> GetAllCompanyTasks(int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            return null;
        }
        
        var serviceTasks = await _taskRepository.ReadAllTasksCompanyId(companyId);
        var mappedServiceTask = serviceTasks.Select(i => _mapper.Map<ServiceTaskDto>(i));
        return mappedServiceTask;
    }

    public async Task<bool> AssignTaskToUser(int userId, int taskId)
    {
        var user = await _userRepository.ReadById(userId);
        if (user == null)
        {
            return false;
        }

        return await _taskRepository.AssignTaskToUser(user, taskId);
    }

    public async Task<bool> DeleteTaskForUser(int userId, int taskId)
    {
        var user = await _userRepository.ReadById(userId);
        if (user == null)
        {
            return false;
        }
        return await _taskRepository.DeleteTaskToUser(user, taskId);
    }

    public async Task<bool> ReassignTaskToUser(int oldUserId, int newUserId, int taskId)
    {
        var newUser = _userRepository.ReadById(newUserId).Result;
        if (newUser == null)
        {
            return false;
        }

        return await _taskRepository.ReassignTaskToUser(oldUserId, newUser, taskId);
    }

    public async Task<ServiceTaskDto?> GetTaskForUser(int userId, int taskId)
    {
        var serviceTask = await _taskRepository.ReadTaskUser(userId, taskId);
        var mappedTask = _mapper.Map<ServiceTaskDto>(serviceTask);
        return mappedTask;
    }

    public async Task<IEnumerable<ServiceTaskDto?>> GetAllUserTasks(int userId)
    {
        var serviceTasksUser = await _taskRepository.ReadAllUserTasks(userId);
        var mappedTasksUser = serviceTasksUser.Select(i => _mapper.Map<ServiceTaskDto>(i));
        return mappedTasksUser;
    }
}
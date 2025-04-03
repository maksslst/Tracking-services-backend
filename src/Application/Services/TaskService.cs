using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.CompanyRepository;
using Infrastructure.Repositories.ResourceRepository;
using Infrastructure.Repositories.TaskRepository;
using Infrastructure.Repositories.UserRepository;
using Application.Requests;
using Application.Responses;
using Npgsql;

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

    public async Task<int> Add(CreateTaskRequest request)
    {
        var task = _mapper.Map<ServiceTask>(request);
        return await _taskRepository.CreateTask(task);
    }

    public async Task<bool> Update(UpdateTaskRequest request)
    {
        var taskToUpdate = await _taskRepository.ReadTaskId(request.Id);
        if (taskToUpdate == null)
        {
            throw new NotFoundApplicationException("Task not found");
        }

        var assignedUserToUpdate = await _userRepository.ReadById(request.AssignedUserId);
        if (taskToUpdate.AssignedUser.CompanyId != assignedUserToUpdate.CompanyId)
        {
            throw new UserAuthorizationException("User is not in the company");
        }

        taskToUpdate = _mapper.Map<ServiceTask>(request);
        return await _taskRepository.UpdateTask(taskToUpdate, assignedUserToUpdate);
    }

    public async Task<bool> Delete(int serviceTaskId)
    {
        bool isDeleted = await _taskRepository.DeleteTask(serviceTaskId);
        if (!isDeleted)
        {
            throw new NotFoundApplicationException("Task not found");
        }

        return true;
    }

    public async Task<TaskResponse> GetTask(int taskId)
    {
        var serviceTask = await _taskRepository.ReadTaskId(taskId);
        if (serviceTask == null)
        {
            throw new NotFoundApplicationException("Task not found");
        }

        return _mapper.Map<TaskResponse>(serviceTask);
    }

    public async Task<IEnumerable<TaskResponse>> GetAllCompanyTasks(int companyId)
    {
        var serviceTasks = await _taskRepository.ReadAllTasksCompanyId(companyId);
        if (serviceTasks.Count() == 0 || serviceTasks == null)
        {
            throw new NotFoundApplicationException("Tasks not found");
        }

        var tasksResponse = serviceTasks.Select(i => _mapper.Map<TaskResponse>(i));
        return tasksResponse;
    }

    public async Task<bool> AssignTaskToUser(int userId, int taskId)
    {
        if (await _taskRepository.ReadTaskId(taskId) == null)
        {
            throw new NotFoundApplicationException("Task not found");
        }

        return await _taskRepository.AssignTaskToUser(userId, taskId);
    }

    public async Task<bool> DeleteTaskForUser(int userId, int taskId)
    {
        if (await _taskRepository.ReadTaskId(taskId) == null)
        {
            throw new NotFoundApplicationException("Task not found");
        }

        return await _taskRepository.DeleteTaskToUser(userId, taskId);
    }

    public async Task<bool> ReassignTaskToUser(int oldUserId, int newUserId, int taskId)
    {
        if (await _taskRepository.ReadTaskId(taskId) == null)
        {
            throw new NotFoundApplicationException("Task not found");
        }

        return await _taskRepository.ReassignTaskToUser(oldUserId, newUserId, taskId);
    }

    public async Task<TaskResponse> GetTaskForUser(int userId, int taskId)
    {
        var serviceTask = await _taskRepository.ReadTaskUser(userId, taskId);
        if (serviceTask == null)
        {
            throw new NotFoundApplicationException("Task not found");
        }

        return _mapper.Map<TaskResponse>(serviceTask);
    }

    public async Task<IEnumerable<TaskResponse>> GetAllUserTasks(int userId)
    {
        var serviceTasksUser = await _taskRepository.ReadAllUserTasks(userId);
        if (serviceTasksUser.Count() == 0 || serviceTasksUser == null)
        {
            throw new NotFoundApplicationException("Tasks not found");
        }

        var tasksResponse = serviceTasksUser.Select(i => _mapper.Map<TaskResponse>(i));
        return tasksResponse;
    }
}
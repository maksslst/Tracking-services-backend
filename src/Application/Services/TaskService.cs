using Application.DTOs.Mappings;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.CompanyRepository;
using Infrastructure.Repositories.ResourceRepository;
using Infrastructure.Repositories.TaskRepository;
using Infrastructure.Repositories.UserRepository;
using Application.Requests;
using Application.Responses;
using TaskStatus = Domain.Enums.TaskStatus;

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

    public async Task<ServiceTask?> Add(CreateTaskRequest request)
    {
        if (await _resourceRepository.ReadByResourceId(request.ResourceId) == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        if (await _userRepository.ReadById(request.AssignedUserId) == null ||
            await _userRepository.ReadById(request.CreatedById) == null)
        {
            throw new NotFoundApplicationException("User not found");
        }

        var task = new ServiceTask()
        {
            ResourceId = request.ResourceId,
            Description = request.Description,
            AssignedUserId = request.AssignedUserId,
            CreatedById = request.CreatedById,
            Status = request.Status
        };

        await _taskRepository.CreateTask(task);
        return task;
    }

    public async Task<bool> Update(UpdateTaskRequest request)
    {
        var taskToUpdate = await _taskRepository.ReadTaskId(request.TaskId);
        if (taskToUpdate == null)
        {
            throw new NotFoundApplicationException("Task not found");
        }

        if (await _resourceRepository.ReadByResourceId(request.ResourceId) == null)
        {
            throw new NotFoundApplicationException("Resource not found");
        }

        var assignedUserToUpdate = await _userRepository.ReadById(request.AssignedUserId);
        var createdUserToUpdate = await _userRepository.ReadById(request.CreatedById);
        if (assignedUserToUpdate == null || createdUserToUpdate == null)
        {
            throw new NotFoundApplicationException("User not found");
        }

        if (taskToUpdate.AssignedUser.CompanyId != assignedUserToUpdate.CompanyId ||
            taskToUpdate.CreatedBy.CompanyId != createdUserToUpdate.CompanyId)
        {
            throw new UserArgumentException("User is not in the company");
        }

        var task = new ServiceTask()
        {
            Id = request.TaskId,
            ResourceId = request.ResourceId,
            Description = request.Description,
            AssignedUserId = request.AssignedUserId,
            CreatedById = request.CreatedById,
            CompletionTime = request.CompletionTime,
            Status = request.Status
        };

        return await _taskRepository.UpdateTask(task, assignedUserToUpdate);
    }

    public async Task<bool> Delete(int serviceTaskId)
    {
        if (await _taskRepository.ReadTaskId(serviceTaskId) == null)
        {
            throw new NotFoundApplicationException("Task not found");
        }

        return await _taskRepository.DeleteTask(serviceTaskId);
    }

    public async Task<TaskResponse?> GetTask(int taskId)
    {
        var serviceTask = await _taskRepository.ReadTaskId(taskId);
        if (serviceTask == null)
        {
            throw new NotFoundApplicationException("Task not found");
        }

        return _mapper.Map<TaskResponse>(serviceTask);
    }

    public async Task<IEnumerable<TaskResponse?>> GetAllCompanyTasks(int companyId)
    {
        var company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

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
        var user = await _userRepository.ReadById(userId);
        if (user == null)
        {
            throw new NotFoundApplicationException("User not found");
        }

        if (await _taskRepository.ReadTaskId(taskId) == null)
        {
            throw new NotFoundApplicationException("Task not found");
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

        if (await _taskRepository.ReadTaskId(taskId) == null)
        {
            throw new NotFoundApplicationException("Task not found");
        }

        return await _taskRepository.DeleteTaskToUser(user, taskId);
    }

    public async Task<bool> ReassignTaskToUser(int oldUserId, int newUserId, int taskId)
    {
        var newUser = _userRepository.ReadById(newUserId).Result;
        if (newUser == null || await _userRepository.ReadById(oldUserId) == null)
        {
            throw new NotFoundApplicationException("User not found");
        }

        if (await _taskRepository.ReadTaskId(taskId) == null)
        {
            throw new NotFoundApplicationException("Task not found");
        }

        return await _taskRepository.ReassignTaskToUser(oldUserId, newUser, taskId);
    }

    public async Task<TaskResponse?> GetTaskForUser(int userId, int taskId)
    {
        if (await _userRepository.ReadById(userId) == null)
        {
            throw new NotFoundApplicationException("User not found");
        }

        var serviceTask = await _taskRepository.ReadTaskUser(userId, taskId);
        if (serviceTask == null)
        {
            throw new NotFoundApplicationException("Task not found");
        }

        return _mapper.Map<TaskResponse>(serviceTask);
    }

    public async Task<IEnumerable<TaskResponse?>> GetAllUserTasks(int userId)
    {
        if (await _userRepository.ReadById(userId) == null)
        {
            throw new NotFoundApplicationException("User not found");
        }

        var serviceTasksUser = await _taskRepository.ReadAllUserTasks(userId);
        if (serviceTasksUser.Count() == 0 || serviceTasksUser == null)
        {
            throw new NotFoundApplicationException("Tasks not found");
        }

        var tasksResponse = serviceTasksUser.Select(i => _mapper.Map<TaskResponse>(i));
        return tasksResponse;
    }
}
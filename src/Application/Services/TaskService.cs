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

    public async Task Update(UpdateTaskRequest request)
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
        bool isUpdated = await _taskRepository.UpdateTask(taskToUpdate, assignedUserToUpdate);
        if (!isUpdated)
        {
            throw new EntityUpdateException("Failed to update task");
        }
    }

    public async Task Delete(int serviceTaskId)
    {
        bool isDeleted = await _taskRepository.DeleteTask(serviceTaskId);
        if (!isDeleted)
        {
            throw new EntityDeleteException("Couldn't delete task");
        }
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
        if (await _companyRepository.ReadByCompanyId(companyId) == null)
        {
            throw new NotFoundApplicationException("Company not found");
        }

        var serviceTasks = await _taskRepository.ReadAllTasksCompanyId(companyId);
        var tasksResponse = serviceTasks.Select(i => _mapper.Map<TaskResponse>(i));
        return tasksResponse;
    }

    public async Task AssignTaskToUser(int userId, int taskId)
    {
        bool isAssigned = await _taskRepository.SetTaskAssignment(userId, taskId, true);
        if (!isAssigned)
        {
            throw new EntityUpdateException("Failed to assign task to user");
        }
    }

    public async Task DeleteTaskForUser(int userId, int taskId)
    {
        bool isDeleted = await _taskRepository.SetTaskAssignment(userId, taskId, false);
        if (!isDeleted)
        {
            throw new EntityDeleteException("Couldn't delete the user's task");
        }
    }

    public async Task ReassignTaskToUser(int newUserId, int taskId)
    {
        bool isDeleted = await _taskRepository.SetTaskAssignment(newUserId, taskId, false);
        if (!isDeleted)
        {
            throw new EntityDeleteException("Couldn't delete the user's task");
        }
        
        bool isAssigned = await _taskRepository.SetTaskAssignment(newUserId, taskId, true);
        if (!isAssigned)
        {
            throw new EntityUpdateException("Failed to assign task to user");
        }
    }

    public async Task<TaskResponse> GetTaskForUser(int userId, int taskId)
    {
        var serviceTask = await _taskRepository.ReadTaskId(taskId);
        if (serviceTask == null || serviceTask.AssignedUserId != userId)
        {
            throw new UserAuthorizationException("This user does not own this task");
        }

        return _mapper.Map<TaskResponse>(serviceTask);
    }

    public async Task<IEnumerable<TaskResponse>> GetAllUserTasks(int userId)
    {
        var serviceTasksUser = await _taskRepository.ReadAllUserTasks(userId);
        var tasksResponse = serviceTasksUser.Select(i => _mapper.Map<TaskResponse>(i));
        return tasksResponse;
    }
}
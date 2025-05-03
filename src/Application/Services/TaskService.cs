using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.CompanyRepository;
using Infrastructure.Repositories.TaskRepository;
using Infrastructure.Repositories.UserRepository;
using Application.Requests;
using Application.Responses;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(ITaskRepository taskRepository, IMapper mapper, IUserRepository userRepository,
        ICompanyRepository companyRepository, ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _logger = logger;
    }

    public async Task<int> Add(CreateTaskRequest request)
    {
        var task = _mapper.Map<ServiceTask>(request);
        var taskId = await _taskRepository.CreateTask(task);
        _logger.LogInformation("Added a task with id: {taskId}", taskId);
        return taskId;
    }

    public async Task Update(UpdateTaskRequest request)
    {
        var taskToUpdate = await _taskRepository.ReadTaskId(request.Id);
        if (taskToUpdate == null)
        {
            throw new NotFoundApplicationException("Task not found");
        }

        var assignedUserToUpdate = await _userRepository.ReadById(request.AssignedUserId);
        if (assignedUserToUpdate == null || taskToUpdate.AssignedUser?.CompanyId != assignedUserToUpdate.CompanyId)
        {
            throw new UnauthorizedApplicationException("User is not in the company");
        }

        taskToUpdate = _mapper.Map<ServiceTask>(request);
        bool isUpdated = await _taskRepository.UpdateTask(taskToUpdate, assignedUserToUpdate);
        if (!isUpdated)
        {
            throw new EntityUpdateException("Failed to update task");
        }

        _logger.LogInformation("Updated a task with id: {taskId}", taskToUpdate.Id);
    }

    public async Task Delete(int serviceTaskId)
    {
        bool isDeleted = await _taskRepository.DeleteTask(serviceTaskId);
        if (!isDeleted)
        {
            throw new EntityDeleteException("Couldn't delete task");
        }

        _logger.LogInformation("Deleted a task with id: {serviceTaskId}", serviceTaskId);
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

        _logger.LogInformation("Assigned a task with id: {taskId} to a user with id: {userId}", taskId, userId);
    }

    public async Task DeleteTaskForUser(int userId, int taskId)
    {
        bool isUnassigned = await _taskRepository.SetTaskAssignment(userId, taskId, false);
        if (!isUnassigned)
        {
            throw new EntityUpdateException("Failed to unassign task from user");
        }

        _logger.LogInformation("Unassigned a task with id: {taskId} the user with id: {userId}", taskId, userId);
    }

    public async Task ReassignTaskToUser(int newUserId, int taskId)
    {
        bool isReassigned = await _taskRepository.SetTaskAssignment(newUserId, taskId, true);
        if (!isReassigned)
        {
            throw new EntityUpdateException("Couldn't reassign task");
        }

        _logger.LogInformation("Reassigned a task with id: {taskId} the user with id: {userId}", taskId, newUserId);
    }

    public async Task<TaskResponse> GetTaskForUser(int userId, int taskId)
    {
        var serviceTask = await _taskRepository.ReadTaskId(taskId);
        if (serviceTask == null || serviceTask.AssignedUserId != userId)
        {
            throw new UnauthorizedApplicationException("This user does not own this task");
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
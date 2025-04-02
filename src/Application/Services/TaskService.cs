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
        try
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

            return await _taskRepository.CreateTask(task);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't add task");
        }
    }

    public async Task<bool> Update(UpdateTaskRequest request)
    {
        try
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
                throw new UserAuthorizationException("User is not in the company");
            }

            taskToUpdate.ResourceId = request.ResourceId;
            taskToUpdate.Description = request.Description;
            taskToUpdate.AssignedUserId = request.AssignedUserId;
            taskToUpdate.CreatedById = request.CreatedById;
            taskToUpdate.Status = request.Status;
            taskToUpdate.CompletionTime = DateTime.Now;
           
            return await _taskRepository.UpdateTask(taskToUpdate, assignedUserToUpdate);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't update task");
        }
    }

    public async Task<bool> Delete(int serviceTaskId)
    {
        try
        {
            bool isDeleted = await _taskRepository.DeleteTask(serviceTaskId);
            if (!isDeleted)
            {
                throw new NotFoundApplicationException("Task not found");
            }

            return true;
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't delete task");
        }
    }

    public async Task<TaskResponse> GetTask(int taskId)
    {
        try
        {
            var serviceTask = await _taskRepository.ReadTaskId(taskId);
            if (serviceTask == null)
            {
                throw new NotFoundApplicationException("Task not found");
            }

            return _mapper.Map<TaskResponse>(serviceTask);
        }
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't get task");
        }
    }

    public async Task<IEnumerable<TaskResponse>> GetAllCompanyTasks(int companyId)
    {
        try
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
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't get all the company's tasks");
        }
    }

    public async Task<bool> AssignTaskToUser(int userId, int taskId)
    {
        try
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
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't assign task to user");
        }
    }

    public async Task<bool> DeleteTaskForUser(int userId, int taskId)
    {
        try
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
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't delete task to user");
        }
    }

    public async Task<bool> ReassignTaskToUser(int oldUserId, int newUserId, int taskId)
    {
        try
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
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't reassign task to user");
        }
    }

    public async Task<TaskResponse> GetTaskForUser(int userId, int taskId)
    {
        try
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
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't get task for user");
        }
    }

    public async Task<IEnumerable<TaskResponse>> GetAllUserTasks(int userId)
    {
        try
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
        catch (NpgsqlException)
        {
            throw new DatabaseException("Couldn't get all tasks for user");
        }
    }
}
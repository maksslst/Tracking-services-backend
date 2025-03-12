using System.Data;
using Domain.Entities;
using Bogus;

namespace Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private List<ServiceTask> _tasks;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;

    public TaskRepository(IServiceRepository serviceRepository, IUserRepository userRepository, ICompanyRepository companyRepository)
    {
        _tasks = new List<ServiceTask>();
        _serviceRepository = serviceRepository;
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        DataGeneration();
    }

    public Task CreateTask(ServiceTask serviceTask)
    {
        _tasks.Add(serviceTask);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateTask(ServiceTask serviceTask)
    {
        var task = _tasks.Find(i => i.Id == serviceTask.Id);
        if (task == null)
        {
            return Task.FromResult(false);
        }

        if (_serviceRepository.ReadByServiceId(serviceTask.ServiceId).Result == null)
        {
            return Task.FromResult(false);
        }

        task.ServiceId = serviceTask.ServiceId;
        task.Service = serviceTask.Service;
        task.Description = serviceTask.Description;

        var assignedUserToUpdate = _userRepository.ReadById(serviceTask.AssignedUserId).Result;
        if (assignedUserToUpdate == null || assignedUserToUpdate.CompanyId !=
            _userRepository.ReadById(serviceTask.AssignedUserId).Result.CompanyId)
        {
            return Task.FromResult(false);
        }

        task.AssignedUserId = assignedUserToUpdate.Id;
        task.AssignedUser = assignedUserToUpdate;
        task.StartTime = serviceTask.StartTime;
        task.CompletionTime = serviceTask.CompletionTime;
        task.Status = serviceTask.Status;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteTask(int serviceTaskId)
    {
        var task = _tasks.Find(i => i.Id == serviceTaskId);
        if (task == null)
        {
            return Task.FromResult(false);
        }

        _tasks.Remove(task);
        return Task.FromResult(true);
    }

    public Task<ServiceTask?> ReadTaskId(int taskId)
    {
        var task = _tasks.Find(i => i.Id == taskId);
        if (task == null)
        {
            return Task.FromResult<ServiceTask?>(null);
        }
        
        return Task.FromResult(task);
    }

    public Task<List<ServiceTask?>> ReadAllTasksCompanyId(int companyId)
    {
        if (_companyRepository.ReadByCompanyId(companyId).Result == null)
        {
            return Task.FromResult(new List<ServiceTask?>());
        }

        var companyServices = _serviceRepository.ReadCompanyServices(companyId).Result;
        if (companyServices == null)
        {
            return Task.FromResult(new List<ServiceTask?>());
        }

        List<ServiceTask> companyTasks = new List<ServiceTask>();
        foreach (var task in _tasks)
        {
            if (companyServices.Any(i => i.Id == task.ServiceId))
            {
                companyTasks.Add(task);
            }
        }
        
        return Task.FromResult(companyTasks);
    }

    public Task<bool> AssignTaskToUser(int userId, int taskId)
    {
        var user = _userRepository.ReadById(userId).Result;
        if (user == null)
        {
            return Task.FromResult(false);
        }
        
        var task = _tasks.Find(i => i.Id == taskId);
        if (task == null)
        {
            return Task.FromResult(false);
        }

        if (task.AssignedUserId != null)
        {
            return Task.FromResult(false);
        }
        
        task.AssignedUserId = userId;
        task.AssignedUser = user;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteTaskToUser(int userId, int taskId)
    {
        var user =_userRepository.ReadById(userId).Result;
        if (user == null)
        {
            return Task.FromResult(false);
        }
        
        var task = _tasks.Find(i => i.Id == taskId);
        if (task == null)
        {
            return Task.FromResult(false);
        }

        if (task.AssignedUserId != userId)
        {
            return Task.FromResult(false);
        }

        task.AssignedUserId = null;
        task.AssignedUser = null;
        return Task.FromResult(true);
    }

    public Task<bool> ReassignTaskToUser(int oldUserId, int newUserId, int taskId)
    {
        var user = _userRepository.ReadById(newUserId).Result;
        if (user == null)
        {
            return Task.FromResult(false);
        }
        
        var task = _tasks.Find(i => i.Id == taskId && i.AssignedUserId == oldUserId);
        if (task == null)
        {
            return Task.FromResult(false);
        }

        if (task.AssignedUserId == null)
        {
            return Task.FromResult(false);
        }

        task.AssignedUserId = newUserId;
        task.AssignedUser = user;
        return Task.FromResult(true);
    }

    public Task<ServiceTask?> ReadTaskUser(int userId, int taskId)
    {
        var task = _tasks.Find(i => i.Id == taskId && i.AssignedUserId == userId);
        if (task == null)
        {
            return Task.FromResult((ServiceTask?)null);
        }

        return Task.FromResult(task);
    }

    public Task<List<ServiceTask?>> ReadAllUserTasks(int userId)
    {
        var task = _tasks.FindAll(i => i.AssignedUserId == userId);
        return Task.FromResult(task);
    }

    private void DataGeneration()
    {
        var faker = new Faker();
        Random random = new Random();
        for (int i = 0; i < 5; i++)
        {
            ServiceTask serviceTask = new ServiceTask()
            {
                Id = i + 1,
                ServiceId = random.Next(1, 5),
                Description = faker.Random.Word(),
                CreatedById = random.Next(1, 5),
                StartTime = DateTime.Today,
                CompletionTime = DateTime.Now,
                Status = faker.PickRandom<ServiceTask.TaskStatus>()
            };
            
            _tasks.Add(serviceTask);
        }
    }
}
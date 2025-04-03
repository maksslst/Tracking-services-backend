using Bogus;
using Domain.Entities;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Infrastructure.Repositories.TaskRepository;

public class TaskInMemoryRepository : ITaskRepository
{
    private List<ServiceTask> _tasks;

    public TaskInMemoryRepository()
    {
        _tasks = new List<ServiceTask>();
        DataGeneration();
    }

    public Task<int> CreateTask(ServiceTask serviceTask)
    {
        _tasks.Add(serviceTask);
        return Task.FromResult(serviceTask.Id);
    }

    public Task<bool> UpdateTask(ServiceTask serviceTask,User assignedUserToUpdate)
    {
        var task = _tasks.Find(i => i.Id == serviceTask.Id);
        if (task == null)
        {
            return Task.FromResult(false);
        }

        task.ResourceId = serviceTask.ResourceId;
        task.Resource = serviceTask.Resource;
        task.Description = serviceTask.Description;
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

    public Task<IEnumerable<ServiceTask?>> ReadAllTasksCompanyId(int companyId)
    {
        List<ServiceTask> companyTasks = new List<ServiceTask>();
        foreach (var task in _tasks)
        {
            if (task.Resource.CompanyId == companyId)
            {
                companyTasks.Add(task);
            }
        }
        
        return Task.FromResult<IEnumerable<ServiceTask?>>(companyTasks);
    }

    public Task<bool> AssignTaskToUser(int user, int taskId)
    {
        
        var task = _tasks.Find(i => i.Id == taskId);
        if (task == null)
        {
            return Task.FromResult(false);
        }

        if (task.AssignedUserId != null)
        {
            return Task.FromResult(false);
        }
        
        task.AssignedUserId = user;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteTaskToUser(int userId, int taskId)
    {
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
        return Task.FromResult(true);
    }

    public Task<bool> ReassignTaskToUser(int oldUserId, int newUserId, int taskId)
    {
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

    public Task<IEnumerable<ServiceTask?>> ReadAllUserTasks(int userId)
    {
        var task = _tasks.FindAll(i => i.AssignedUserId == userId);
        return Task.FromResult<IEnumerable<ServiceTask?>>(task);
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
                ResourceId = random.Next(1, 5),
                Description = faker.Random.Word(),
                CreatedById = random.Next(1, 5),
                StartTime = DateTime.Today,
                CompletionTime = DateTime.Now,
                Status = faker.PickRandom<TaskStatus>()
            };
            
            _tasks.Add(serviceTask);
        }
    }
}
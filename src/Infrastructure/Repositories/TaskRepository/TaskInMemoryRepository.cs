using System.Diagnostics.CodeAnalysis;
using Bogus;
using Domain.Entities;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Infrastructure.Repositories.TaskRepository;

[ExcludeFromCodeCoverage]
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

    public Task<bool> SetTaskAssignment(int userId, int taskId, bool assign)
    {
        return Task.FromResult(true);
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
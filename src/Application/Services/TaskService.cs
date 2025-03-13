using Application.DTOs.Mappings;
using Infrastructure.Repositories;
using AutoMapper;
using Domain.Entities;

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
        ServiceTask mappedTask = _mapper.Map<ServiceTask>(serviceTaskDto);
        if (mappedTask != null)
        {
            await _taskRepository.CreateTask(mappedTask);
            return mappedTask;
        }

        return null;
    }

    public async Task<bool> Update(ServiceTaskDto serviceTaskDto)
    {
        ServiceTask mappedTask = _mapper.Map<ServiceTask>(serviceTaskDto);
        if (mappedTask == null)
        {
            return false;
        }

        if (await _resourceRepository.ReadByServiceId(serviceTaskDto.ServiceId) == null)
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
        ServiceTask? serviceTask = await _taskRepository.ReadTaskId(taskId);
        ServiceTaskDto mappedTask = _mapper.Map<ServiceTaskDto>(serviceTask);
        return mappedTask;
    }

    public async Task<IEnumerable<ServiceTaskDto?>> GetAllTasksCompany(int companyId)
    {
        Company? company = await _companyRepository.ReadByCompanyId(companyId);
        if (company == null)
        {
            return null;
        }

        var companyServices = _resourceRepository.ReadCompanyServices(company).Result;
        if (companyServices == null)
        {
            return null;
        }

        IEnumerable<ServiceTask?> serviceTasks = await _taskRepository.ReadAllTasksCompanyId(companyServices);
        IEnumerable<ServiceTaskDto> mappedServiceTask = serviceTasks.Select(i => _mapper.Map<ServiceTaskDto>(i));
        return mappedServiceTask;
    }

    public async Task<bool> AssignTaskToUser(int userId, int taskId)
    {
        User? user = await _userRepository.ReadById(userId);
        if (user == null)
        {
            return false;
        }
        
        return await _taskRepository.AssignTaskToUser(user, taskId);
    }

    public async Task<bool> DeleteTaskForUser(int userId, int taskId)
    {
        User? user = await _userRepository.ReadById(userId);
        if (user == null)
        {
            return false;
        }
        return await _taskRepository.DeleteTaskToUser(user, taskId);
    }

    public async Task<bool> ReassignTaskToUser(int oldUserId, int newUserId, int taskId)
    {
        User? newUser = _userRepository.ReadById(newUserId).Result;
        if (newUser == null)
        {
            return false;
        }
        
        return await _taskRepository.ReassignTaskToUser(oldUserId, newUser, taskId);
    }

    public async Task<ServiceTaskDto?> GetTaskForUser(int userId, int taskId)
    {
        ServiceTask? serviceTask = await _taskRepository.ReadTaskUser(userId, taskId);
        ServiceTaskDto mappedTask = _mapper.Map<ServiceTaskDto>(serviceTask);
        return mappedTask;
    }

    public async Task<IEnumerable<ServiceTaskDto?>> GetAllUserTasks(int userId)
    {
        IEnumerable<ServiceTask?> serviceTasksUser = await _taskRepository.ReadAllUserTasks(userId);
        IEnumerable<ServiceTaskDto> mappedTasksUser = serviceTasksUser.Select(i => _mapper.Map<ServiceTaskDto>(i));
        return mappedTasksUser;
    }
}
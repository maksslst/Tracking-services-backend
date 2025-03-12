using Application.DTOs.Mappings;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskController : ControllerBase
{
    private ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    #region HttpPost
    [HttpPost]
    public async Task<IActionResult> Add([FromBody]ServiceTaskDto serviceTaskDto)
    {
        await _taskService.Add(serviceTaskDto);
        return Created();
    }
    
    [HttpPost("{userId}/{taskId}")]
    public async Task<IActionResult> AssignTaskToUser([FromQuery]int userId, int taskId)
    {
        var result = await _taskService.AssignTaskToUser(userId, taskId);
        if (!result)
        {
            return BadRequest("Не удалось назначить задачу пользователю");
        }
        return Ok();
    }
    #endregion

    #region HttpPut
    [HttpPut]
    public async Task<IActionResult> Update([FromBody]ServiceTaskDto serviceTaskDto)
    {
        var result = await _taskService.Update(serviceTaskDto);
        if (!result)
        {
            return BadRequest("Не удалось обновить задачу");
        }
        
        return Ok();
    }
    
    [HttpPut("{oldUserId}/{newUserId}/{taskId}")]
    public async Task<IActionResult> ReassignTaskToUser([FromQuery] int oldUserId, int newUserId, int taskId)
    {
        var result = await _taskService.ReassignTaskToUser(oldUserId, newUserId, taskId);
        if (!result)
        {
            return BadRequest($"Не удалось переназначить задачу пользователю с Id: {newUserId}");
        }
        return Ok();
    }
    #endregion

    #region HttpDelete
    [HttpDelete("{taskId}")]
    public async Task<IActionResult> Delete([FromQuery]int taskId)
    {
        var result = await _taskService.Delete(taskId);
        if (!result)
        {
            return BadRequest("Не удалось удалить задачу");
        }
        return Ok();
    }
    
    [HttpDelete("{userId}/{taskId}")]
    public async Task<IActionResult> DeleteTaskToUser([FromQuery]int userId, int taskId)
    {
        var result = await _taskService.DeleteTaskToUser(userId, taskId);
        if (!result)
        {
            return BadRequest("Не удалось удалить задачу у пользователя");
        }
        return Ok();
    }
    #endregion

    #region HttpGet
    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTaskId([FromQuery]int taskId)
    {
        ServiceTaskDto? serviceTask = await _taskService.GetTaskId(taskId);
        if (serviceTask == null)
        {
            return BadRequest("Не удалось найти задачу");
        }
        return Ok(serviceTask);
    }

    [HttpGet("GetAllTasksCompanyId/{companyId}")]
    public async Task<IActionResult> GetAllTasksCompanyId([FromQuery]int companyId)
    {
        List<ServiceTaskDto?> serviceTasks = await _taskService.GetAllTasksCompanyId(companyId);
        if (serviceTasks.Count == 0)
        {
            return BadRequest("Не удалось получить задачи компании");
        }
        return Ok(serviceTasks);
    }

    [HttpGet("{userId}/{taskId}")]
    public async Task<IActionResult> GetTaskUser([FromQuery]int userId, int taskId)
    {
        ServiceTaskDto? serviceTask = await _taskService.GetTaskUser(userId, taskId);
        if (serviceTask == null)
        {
            return BadRequest("Не удалось найти задачу");
        }
        return Ok(serviceTask);
    }

    [HttpGet("GetAllUserTasks/{userId}")]
    public async Task<IActionResult> GetAllUserTasks([FromQuery]int userId)
    {
        List<ServiceTaskDto?> serviceTasks = await _taskService.GetAllUserTasks(userId);
        if (serviceTasks.Count == 0)
        {
            return BadRequest("Не удалось найти список задач пользователя");
        }
        
        return Ok(serviceTasks);
    }
    #endregion
}
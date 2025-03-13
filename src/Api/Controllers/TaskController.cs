using Application.DTOs.Mappings;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    #region HttpPost
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] ServiceTaskDto serviceTaskDto)
    {
        ServiceTask? serviceTask = await _taskService.Add(serviceTaskDto);
        if (serviceTask == null)
        {
            return BadRequest("Не удалось создать задачу");
        }

        return Created(serviceTask.Id.ToString(), serviceTask);
    }

    [HttpPost("{userId}/{taskId}")]
    public async Task<IActionResult> AssignTaskToUser(int userId, int taskId)
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
    public async Task<IActionResult> Update([FromBody] ServiceTaskDto serviceTaskDto)
    {
        var result = await _taskService.Update(serviceTaskDto);
        if (!result)
        {
            return BadRequest("Не удалось обновить задачу");
        }

        return Ok();
    }

    [HttpPut("{oldUserId}/{newUserId}/{taskId}")]
    public async Task<IActionResult> ReassignTaskToUser(int oldUserId, int newUserId, int taskId)
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
    public async Task<IActionResult> Delete(int taskId)
    {
        if (await _taskService.GetTask(taskId) == null)
        {
            return NotFound("Задача не найдена");
        }
        
        var result = await _taskService.Delete(taskId);
        if (!result)
        {
            return BadRequest("Не удалось удалить задачу");
        }

        return NoContent();
    }

    [HttpDelete("{userId}/{taskId}")]
    public async Task<IActionResult> DeleteTaskToUser(int userId, int taskId)
    {
        var result = await _taskService.DeleteTaskForUser(userId, taskId);
        if (!result)
        {
            return BadRequest("Не удалось удалить задачу у пользователя");
        }

        return NoContent();
    }
    #endregion

    #region HttpGet
    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTaskId(int taskId)
    {
        ServiceTaskDto? serviceTask = await _taskService.GetTask(taskId);
        if (serviceTask == null)
        {
            return BadRequest("Не удалось найти задачу");
        }

        return Ok(serviceTask);
    }

    [HttpGet("GetAllTasksCompanyId/{companyId}")]
    public async Task<IActionResult> GetAllTasksCompanyId(int companyId)
    {
        IEnumerable<ServiceTaskDto?> serviceTasks = await _taskService.GetAllTasksCompany(companyId);
        if (serviceTasks == null)
        {
            return BadRequest("Не удалось получить задачи компании");
        }

        return Ok(serviceTasks);
    }

    [HttpGet("{userId}/{taskId}")]
    public async Task<IActionResult> GetTaskUser(int userId, int taskId)
    {
        ServiceTaskDto? serviceTask = await _taskService.GetTaskForUser(userId, taskId);
        if (serviceTask == null)
        {
            return BadRequest("Не удалось найти задачу");
        }

        return Ok(serviceTask);
    }

    [HttpGet("GetAllUserTasks/{userId}")]
    public async Task<IActionResult> GetAllUserTasks(int userId)
    {
        IEnumerable<ServiceTaskDto?> serviceTasks = await _taskService.GetAllUserTasks(userId);
        if (serviceTasks == null)
        {
            return BadRequest("Не удалось найти список задач пользователя");
        }

        return Ok(serviceTasks);
    }
    #endregion
}
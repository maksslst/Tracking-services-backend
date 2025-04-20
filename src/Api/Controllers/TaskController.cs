using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Requests;

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
    public async Task<IActionResult> Add([FromBody] CreateTaskRequest request)
    {
        int serviceTask = await _taskService.Add(request);
        return CreatedAtAction(nameof(GetTaskId), new { taskId = serviceTask }, serviceTask);
    }

    [HttpPost("AssignTaskToUser/{userId}/{taskId}")]
    public async Task<IActionResult> AssignTaskToUser(int userId, int taskId)
    {
        await _taskService.AssignTaskToUser(userId, taskId);
        return NoContent();
    }

    #endregion

    #region HttpPut

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateTaskRequest request)
    {
        await _taskService.Update(request);
        return NoContent();
    }

    [HttpPut("ReassignTaskToUser/{newUserId}/{taskId}")]
    public async Task<IActionResult> ReassignTaskToUser(int newUserId, int taskId)
    {
        await _taskService.ReassignTaskToUser(newUserId, taskId);
        return NoContent();
    }

    #endregion

    #region HttpDelete

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> Delete(int taskId)
    {
        await _taskService.Delete(taskId);
        return NoContent();
    }

    [HttpDelete("DeleteTaskToUser/{userId}/{taskId}")]
    public async Task<IActionResult> DeleteTaskToUser(int userId, int taskId)
    {
        await _taskService.DeleteTaskForUser(userId, taskId);
        return NoContent();
    }

    #endregion

    #region HttpGet

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTaskId(int taskId)
    {
        var serviceTask = await _taskService.GetTask(taskId);
        return Ok(serviceTask);
    }

    [HttpGet("GetAllCompanyTasks/{companyId}")]
    public async Task<IActionResult> GetAllCompanyTasks(int companyId)
    {
        var serviceTasks = await _taskService.GetAllCompanyTasks(companyId);
        return Ok(serviceTasks);
    }

    [HttpGet("GetTaskUser/{userId}/{taskId}")]
    public async Task<IActionResult> GetTaskUser(int userId, int taskId)
    {
        var serviceTask = await _taskService.GetTaskForUser(userId, taskId);
        return Ok(serviceTask);
    }

    [HttpGet("GetAllUserTasks/{userId}")]
    public async Task<IActionResult> GetAllUserTasks(int userId)
    {
        var serviceTasks = await _taskService.GetAllUserTasks(userId);
        return Ok(serviceTasks);
    }

    #endregion
}
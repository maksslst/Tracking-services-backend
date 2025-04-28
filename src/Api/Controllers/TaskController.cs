using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.Requests;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TaskController(ITaskService taskService) : ControllerBase
{
    #region HttpPost

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateTaskRequest request)
    {
        int serviceTask = await taskService.Add(request);
        return CreatedAtAction(nameof(GetTaskId), new { taskId = serviceTask }, serviceTask);
    }

    [Authorize(Roles = "Admin, Moderator")]
    [HttpPost("AssignTaskToUser/{userId}/{taskId}")]
    public async Task<IActionResult> AssignTaskToUser(int userId, int taskId)
    {
        await taskService.AssignTaskToUser(userId, taskId);
        return NoContent();
    }

    #endregion

    #region HttpPut

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateTaskRequest request)
    {
        await taskService.Update(request);
        return NoContent();
    }

    [Authorize(Roles = "Admin, Moderator")]
    [HttpPut("ReassignTaskToUser/{newUserId}/{taskId}")]
    public async Task<IActionResult> ReassignTaskToUser(int newUserId, int taskId)
    {
        await taskService.ReassignTaskToUser(newUserId, taskId);
        return NoContent();
    }

    #endregion

    #region HttpDelete

    [Authorize(Roles = "Admin, Moderator")]
    [HttpDelete("{taskId}")]
    public async Task<IActionResult> Delete(int taskId)
    {
        await taskService.Delete(taskId);
        return NoContent();
    }

    [Authorize(Roles = "Admin, Moderator")]
    [HttpDelete("DeleteTaskToUser/{userId}/{taskId}")]
    public async Task<IActionResult> DeleteTaskToUser(int userId, int taskId)
    {
        await taskService.DeleteTaskForUser(userId, taskId);
        return NoContent();
    }

    #endregion

    #region HttpGet

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTaskId(int taskId)
    {
        var serviceTask = await taskService.GetTask(taskId);
        return Ok(serviceTask);
    }

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpGet("GetAllCompanyTasks/{companyId}")]
    public async Task<IActionResult> GetAllCompanyTasks(int companyId)
    {
        var serviceTasks = await taskService.GetAllCompanyTasks(companyId);
        return Ok(serviceTasks);
    }

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpGet("GetTaskUser/{userId}/{taskId}")]
    public async Task<IActionResult> GetTaskUser(int userId, int taskId)
    {
        var serviceTask = await taskService.GetTaskForUser(userId, taskId);
        return Ok(serviceTask);
    }

    [Authorize(Roles = "Admin, Moderator, User")]
    [HttpGet("GetAllUserTasks/{userId}")]
    public async Task<IActionResult> GetAllUserTasks(int userId)
    {
        var serviceTasks = await taskService.GetAllUserTasks(userId);
        return Ok(serviceTasks);
    }

    #endregion
}
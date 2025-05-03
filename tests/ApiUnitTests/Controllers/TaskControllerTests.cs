using System.Security.Claims;
using Api.Controllers;
using Application.Requests;
using Application.Responses;
using Application.Services;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskStatus = Domain.Enums.TaskStatus;

namespace ApiUnitTests.Controllers;

public class TaskControllerTests
{
    private readonly Mock<ITaskService> _taskServiceMock;
    private readonly TaskController _controller;
    private readonly Faker _faker;

    public TaskControllerTests()
    {
        _taskServiceMock = new Mock<ITaskService>();
        _controller = new TaskController(_taskServiceMock.Object);
        _faker = new Faker();
    }

    #region AddTests

    [Fact]
    public async Task Add_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Description = _faker.Random.String(),
            AssignedUserId = _faker.Random.Int(1, 100),
            ResourceId = _faker.Random.Int(1, 100),
            Status = TaskStatus.Opened,
            CreatedById = _faker.Random.Int(1, 100)
        };
        var taskId = _faker.Random.Int(1, 100);
        _taskServiceMock.Setup(x => x.Add(request)).ReturnsAsync(taskId);

        // Act
        var result = await _controller.Add(request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult?.ActionName.Should().Be(nameof(_controller.GetTaskId));
        createdResult?.RouteValues?["taskId"].Should().Be(taskId);
        createdResult?.Value.Should().Be(taskId);
        _taskServiceMock.Verify(x => x.Add(request), Times.Once());
    }

    [Fact]
    public async Task AssignTaskToUser_ValidIds_ReturnsNoContent()
    {
        // Arrange
        var userId = _faker.Random.Int(1, 100);
        var taskId = _faker.Random.Int(1, 100);
        _taskServiceMock.Setup(x => x.AssignTaskToUser(userId, taskId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AssignTaskToUser(userId, taskId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _taskServiceMock.Verify(x => x.AssignTaskToUser(userId, taskId), Times.Once());
    }

    #endregion

    #region UpdateTests

    [Fact]
    public async Task Update_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new UpdateTaskRequest
        {
            Id = _faker.Random.Int(1, 100),
            Description = _faker.Random.String(),
            AssignedUserId = _faker.Random.Int(1, 100),
            ResourceId = _faker.Random.Int(1, 100),
            Status = TaskStatus.Opened,
        };
        _taskServiceMock.Setup(x => x.Update(request)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _taskServiceMock.Verify(x => x.Update(request), Times.Once());
    }

    [Fact]
    public async Task ReassignTaskToUser_ValidIds_ReturnsNoContent()
    {
        // Arrange
        var newUserId = _faker.Random.Int(1, 100);
        var taskId = _faker.Random.Int(1, 100);
        _taskServiceMock.Setup(x => x.ReassignTaskToUser(newUserId, taskId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ReassignTaskToUser(newUserId, taskId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _taskServiceMock.Verify(x => x.ReassignTaskToUser(newUserId, taskId), Times.Once());
    }

    #endregion

    #region DeleteTests

    [Fact]
    public async Task Delete_ExistingTask_ReturnsNoContent()
    {
        // Arrange
        var taskId = _faker.Random.Int(1, 100);
        _taskServiceMock.Setup(x => x.Delete(taskId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(taskId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _taskServiceMock.Verify(x => x.Delete(taskId), Times.Once());
    }

    [Fact]
    public async Task DeleteTaskToUser_ValidIds_ReturnsNoContent()
    {
        // Arrange
        var userId = _faker.Random.Int(1, 100);
        var taskId = _faker.Random.Int(1, 100);
        _taskServiceMock.Setup(x => x.DeleteTaskForUser(userId, taskId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteTaskToUser(userId, taskId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _taskServiceMock.Verify(x => x.DeleteTaskForUser(userId, taskId), Times.Once());
    }

    [Fact]
    public async Task DeleteTaskToUser_AdminRole_ReturnsNoContent()
    {
        // Arrange
        var userId = _faker.Random.Int(1, 100);
        var taskId = _faker.Random.Int(1, 100);
        var adminUser = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Role, "Admin")
        ]));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = adminUser }
        };

        _taskServiceMock.Setup(x => x.DeleteTaskForUser(userId, taskId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteTaskToUser(userId, taskId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _taskServiceMock.Verify(x => x.DeleteTaskForUser(userId, taskId), Times.Once());
    }

    #endregion

    #region GetTests

    [Fact]
    public async Task GetTaskId_ExistingTask_ReturnsOkWithTask()
    {
        // Arrange
        var taskId = _faker.Random.Int(1, 100);
        var assignedUserId = _faker.Random.Int(1, 100);
        var createdById = _faker.Random.Int(1, 100);
        var resourceId = _faker.Random.Int(1, 100);
        var task = CreatingTaskResponse(assignedUserId, createdById, resourceId);
        _taskServiceMock.Setup(x => x.GetTask(taskId)).ReturnsAsync(task);

        // Act
        var result = await _controller.GetTaskId(taskId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(task);
        _taskServiceMock.Verify(x => x.GetTask(taskId), Times.Once());
    }

    [Fact]
    public async Task GetAllCompanyTasks_ExistingTasks_ReturnsOkWithTasks()
    {
        // Arrange
        var companyId = _faker.Random.Int(1, 100);
        var assignedUserId = _faker.Random.Int(1, 100);
        var createdById = _faker.Random.Int(1, 100);
        var resourceId = _faker.Random.Int(1, 100);
        var tasks = new List<TaskResponse>
        {
            CreatingTaskResponse(assignedUserId, createdById, resourceId),
            CreatingTaskResponse(assignedUserId, createdById, resourceId)
        };
        _taskServiceMock.Setup(x => x.GetAllCompanyTasks(companyId)).ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetAllCompanyTasks(companyId);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(tasks);
        _taskServiceMock.Verify(x => x.GetAllCompanyTasks(companyId), Times.Once());
    }

    [Fact]
    public async Task GetTaskUser_ExistingTask_ReturnsOkWithTask()
    {
        // Arrange
        var taskId = _faker.Random.Int(1, 100);
        var assignedUserId = _faker.Random.Int(1, 100);
        var createdById = _faker.Random.Int(1, 100);
        var resourceId = _faker.Random.Int(1, 100);
        var task = CreatingTaskResponse(assignedUserId, createdById, resourceId);
        _taskServiceMock.Setup(x => x.GetTaskForUser(assignedUserId, taskId)).ReturnsAsync(task);

        // Act
        var result = await _controller.GetTaskUser(assignedUserId, taskId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(task);
        _taskServiceMock.Verify(x => x.GetTaskForUser(assignedUserId, taskId), Times.Once());
    }

    [Fact]
    public async Task GetAllUserTasks_WithTasks_ReturnsOkWithTasks()
    {
        // Arrange
        var assignedUserId = _faker.Random.Int(1, 100);
        var createdById = _faker.Random.Int(1, 100);
        var resourceId = _faker.Random.Int(1, 100);
        var tasks = new List<TaskResponse>
        {
            CreatingTaskResponse(assignedUserId, createdById, resourceId),
            CreatingTaskResponse(assignedUserId, createdById, resourceId)
        };
        _taskServiceMock.Setup(x => x.GetAllUserTasks(assignedUserId)).ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetAllUserTasks(assignedUserId);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(tasks);
        _taskServiceMock.Verify(x => x.GetAllUserTasks(assignedUserId), Times.Once());
    }

    #endregion

    private TaskResponse CreatingTaskResponse(int assignedUserId, int createdById, int resourceId)
    {
        var task = new TaskResponse
        {
            AssignedUserId = assignedUserId,
            CreatedById = createdById,
            Status = TaskStatus.Opened,
            Description = _faker.Random.String(),
            ResourceId = resourceId,
            StartTime = DateTime.Now
        };

        return task;
    }
}
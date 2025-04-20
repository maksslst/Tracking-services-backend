using Application.Exceptions;
using Application.Mappings;
using Application.Requests;
using Application.Services;
using AutoMapper;
using Bogus;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Repositories.CompanyRepository;
using Infrastructure.Repositories.ResourceRepository;
using Infrastructure.Repositories.TaskRepository;
using Infrastructure.Repositories.UserRepository;
using Microsoft.Extensions.Logging;
using Moq;
using TaskStatus = Domain.Enums.TaskStatus;

namespace ApplicationUnitTests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly Mock<ILogger<TaskService>> _loggerMock;
    private readonly ITaskService _taskService;
    private readonly Faker _faker;

    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        var resourceRepositoryMock = new Mock<IResourceRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _loggerMock = new Mock<ILogger<TaskService>>();
        _faker = new Faker();

        var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = mappingConfig.CreateMapper();

        _taskService = new TaskService(_taskRepositoryMock.Object, mapper, resourceRepositoryMock.Object,
            _userRepositoryMock.Object, _companyRepositoryMock.Object, _loggerMock.Object);
    }

    #region AddTests

    [Fact]
    public async Task Add_WhenValidRequest_ReturnsTaskId()
    {
        // Arrange
        var request = new CreateTaskRequest()
        {
            ResourceId = _faker.Random.Int(1, 100),
            Description = _faker.Lorem.Sentence(),
            CreatedById = _faker.Random.Int(1, 100),
            Status = TaskStatus.Opened
        };
        _taskRepositoryMock.Setup(i => i.CreateTask(It.IsAny<ServiceTask>())).ReturnsAsync(1);

        // Act
        var result = await _taskService.Add(request);

        // Assert
        result.Should().Be(1);
        _taskRepositoryMock.Verify(i => i.CreateTask(It.Is<ServiceTask>(t =>
            t.ResourceId == request.ResourceId &&
            t.Description == request.Description &&
            t.CreatedById == request.CreatedById &&
            t.Status == request.Status)), Times.Once());
    }

    #endregion

    #region UpdateTests

    [Fact]
    public async Task Update_WhenTaskAndUserExistInSameCompany_UpdatesTaskSuccessfully()
    {
        var companyId = _faker.Random.Int(1, 100);
        var request = new UpdateTaskRequest
        {
            Id = _faker.Random.Int(1, 100),
            ResourceId = _faker.Random.Int(1, 100),
            AssignedUserId = _faker.Random.Int(1, 100),
            Description = _faker.Lorem.Sentence(),
            Status = TaskStatus.Completed
        };
        var user = new User
        {
            Id = request.AssignedUserId,
            CompanyId = companyId,
            Email = _faker.Person.Email,
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            Username = _faker.Person.UserName
        };
        var task = new ServiceTask
        {
            Id = request.Id,
            ResourceId = _faker.Random.Int(1, 100),
            AssignedUserId = request.AssignedUserId,
            AssignedUser = new User
            {
                Id = request.AssignedUserId,
                CompanyId = companyId,
                Email = _faker.Person.Email,
                FirstName = _faker.Person.FirstName,
                LastName = _faker.Person.LastName,
                Username = _faker.Person.UserName
            },
            Description = _faker.Random.String(),
            CreatedById = _faker.Random.Int(1, 100),
            StartTime = DateTime.Now,
            Status = TaskStatus.InProgress
        };

        _taskRepositoryMock.Setup(i => i.ReadTaskId(request.Id)).ReturnsAsync(task);
        _userRepositoryMock.Setup(i => i.ReadById(request.AssignedUserId)).ReturnsAsync(user);
        _taskRepositoryMock.Setup(i => i.UpdateTask(It.Is<ServiceTask>(t =>
                t.Id == request.Id &&
                t.ResourceId == request.ResourceId &&
                t.AssignedUserId == request.AssignedUserId &&
                t.Description == request.Description &&
                t.Status == request.Status), user))
            .ReturnsAsync(true);

        // Act
        await _taskService.Update(request);

        // Assert
        _taskRepositoryMock.Verify(i => i.ReadTaskId(request.Id), Times.Once());
        _userRepositoryMock.Verify(i => i.ReadById(request.AssignedUserId), Times.Once());
        _taskRepositoryMock.Verify(i => i.UpdateTask(It.Is<ServiceTask>(t =>
            t.Id == request.Id &&
            t.ResourceId == request.ResourceId &&
            t.AssignedUserId == request.AssignedUserId &&
            t.Description == request.Description &&
            t.Status == request.Status), user), Times.Once());
    }

    [Fact]
    public async Task Update_WhenTaskNotFound_ThrowsNotFoundApplicationException()
    {
        // Arrange
        var request = new UpdateTaskRequest
        {
            Id = _faker.Random.Int(1, 100),
            AssignedUserId = _faker.Random.Int(1, 100),
            ResourceId = _faker.Random.Int(1, 100),
            Description = _faker.Lorem.Sentence(),
            Status = TaskStatus.Completed
        };
        _taskRepositoryMock.Setup(i => i.ReadTaskId(request.Id)).ReturnsAsync((ServiceTask)null);

        // Act & Assert
        await _taskService.Invoking(i => i.Update(request))
            .Should().ThrowAsync<NotFoundApplicationException>()
            .WithMessage("Task not found");

        _taskRepositoryMock.Verify(i => i.ReadTaskId(request.Id), Times.Once());
        _userRepositoryMock.Verify(i => i.ReadById(It.IsAny<int>()), Times.Never());
        _taskRepositoryMock.Verify(i => i.UpdateTask(It.IsAny<ServiceTask>(), It.IsAny<User>()), Times.Never());
    }

    [Fact]
    public async Task Update_WhenUserInDifferentCompany_ThrowsUserAuthorizationException()
    {
        // Arrange
        var taskCompanyId = _faker.Random.Int(1, 100);
        var userCompanyId = _faker.Random.Int(101, 200);
        var request = new UpdateTaskRequest
        {
            Id = _faker.Random.Int(1, 100),
            ResourceId = _faker.Random.Int(1, 100),
            AssignedUserId = _faker.Random.Int(1, 100),
            Description = _faker.Lorem.Sentence(),
            Status = TaskStatus.Completed
        };
        var task = new ServiceTask
        {
            Id = request.Id,
            AssignedUserId = request.AssignedUserId,
            AssignedUser = new User
            {
                Id = request.AssignedUserId,
                CompanyId = taskCompanyId,
                Email = _faker.Person.Email,
                FirstName = _faker.Person.FirstName,
                LastName = _faker.Person.LastName,
                Username = _faker.Person.UserName
            },
            Description = _faker.Random.String(),
            Status = TaskStatus.InProgress,
            CreatedById = _faker.Random.Int(1, 100),
            StartTime = DateTime.Now,
            ResourceId = _faker.Random.Int(1, 100)
        };
        var user = new User
        {
            Id = request.AssignedUserId,
            CompanyId = userCompanyId,
            Email = _faker.Person.Email,
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            Username = _faker.Person.UserName
        };

        _taskRepositoryMock.Setup(i => i.ReadTaskId(request.Id)).ReturnsAsync(task);
        _userRepositoryMock.Setup(i => i.ReadById(request.AssignedUserId)).ReturnsAsync(user);

        // Act & Assert
        await _taskService.Invoking(i => i.Update(request))
            .Should().ThrowAsync<UserAuthorizationException>()
            .WithMessage("User is not in the company");

        _taskRepositoryMock.Verify(i => i.ReadTaskId(request.Id), Times.Once());
        _userRepositoryMock.Verify(i => i.ReadById(request.AssignedUserId), Times.Once());
        _taskRepositoryMock.Verify(i => i.UpdateTask(It.IsAny<ServiceTask>(), It.IsAny<User>()), Times.Never());
    }

    [Fact]
    public async Task Update_WhenUpdateFails_ThrowsEntityUpdateException()
    {
        // Arrange
        var companyId = _faker.Random.Int(1, 100);
        var request = new UpdateTaskRequest
        {
            Id = _faker.Random.Int(1, 100),
            ResourceId = _faker.Random.Int(1, 100),
            AssignedUserId = _faker.Random.Int(1, 100),
            Description = _faker.Lorem.Sentence(),
            Status = TaskStatus.InProgress
        };
        var task = new ServiceTask
        {
            Id = request.Id,
            AssignedUserId = request.AssignedUserId,
            AssignedUser = new User
            {
                Id = request.AssignedUserId,
                CompanyId = companyId,
                Email = _faker.Person.Email,
                FirstName = _faker.Person.FirstName,
                LastName = _faker.Person.LastName,
                Username = _faker.Person.UserName
            },
            Description = _faker.Random.String(),
            Status = TaskStatus.Completed,
            CreatedById = _faker.Random.Int(1, 100),
            StartTime = DateTime.Now,
            ResourceId = _faker.Random.Int(1, 100)
        };
        var user = new User
        {
            Id = request.AssignedUserId,
            CompanyId = companyId,
            Email = _faker.Person.Email,
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            Username = _faker.Person.UserName
        };

        _taskRepositoryMock.Setup(i => i.ReadTaskId(request.Id)).ReturnsAsync(task);
        _userRepositoryMock.Setup(i => i.ReadById(request.AssignedUserId)).ReturnsAsync(user);
        _taskRepositoryMock.Setup(i => i.UpdateTask(It.IsAny<ServiceTask>(), user)).ReturnsAsync(false);

        // Act & Assert
        await _taskService.Invoking(i => i.Update(request))
            .Should()
            .ThrowAsync<EntityUpdateException>()
            .WithMessage("Failed to update task");

        // Assert
        _taskRepositoryMock.Verify(i => i.ReadTaskId(request.Id), Times.Once());
        _userRepositoryMock.Verify(i => i.ReadById(request.AssignedUserId), Times.Once());
        _taskRepositoryMock.Verify(i => i.UpdateTask(It.IsAny<ServiceTask>(), user), Times.Once());
    }

    #endregion

    #region DeleteTests

    [Fact]
    public async Task Delete_WhenTaskExists_DeletesTaskSuccessfully()
    {
        // Arrange
        int taskId = _faker.Random.Int(1, 100);
        _taskRepositoryMock.Setup(i => i.DeleteTask(taskId)).ReturnsAsync(true);

        // Act
        await _taskService.Delete(taskId);

        // Assert
        _taskRepositoryMock.Verify(i => i.DeleteTask(taskId), Times.Once());
    }

    [Fact]
    public async Task Delete_WhenDeletionFails_ThrowsEntityDeleteException()
    {
        // Arrange
        int taskId = _faker.Random.Int(1, 100);
        _taskRepositoryMock.Setup(i => i.DeleteTask(taskId)).ReturnsAsync(false);

        // Act & Assert
        await _taskService.Invoking(i => i.Delete(taskId))
            .Should().ThrowAsync<EntityDeleteException>()
            .WithMessage("Couldn't delete task");

        _taskRepositoryMock.Verify(i => i.DeleteTask(taskId), Times.Once());
    }

    #endregion

    #region GetTaskTests

    [Fact]
    public async Task GetTask_WhenTaskExists_ReturnsTaskResponse()
    {
        // Arrange
        int taskId = _faker.Random.Int(1, 100);
        var task = new ServiceTask
        {
            Id = taskId,
            ResourceId = _faker.Random.Int(1, 100),
            Description = _faker.Lorem.Sentence(),
            CreatedById = _faker.Random.Int(1, 100),
            StartTime = DateTime.Now,
            Status = Domain.Enums.TaskStatus.Opened
        };
        _taskRepositoryMock.Setup(i => i.ReadTaskId(taskId)).ReturnsAsync(task);

        // Act
        var result = await _taskService.GetTask(taskId);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(task.Description);
        result.CreatedById.Should().Be(task.CreatedById);
        result.StartTime.Should().Be(task.StartTime);
        result.Status.Should().Be(Domain.Enums.TaskStatus.Opened);
        result.ResourceId.Should().Be(task.ResourceId);
        result.CompletionTime.Should().Be(task.CompletionTime);
        result.AssignedUserId.Should().Be(task.AssignedUserId);
        _taskRepositoryMock.Verify(i => i.ReadTaskId(taskId), Times.Once());
    }

    [Fact]
    public async Task GetTask_WhenTaskNotFound_ThrowsNotFoundApplicationException()
    {
        // Arrange
        int taskId = _faker.Random.Int(1, 100);
        _taskRepositoryMock.Setup(i => i.ReadTaskId(taskId)).ReturnsAsync((ServiceTask)null);

        // Act & Assert
        await _taskService.Invoking(i => i.GetTask(taskId))
            .Should().ThrowAsync<NotFoundApplicationException>()
            .WithMessage("Task not found");

        _taskRepositoryMock.Verify(i => i.ReadTaskId(taskId), Times.Once());
    }

    #endregion

    #region GetAllCompanyTasksTests

    [Fact]
    public async Task GetAllCompanyTasks_WhenTasksExist_ReturnsTaskResponses()
    {
        // Arrange
        int companyId = _faker.Random.Int(1, 100);
        var company = new Company { Id = companyId, CompanyName = _faker.Company.CompanyName() };
        var tasks = new List<ServiceTask>
        {
            new ServiceTask
            {
                Id = 1, ResourceId = _faker.Random.Int(1, 100), Description = _faker.Lorem.Sentence(),
                CreatedById = _faker.Random.Int(1, 100), StartTime = DateTime.Now,
                Status = Domain.Enums.TaskStatus.Completed
            },
            new ServiceTask
            {
                Id = 2, ResourceId = _faker.Random.Int(1, 100), Description = _faker.Lorem.Sentence(),
                CreatedById = _faker.Random.Int(1, 100), StartTime = DateTime.Now,
                Status = Domain.Enums.TaskStatus.Opened
            }
        };
        _companyRepositoryMock.Setup(i => i.ReadByCompanyId(companyId)).ReturnsAsync(company);
        _taskRepositoryMock.Setup(i => i.ReadAllTasksCompanyId(companyId)).ReturnsAsync(tasks);

        // Act
        var result = await _taskService.GetAllCompanyTasks(companyId);

        // Assert
        result.Should().HaveCount(2);
        result.Select(r => r.Description).Should().Contain(tasks.Select(t => t.Description));
        _companyRepositoryMock.Verify(i => i.ReadByCompanyId(companyId), Times.Once());
        _taskRepositoryMock.Verify(i => i.ReadAllTasksCompanyId(companyId), Times.Once());
    }

    [Fact]
    public async Task GetAllCompanyTasks_WhenCompanyNotFound_ThrowsNotFoundApplicationException()
    {
        // Arrange
        int companyId = _faker.Random.Int(1, 100);
        _companyRepositoryMock.Setup(i => i.ReadByCompanyId(companyId)).ReturnsAsync((Company)null);

        // Act & Assert
        await _taskService.Invoking(i => i.GetAllCompanyTasks(companyId))
            .Should().ThrowAsync<NotFoundApplicationException>()
            .WithMessage("Company not found");

        _companyRepositoryMock.Verify(i => i.ReadByCompanyId(companyId), Times.Once());
        _taskRepositoryMock.Verify(i => i.ReadAllTasksCompanyId(It.IsAny<int>()), Times.Never());
    }

    [Fact]
    public async Task GetAllCompanyTasks_WhenNoTasksExist_ReturnsEmptyCollection()
    {
        // Arrange
        int companyId = _faker.Random.Int(1, 100);
        var company = new Company { Id = companyId, CompanyName = _faker.Company.CompanyName() };
        _companyRepositoryMock.Setup(i => i.ReadByCompanyId(companyId)).ReturnsAsync(company);
        _taskRepositoryMock.Setup(i => i.ReadAllTasksCompanyId(companyId)).ReturnsAsync(new List<ServiceTask>());

        // Act
        var result = await _taskService.GetAllCompanyTasks(companyId);

        // Assert
        result.Should().BeEmpty();
        _companyRepositoryMock.Verify(i => i.ReadByCompanyId(companyId), Times.Once());
        _taskRepositoryMock.Verify(i => i.ReadAllTasksCompanyId(companyId), Times.Once());
    }

    #endregion

    #region AssignTaskToUserTests

    [Fact]
    public async Task AssignTaskToUser_WhenAssignmentSucceeds_AssignsTaskSuccessfully()
    {
        // Arrange
        int userId = _faker.Random.Int(1, 100);
        int taskId = _faker.Random.Int(1, 100);
        _taskRepositoryMock.Setup(i => i.SetTaskAssignment(userId, taskId, true)).ReturnsAsync(true);

        // Act
        await _taskService.AssignTaskToUser(userId, taskId);

        // Assert
        _taskRepositoryMock.Verify(i => i.SetTaskAssignment(userId, taskId, true), Times.Once());
    }

    [Fact]
    public async Task AssignTaskToUser_WhenAssignmentFails_ThrowsEntityUpdateException()
    {
        // Arrange
        int userId = _faker.Random.Int(1, 100);
        int taskId = _faker.Random.Int(1, 100);
        _taskRepositoryMock.Setup(i => i.SetTaskAssignment(userId, taskId, true)).ReturnsAsync(false);

        // Act & Assert
        await _taskService.Invoking(i => i.AssignTaskToUser(userId, taskId))
            .Should().ThrowAsync<EntityUpdateException>()
            .WithMessage("Failed to assign task to user");

        // Assert
        _taskRepositoryMock.Verify(i => i.SetTaskAssignment(userId, taskId, true), Times.Once());
    }

    #endregion

    #region DeleteTaskForUserTests

    [Fact]
    public async Task DeleteTaskForUser_WhenUnassignmentSucceeds_UnassignsTaskSuccessfully()
    {
        // Arrange
        int userId = _faker.Random.Int(1, 100);
        int taskId = _faker.Random.Int(1, 100);
        _taskRepositoryMock.Setup(i => i.SetTaskAssignment(userId, taskId, false)).ReturnsAsync(true);

        // Act
        await _taskService.DeleteTaskForUser(userId, taskId);

        // Assert
        _taskRepositoryMock.Verify(i => i.SetTaskAssignment(userId, taskId, false), Times.Once());
    }

    [Fact]
    public async Task DeleteTaskForUser_WhenUnassignmentFails_ThrowsEntityUpdateException()
    {
        // Arrange
        int userId = _faker.Random.Int(1, 100);
        int taskId = _faker.Random.Int(1, 100);
        _taskRepositoryMock.Setup(i => i.SetTaskAssignment(userId, taskId, false)).ReturnsAsync(false);

        // Act & Assert
        await _taskService.Invoking(i => i.DeleteTaskForUser(userId, taskId))
            .Should().ThrowAsync<EntityUpdateException>()
            .WithMessage("Failed to unassign task from user");

        // Assert
        _taskRepositoryMock.Verify(i => i.SetTaskAssignment(userId, taskId, false), Times.Once());
    }

    #endregion

    #region ReassignTaskToUserTests

    [Fact]
    public async Task ReassignTaskToUser_WhenReassignmentSucceeds_ReassignsTaskSuccessfully()
    {
        // Arrange
        int newUserId = _faker.Random.Int(1, 100);
        int taskId = _faker.Random.Int(1, 100);
        _taskRepositoryMock.Setup(i => i.SetTaskAssignment(newUserId, taskId, true)).ReturnsAsync(true);

        // Act
        await _taskService.ReassignTaskToUser(newUserId, taskId);

        // Assert
        _taskRepositoryMock.Verify(i => i.SetTaskAssignment(newUserId, taskId, true), Times.Once());
    }

    [Fact]
    public async Task ReassignTaskToUser_WhenReassignmentFails_ThrowsEntityUpdateException()
    {
        // Arrange
        int newUserId = _faker.Random.Int(1, 100);
        int taskId = _faker.Random.Int(1, 100);
        _taskRepositoryMock.Setup(i => i.SetTaskAssignment(newUserId, taskId, true)).ReturnsAsync(false);

        // Act & Assert
        await _taskService.Invoking(i => i.ReassignTaskToUser(newUserId, taskId))
            .Should().ThrowAsync<EntityUpdateException>()
            .WithMessage("Couldn't reassign task");

        // Assert
        _taskRepositoryMock.Verify(i => i.SetTaskAssignment(newUserId, taskId, true), Times.Once());
    }

    #endregion

    #region GetTaskForUserTests

    [Fact]
    public async Task GetTaskForUser_WhenTaskBelongsToUser_ReturnsTaskResponse()
    {
        // Arrange
        int userId = _faker.Random.Int(1, 100);
        int taskId = _faker.Random.Int(1, 100);
        var task = new ServiceTask
        {
            Id = taskId,
            ResourceId = _faker.Random.Int(1, 100),
            AssignedUserId = userId,
            Description = _faker.Lorem.Sentence(),
            CreatedById = _faker.Random.Int(1, 100),
            StartTime = DateTime.Now,
            Status = Domain.Enums.TaskStatus.Completed
        };
        _taskRepositoryMock.Setup(i => i.ReadTaskId(taskId)).ReturnsAsync(task);

        // Act
        var result = await _taskService.GetTaskForUser(userId, taskId);

        // Assert
        result.Should().NotBeNull();
        result.AssignedUserId.Should().Be(task.AssignedUserId);
        result.Description.Should().Be(task.Description);
        result.CreatedById.Should().Be(task.CreatedById);
        result.StartTime.Should().Be(task.StartTime);
        result.Status.Should().Be(Domain.Enums.TaskStatus.Completed);
        result.ResourceId.Should().Be(task.ResourceId);
        _taskRepositoryMock.Verify(i => i.ReadTaskId(taskId), Times.Once());
    }

    [Fact]
    public async Task GetTaskForUser_WhenTaskNotFound_ThrowsUserAuthorizationException()
    {
        // Arrange
        int userId = _faker.Random.Int(1, 100);
        int taskId = _faker.Random.Int(1, 100);
        _taskRepositoryMock.Setup(i => i.ReadTaskId(taskId)).ReturnsAsync((ServiceTask)null);

        // Act & Assert
        await _taskService.Invoking(i => i.GetTaskForUser(userId, taskId))
            .Should().ThrowAsync<UserAuthorizationException>()
            .WithMessage("This user does not own this task");

        // Assert
        _taskRepositoryMock.Verify(i => i.ReadTaskId(taskId), Times.Once());
    }

    [Fact]
    public async Task GetTaskForUser_WhenTaskNotAssignedToUser_ThrowsUserAuthorizationException()
    {
        // Arrange
        int userId = _faker.Random.Int(1, 100);
        int taskId = _faker.Random.Int(1, 100);
        var task = new ServiceTask
        {
            Id = taskId,
            ResourceId = _faker.Random.Int(1, 100),
            AssignedUserId = _faker.Random.Int(101, 200), // Different user
            Description = _faker.Lorem.Sentence(),
            CreatedById = _faker.Random.Int(1, 100),
            StartTime = DateTime.Now,
            Status = Domain.Enums.TaskStatus.Completed
        };
        _taskRepositoryMock.Setup(i => i.ReadTaskId(taskId)).ReturnsAsync(task);

        // Act & Assert
        await _taskService.Invoking(i => i.GetTaskForUser(userId, taskId))
            .Should().ThrowAsync<UserAuthorizationException>()
            .WithMessage("This user does not own this task");

        // Assert
        _taskRepositoryMock.Verify(i => i.ReadTaskId(taskId), Times.Once());
    }

    #endregion

    #region GetAllUserTasksTests

    [Fact]
    public async Task GetAllUserTasks_WhenTasksExist_ReturnsTaskResponses()
    {
        // Arrange
        int userId = _faker.Random.Int(1, 100);
        var tasks = new List<ServiceTask>
        {
            new ServiceTask
            {
                Id = 1, ResourceId = _faker.Random.Int(1, 100), AssignedUserId = userId,
                Description = _faker.Lorem.Sentence(), CreatedById = _faker.Random.Int(1, 100),
                StartTime = DateTime.Now, Status = Domain.Enums.TaskStatus.Opened
            },
            new ServiceTask
            {
                Id = 2, ResourceId = _faker.Random.Int(1, 100), AssignedUserId = userId,
                Description = _faker.Lorem.Sentence(), CreatedById = _faker.Random.Int(1, 100),
                StartTime = DateTime.Now, Status = TaskStatus.Opened
            }
        };
        _taskRepositoryMock.Setup(i => i.ReadAllUserTasks(userId)).ReturnsAsync(tasks);

        // Act
        var result = await _taskService.GetAllUserTasks(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Select(r => r.Description).Should().Contain(tasks.Select(t => t.Description));
        _taskRepositoryMock.Verify(i => i.ReadAllUserTasks(userId), Times.Once());
    }

    [Fact]
    public async Task GetAllUserTasks_WhenNoTasksExist_ReturnsEmptyCollection()
    {
        // Arrange
        int userId = _faker.Random.Int(1, 100);
        _taskRepositoryMock.Setup(i => i.ReadAllUserTasks(userId)).ReturnsAsync(new List<ServiceTask>());

        // Act
        var result = await _taskService.GetAllUserTasks(userId);

        // Assert
        result.Should().BeEmpty();
        _taskRepositoryMock.Verify(i => i.ReadAllUserTasks(userId), Times.Once());
    }

    #endregion
}
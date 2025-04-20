using Application.Exceptions;
using Application.Requests;
using Application.Services;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TaskStatus = Domain.Enums.TaskStatus;

namespace ApplicationIntegrationTests.Service;

public class TaskServiceTests : IClassFixture<TestingFixture>
{
    private readonly ITaskService _taskService;
    private readonly TestingFixture _fixture;
    private readonly Faker _faker;

    public TaskServiceTests(TestingFixture fixture)
    {
        _fixture = fixture;
        var scope = fixture.ServiceProvider.CreateScope();
        _taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();
        _faker = new Faker();
    }
    
    [Fact]
    public async Task Add_ShouldCreateTask()
    {
        // Arrange
        await _fixture.DisposeAsync();
        var company = await _fixture.CreateCompany();
        var assignedUser = await _fixture.CreateUser(company.Id);
        var createdUser = await _fixture.CreateUser(company.Id);
        var resource = await _fixture.CreateResource(company.Id);
        var request = new CreateTaskRequest
        {
            Description = _faker.Random.Words(),
            AssignedUserId = assignedUser.Id,
            CreatedById = createdUser.Id,
            ResourceId = resource.Id,
            Status = TaskStatus.Opened
        };

        // Act
        var id = await _taskService.Add(request);

        // Assert
        var result = await _taskService.GetTask(id);
        result.Should().NotBeNull();
        result.Description.Should().Be(request.Description);
        result.AssignedUserId.Should().Be(request.AssignedUserId);
        result.CreatedById.Should().Be(request.CreatedById);
        result.ResourceId.Should().Be(request.ResourceId);
        result.Status.Should().Be(request.Status);
    }
    
    [Fact]
    public async Task Delete_ShouldRemoveTask()
    {
        // Arrange
        await _fixture.DisposeAsync();
        var company = await _fixture.CreateCompany();
        var assignedUser = await _fixture.CreateUser(company.Id);
        var createdUser = await _fixture.CreateUser(company.Id);
        var resource = await _fixture.CreateResource(company.Id);
        var task = await _fixture.CreateServiceTask(resource.Id, assignedUser.Id, createdUser.Id);

        // Act
       await _taskService.Delete(task.Id);

        // Assert
        await _taskService.Invoking(i => i.GetTask(task.Id))
            .Should()
            .ThrowAsync<NotFoundApplicationException>()
            .WithMessage("Task not found");
    }
    
    [Fact]
    public async Task GetTask_ShouldReturnCorrectTask()
    {
        // Arrange
        await _fixture.DisposeAsync();
        var company = await _fixture.CreateCompany();
        var assignedUser = await _fixture.CreateUser(company.Id);
        var createdUser = await _fixture.CreateUser(company.Id);
        var resource = await _fixture.CreateResource(company.Id);
        var task = await _fixture.CreateServiceTask(resource.Id, assignedUser.Id, createdUser.Id);

        // Act
        var result = await _taskService.GetTask(task.Id);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(task.Description);
        result.AssignedUserId.Should().Be(task.AssignedUserId);
        result.CreatedById.Should().Be(task.CreatedById);
        result.ResourceId.Should().Be(task.ResourceId);
        result.Status.Should().Be(task.Status);
    }
}
using DevProject.Application.DTOs.Tasks;
using DevProject.Application.Mappers;
using DevProject.Domain.Entities;
using DevProject.Domain.Enums;

namespace DevProject.Application.Tests.Mappers;

public sealed class ProjectTaskMapperTests
{
    private readonly ProjectTaskMapper _mapper = new();

    [Fact]
    public void ToEntity_maps_request_and_feature_id()
    {
        var featureId = Guid.NewGuid();
        var dueDate = DateTimeOffset.UtcNow.AddDays(3);
        var request = new CreateProjectTaskRequest(
            "Task",
            "Description",
            TaskPriority.Critical,
            dueDate,
            9);

        var task = _mapper.ToEntity(featureId, request);

        Assert.Equal(featureId, task.FeatureId);
        Assert.Equal("Task", task.Title);
        Assert.Equal("Description", task.Description);
        Assert.Equal(ProjectTaskStatus.Todo, task.Status);
        Assert.Equal(TaskPriority.Critical, task.Priority);
        Assert.Equal(dueDate, task.DueDate);
        Assert.Equal(9, task.Order);
    }

    [Fact]
    public void ToEntity_rejects_null_request()
    {
        Assert.Throws<ArgumentNullException>(() => _mapper.ToEntity(Guid.NewGuid(), null!));
    }

    [Fact]
    public void ToResponse_maps_all_task_fields()
    {
        var dueDate = DateTimeOffset.UtcNow.AddDays(1);
        var task = new ProjectTask(
            Guid.NewGuid(),
            "Task",
            "Description",
            TaskPriority.High,
            dueDate,
            3);
        task.ChangeStatus(ProjectTaskStatus.Done);

        var response = _mapper.ToResponse(task);

        Assert.Equal(task.Id, response.Id);
        Assert.Equal(task.FeatureId, response.FeatureId);
        Assert.Equal(task.Title, response.Title);
        Assert.Equal(task.Description, response.Description);
        Assert.Equal(task.Status, response.Status);
        Assert.Equal(task.Priority, response.Priority);
        Assert.Equal(task.DueDate, response.DueDate);
        Assert.Equal(task.CompletedAt, response.CompletedAt);
        Assert.Equal(task.Order, response.Order);
        Assert.Equal(task.CreatedAt, response.CreatedAt);
        Assert.Equal(task.UpdatedAt, response.UpdatedAt);
    }

    [Fact]
    public void ToResponse_rejects_null_task()
    {
        Assert.Throws<ArgumentNullException>(() => _mapper.ToResponse(null!));
    }

    [Fact]
    public void UpdateEntity_applies_all_request_fields()
    {
        var task = new ProjectTask(Guid.NewGuid(), "Old", "Old description", order: 2);
        var dueDate = DateTimeOffset.UtcNow.AddDays(7);
        var request = new UpdateProjectTaskRequest(
            "Updated",
            "New description",
            TaskPriority.High,
            dueDate,
            11);

        _mapper.UpdateEntity(task, request);

        Assert.Equal("Updated", task.Title);
        Assert.Equal("New description", task.Description);
        Assert.Equal(TaskPriority.High, task.Priority);
        Assert.Equal(dueDate, task.DueDate);
        Assert.Equal(11, task.Order);
    }

    [Fact]
    public void UpdateEntity_does_not_mutate_task_when_request_is_invalid()
    {
        var task = new ProjectTask(Guid.NewGuid(), "Original", "Original description", order: 2);
        var request = new UpdateProjectTaskRequest(
            "Changed",
            "Changed description",
            (TaskPriority)999,
            DateTimeOffset.UtcNow,
            3);

        Assert.Throws<ArgumentException>(() => _mapper.UpdateEntity(task, request));
        Assert.Equal("Original", task.Title);
        Assert.Equal("Original description", task.Description);
        Assert.Equal(TaskPriority.Medium, task.Priority);
        Assert.Null(task.DueDate);
        Assert.Equal(2, task.Order);
        Assert.Null(task.UpdatedAt);
    }

    [Fact]
    public void UpdateStatus_returns_domain_validation_result()
    {
        var task = new ProjectTask(Guid.NewGuid(), "Task");

        Assert.Throws<ArgumentException>(() => _mapper.UpdateStatus(task, (ProjectTaskStatus)999));
        Assert.Equal(ProjectTaskStatus.Todo, task.Status);
    }

    [Fact]
    public void UpdateOrder_applies_requested_order()
    {
        var task = new ProjectTask(Guid.NewGuid(), "Task");

        _mapper.UpdateOrder(task, 6);

        Assert.Equal(6, task.Order);
    }

    [Fact]
    public void Update_methods_reject_null_arguments()
    {
        var task = new ProjectTask(Guid.NewGuid(), "Task");
        var request = new UpdateProjectTaskRequest("Updated", null, TaskPriority.Low, null, 0);

        Assert.Throws<ArgumentNullException>(() => _mapper.UpdateEntity(null!, request));
        Assert.Throws<ArgumentNullException>(() => _mapper.UpdateEntity(task, null!));
        Assert.Throws<ArgumentNullException>(() => _mapper.UpdateStatus(null!, ProjectTaskStatus.Done));
        Assert.Throws<ArgumentNullException>(() => _mapper.UpdateOrder(null!, 1));
    }
}

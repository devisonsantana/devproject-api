using DevProject.Application.DTOs.Tasks;
using DevProject.Domain.Enums;

namespace DevProject.Application.Tests.DTOs.Tasks;

public sealed class ProjectTaskDtoTests
{
    [Fact]
    public void CreateProjectTaskRequest_uses_optional_defaults()
    {
        var request = new CreateProjectTaskRequest("Task", null);

        Assert.Equal("Task", request.Title);
        Assert.Null(request.Description);
        Assert.Equal(TaskPriority.Medium, request.Priority);
        Assert.Null(request.DueDate);
        Assert.Equal(0, request.Order);
    }

    [Fact]
    public void CreateProjectTaskRequest_preserves_all_explicit_values()
    {
        var dueDate = DateTimeOffset.UtcNow.AddDays(2);
        var request = new CreateProjectTaskRequest(
            "Task",
            "Description",
            TaskPriority.Critical,
            dueDate,
            8);

        Assert.Equal("Description", request.Description);
        Assert.Equal(TaskPriority.Critical, request.Priority);
        Assert.Equal(dueDate, request.DueDate);
        Assert.Equal(8, request.Order);
    }

    [Fact]
    public void UpdateProjectTaskRequest_preserves_all_values_and_nullable_due_date()
    {
        var request = new UpdateProjectTaskRequest(
            "Task",
            null,
            TaskPriority.High,
            null,
            5);

        Assert.Equal("Task", request.Title);
        Assert.Null(request.Description);
        Assert.Equal(TaskPriority.High, request.Priority);
        Assert.Null(request.DueDate);
        Assert.Equal(5, request.Order);
    }

    [Fact]
    public void UpdateTaskStatusRequest_preserves_status()
    {
        var request = new UpdateTaskStatusRequest(ProjectTaskStatus.Blocked);

        Assert.Equal(ProjectTaskStatus.Blocked, request.Status);
    }

    [Fact]
    public void ProjectTaskResponse_preserves_all_values_and_nullable_fields()
    {
        var id = Guid.NewGuid();
        var featureId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var dueDate = createdAt.AddDays(1);

        var response = new ProjectTaskResponse(
            id,
            featureId,
            "Task",
            null,
            ProjectTaskStatus.Todo,
            TaskPriority.Low,
            dueDate,
            null,
            2,
            createdAt,
            null);

        Assert.Equal(id, response.Id);
        Assert.Equal(featureId, response.FeatureId);
        Assert.Equal("Task", response.Title);
        Assert.Null(response.Description);
        Assert.Equal(ProjectTaskStatus.Todo, response.Status);
        Assert.Equal(TaskPriority.Low, response.Priority);
        Assert.Equal(dueDate, response.DueDate);
        Assert.Null(response.CompletedAt);
        Assert.Equal(2, response.Order);
        Assert.Equal(createdAt, response.CreatedAt);
        Assert.Null(response.UpdatedAt);
    }
}

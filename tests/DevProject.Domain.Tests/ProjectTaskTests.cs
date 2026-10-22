using DevProject.Domain.Entities;
using DevProject.Domain.Enums;

namespace DevProject.Domain.Tests;

public sealed class ProjectTaskTests
{
    [Fact]
    public void Task_is_created_with_todo_status_and_medium_priority_by_default()
    {
        var featureId = Guid.NewGuid();
        var task = new ProjectTask(featureId, "  Implement feature  ");

        Assert.Equal(featureId, task.FeatureId);
        Assert.Equal("Implement feature", task.Title);
        Assert.Equal(ProjectTaskStatus.Todo, task.Status);
        Assert.Equal(TaskPriority.Medium, task.Priority);
        Assert.Null(task.Description);
        Assert.Null(task.CompletedAt);
    }

    [Fact]
    public void Blank_title_is_normalized_as_invalid_without_mutation()
    {
        var task = new ProjectTask(Guid.NewGuid(), "Before", "Description");

        var action = () => task.UpdateDetails(" ", "Changed", TaskPriority.High, null);

        Assert.Throws<ArgumentException>(action);
        Assert.Equal("Before", task.Title);
        Assert.Equal("Description", task.Description);
        Assert.Equal(TaskPriority.Medium, task.Priority);
        Assert.Null(task.UpdatedAt);
    }

    [Fact]
    public void Marking_done_sets_completion_time_and_leaving_done_clears_it()
    {
        var task = new ProjectTask(Guid.NewGuid(), "Task");

        task.ChangeStatus(ProjectTaskStatus.Done);
        Assert.NotNull(task.CompletedAt);

        task.ChangeStatus(ProjectTaskStatus.InProgress);

        Assert.Null(task.CompletedAt);
        Assert.Equal(ProjectTaskStatus.InProgress, task.Status);
    }

    [Fact]
    public void Invalid_status_does_not_change_task()
    {
        var task = new ProjectTask(Guid.NewGuid(), "Task");

        var action = () => task.ChangeStatus((ProjectTaskStatus)999);

        Assert.Throws<ArgumentException>(action);
        Assert.Equal(ProjectTaskStatus.Todo, task.Status);
        Assert.Null(task.UpdatedAt);
    }
}

using DevProject.Domain.Entities;
using DevProject.Domain.Enums;

namespace DevProject.Domain.Tests;

public sealed class ProjectTests
{
    [Fact]
    public void Project_is_created_with_normalized_values_and_planned_status()
    {
        var project = new Project("  DevProject  ", "  Initial description  ");

        Assert.Equal("DevProject", project.Name);
        Assert.Equal("Initial description", project.Description);
        Assert.Equal(ProjectStatus.Planned, project.Status);
        Assert.Empty(project.Features);
    }

    [Fact]
    public void Blank_description_is_normalized_to_null()
    {
        var project = new Project("DevProject", "  ");

        Assert.Null(project.Description);
    }

    [Fact]
    public void Updating_details_trims_values_and_updates_timestamp()
    {
        var project = new Project("Before");

        project.UpdateDetails("  After  ", "  Updated  ");

        Assert.Equal("After", project.Name);
        Assert.Equal("Updated", project.Description);
        Assert.NotNull(project.UpdatedAt);
        Assert.True(project.UpdatedAt >= project.CreatedAt);
    }

    [Fact]
    public void Invalid_details_do_not_change_project()
    {
        var project = new Project("Before", "Description");

        var action = () => project.UpdateDetails(" ", "Changed");

        Assert.Throws<ArgumentException>(action);
        Assert.Equal("Before", project.Name);
        Assert.Equal("Description", project.Description);
        Assert.Null(project.UpdatedAt);
    }

    [Fact]
    public void Invalid_status_does_not_change_project()
    {
        var project = new Project("Project");

        var action = () => project.ChangeStatus((ProjectStatus)999);

        Assert.Throws<ArgumentException>(action);
        Assert.Equal(ProjectStatus.Planned, project.Status);
        Assert.Null(project.UpdatedAt);
    }
}

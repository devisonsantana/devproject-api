using DevProject.Domain.Models;

namespace DevProject.Domain.Tests.Models;

public class ProjectTests
{
    [Fact]
    public void Create_WithValidNameAndObjetive_ShouldCreateProject()
    {
        string name = "My Project";
        string objective = "My Project Objective";

        var project = new Project(name, objective);

        Assert.Equal(name, project.Name);
        Assert.Equal(objective, project.Objective);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrowException(string name)
    {
        var action = () => new Project(name, "My Objective");

        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithInvalidObjective_ShouldThrowException(string objective)
    {
        var action = () => new Project("My Project", objective);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Create_WithNameContainingWhitespace_ShouldTrimName()
    {
        var name = "    My Project    ";

        var project = new Project(name, "My Objective");

        Assert.Equal("My Project", project.Name);
    }

    [Fact]
    public void Create_WithObjectiveContainingWhitespace_ShouldTrimName()
    {
        var objective = "   My Objective   ";

        var project = new Project("My Project", objective);

        Assert.Equal("My Objective", project.Objective);
    }

    [Fact]
    public void Create_ShouldGenerateUniqueId()
    {
        var project1 = new Project("My Project 1", "My Objective 1");
        var project2 = new Project("My Project 2", "My Objective 2");

        Assert.NotEqual(Guid.Empty, project1.Id);
        Assert.NotEqual(Guid.Empty, project2.Id);
        Assert.NotEqual(project1.Id, project2.Id);
    }

    [Fact]
    public void Create_ShouldSetCreationAndUpdateDates()
    {
        var project = new Project("My Project", "My Objective");

        Assert.NotEqual(default, project.CreatedAt);
        Assert.Equal(project.CreatedAt, project.UpdatedAt);
    }

    [Fact]
    public void Create_ShouldSetStatusPlanning()
    {
        var project = new Project("My Project", "My Objective");

        Assert.Equal(ProjectStatus.Planning, project.Status);
    }

    [Fact]
    public void Create_WithPlannedDates_ShouldSetDates()
    {
        var plannedStartDate = new DateOnly(2026, 10, 9);
        var plannedEndDate = new DateOnly(2026, 11, 2);

        var project = new Project("My Project", "My Objective", plannedStartDate: plannedStartDate, plannedEndDate: plannedEndDate);

        Assert.Equal(plannedStartDate, project.PlannedStartDate);
        Assert.Equal(plannedEndDate, project.PlannedEndDate);
    }

    [Fact]
    public void Create_WithEndDateBeforeStartDate_ShouldThrowException()
    {
        var plannedStartDate = new DateOnly(2026, 10, 20);
        var plannedEndDate = new DateOnly(2026, 10, 19);

        var action = () => new Project(
            "My Project", "My Objective", plannedStartDate: plannedStartDate, plannedEndDate: plannedEndDate);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        var project = new Project("My Project", "My Objective");
        var updatedName = "My Updated Project";

        project.UpdateName(updatedName);

        Assert.Equal(updatedName, project.Name);
    }

    [Fact]
    public void UpdateObjective_WithValidObjective_ShouldUpdateObjective()
    {
        var project = new Project("My Project", "My Objective");
        var updatedObjective = "My Updated Objective";

        project.UpdateObjective(updatedObjective);

        Assert.Equal(updatedObjective, project.Objective);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void UpdateName_WithInvalidName_ShouldThrowException(string name)
    {
        var project = new Project("My Project", "My Objective");

        var action = () => project.UpdateName(name);

        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void UpdateObjective_WithInvalidObjective_ShouldThrowException(string objective)
    {
        var project = new Project("My Project", "My Objective");

        var action = () => project.UpdateObjective(objective);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void UpdateName_ShouldUpdateUpdatedAt()
    {
        var project = new Project("My Project", "My Objective");
        var updatedAt = project.UpdatedAt;

        project.UpdateName("My updated project");

        Assert.True(updatedAt <= project.UpdatedAt);
    }

    [Fact]
    public void UpdateObjective_ShouldUpdateUpdatedAt()
    {
        var project = new Project("My Project", "My Objective");
        var updatedAt = project.UpdatedAt;

        project.UpdateObjective("My updated objective");

        Assert.True(updatedAt <= project.UpdatedAt);
    }

    [Fact]
    public void UpdatePlannedDates_WithValidDates_ShouldUpdateDates()
    {
        var project = new Project("My Project", "My Objective");
        var startDate = new DateOnly(2026, 10, 20);
        var endDate = new DateOnly(2026, 10, 20);

        project.UpdatePlannedDates(startDate, endDate);

        Assert.Equal(startDate, project.PlannedStartDate);
        Assert.Equal(endDate, project.PlannedEndDate);

    }

    [Fact]
    public void UpdatePlannedDates_WithInvalidDates_ShouldThrowException()
    {

        var project = new Project("My Project", "My Objective");
        var updatedStartDate = new DateOnly(2026, 10, 20);
        var updatedEndDate = new DateOnly(2026, 10, 19);

        var action = () => project.UpdatePlannedDates(updatedStartDate, updatedEndDate);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void UpdatePlannedDates_WithInvalidDates_ShouldNotChangeExistingDates()
    {
        var originalStartDate = new DateOnly(2026, 10, 20);
        var originalEndDate = new DateOnly(2026, 10, 29);
        var project = new Project(
            "My Project", "My Objective", plannedStartDate: originalStartDate, plannedEndDate: originalEndDate);
        var invalidStart = new DateOnly(2026, 10, 20);
        var invalidEnd = new DateOnly(2026, 10, 15);

        var action = () => project.UpdatePlannedDates(invalidStart, invalidEnd);

        Assert.Throws<ArgumentException>(action);
        Assert.Equal(originalStartDate, project.PlannedStartDate);
        Assert.Equal(originalEndDate, project.PlannedEndDate);
    }
}

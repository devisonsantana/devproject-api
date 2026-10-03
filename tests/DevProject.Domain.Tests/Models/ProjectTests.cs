using DevProject.Domain.Models;

namespace DevProject.Domain.Tests.Models;

public class ProjectTests
{
    #region Create Tests

    #region Id Tests
    [Fact]
    public void Create_ShouldGenerateUniqueId()
    {
        var project1 = new Project("My Project 1", "My Objective 1");
        var project2 = new Project("My Project 2", "My Objective 2");

        Assert.NotEqual(Guid.Empty, project1.Id);
        Assert.NotEqual(Guid.Empty, project2.Id);
        Assert.NotEqual(project1.Id, project2.Id);
    }
    #endregion

    #region Name Tests
    [Fact]
    public void Create_WithValidNameAndObjective_ShouldCreateProject()
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

    [Fact]
    public void Create_WithNameContainingWhitespace_ShouldTrimName()
    {
        var name = "    My Project    ";

        var project = new Project(name, "My Objective");

        Assert.Equal("My Project", project.Name);
    }
    #endregion

    #region Objective Tests
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
    public void Create_WithObjectiveContainingWhitespace_ShouldTrimObjective()
    {
        var objective = "   My Objective   ";

        var project = new Project("My Project", objective);

        Assert.Equal("My Objective", project.Objective);
    }
    #endregion

    #region Motivation Tests
    [Fact]
    public void Create_WithoutMotivation_ShouldCreateProject()
    {
        var project = new Project("My Project", "My Objective");

        Assert.Null(project.Motivation);
    }

    [Fact]
    public void Create_WithMotivation_ShouldSetMotivation()
    {
        var motivation = "My Motivation";
        var project = new Project("My Project", "My Objective", motivation);

        Assert.Equal(motivation, project.Motivation);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithEmptyMotivation_ShouldStoreNull(string motivation)
    {
        var project = new Project("My Project", "My Objective", motivation);

        Assert.Null(project.Motivation);
    }

    [Fact]
    public void Create_WithMotivationContainingWhitespace_ShouldTrimMotivation()
    {
        var motivation = "   My Motivation   ";
        var project = new Project("My Project", "My Objective", motivation);

        Assert.Equal("My Motivation", project.Motivation);
    }
    #endregion

    #region CreatedAt & UpdatedAt Tests
    [Fact]
    public void Create_ShouldSetCreationAndUpdateDates()
    {
        var project = new Project("My Project", "My Objective");

        Assert.NotEqual(default, project.CreatedAt);
        Assert.Equal(project.CreatedAt, project.UpdatedAt);
    }
    #endregion

    #region PlannedDates Tests
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
    public void Create_WithSameStartAndEndDate_ShouldCreateProject()
    {
        var date = new DateOnly(2026, 10, 20);
        var project = new Project(
            "My Project", "My Objective", plannedStartDate: date, plannedEndDate: date);

        Assert.Equal(date, project.PlannedStartDate);
        Assert.Equal(date, project.PlannedEndDate);
    }

    [Fact]
    public void Create_WithOnlyStartDate_ShouldCreateProject()
    {
        var startDate = new DateOnly(2026, 10, 20);

        var project = new Project(
            "My Project",
            "My Objective",
            plannedStartDate: startDate);

        Assert.Equal(startDate, project.PlannedStartDate);
        Assert.Null(project.PlannedEndDate);
    }

    [Fact]
    public void Create_WithOnlyEndDate_ShouldCreateProject()
    {
        var endDate = new DateOnly(2026, 10, 20);

        var project = new Project("My Project", "My Objective", plannedEndDate: endDate);

        Assert.Equal(endDate, project.PlannedEndDate);
        Assert.Null(project.PlannedStartDate);
    }
    #endregion

    #endregion

    #region Update Tests

    #region UpdateName
    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        var project = new Project("My Project", "My Objective");
        var updatedName = "My Updated Project";

        project.UpdateName(updatedName);

        Assert.Equal(updatedName, project.Name);
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

    [Fact]
    public void UpdateName_ShouldUpdateUpdatedAt()
    {
        var project = new Project("My Project", "My Objective");
        var updatedAt = project.UpdatedAt;

        project.UpdateName("My updated project");

        Assert.True(updatedAt < project.UpdatedAt);
    }

    [Fact]
    public void UpdateName_WithInvalidName_ShouldNotUpdateUpdatedAt()
    {
        var project = new Project("My Project", "My Objective");
        var updatedAt = project.UpdatedAt;

        var action = () => project.UpdateName("");

        Assert.Throws<ArgumentException>(action);
        Assert.Equal(updatedAt, project.UpdatedAt);
    }
    #endregion

    #region UpdateObjective
    [Fact]
    public void UpdateObjective_WithValidObjective_ShouldUpdateObjective()
    {
        var project = new Project("My Project", "My Objective");
        var updatedObjective = "My Updated Objective";

        project.UpdateObjective(updatedObjective);

        Assert.Equal(updatedObjective, project.Objective);
    }

    [Fact]
    public void UpdateObjective_ShouldUpdateUpdatedAt()
    {
        var project = new Project("My Project", "My Objective");
        var updatedAt = project.UpdatedAt;

        project.UpdateObjective("My updated objective");

        Assert.True(updatedAt < project.UpdatedAt);
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
    public void UpdateObjective_WithInvalidObjective_ShouldNotUpdateUpdatedAt()
    {
        var project = new Project("My Project", "My Objective");
        var updatedAt = project.UpdatedAt;

        var action = () => project.UpdateObjective("");

        Assert.Throws<ArgumentException>(action);
        Assert.Equal(updatedAt, project.UpdatedAt);
    }
    #endregion

    #region UpdateMotivation
    [Fact]
    public void UpdateMotivation_ShouldUpdateMotivation()
    {
        var project = new Project("My Project", "My Objective");
        var updatedMotivation = "My updated motivation";

        project.UpdateMotivation(updatedMotivation);

        Assert.Equal(updatedMotivation, project.Motivation);
    }

    [Fact]
    public void UpdateMotivation_ShouldUpdateUpdatedAt()
    {
        var project = new Project("My Project", "My Objective");
        var updatedAt = project.UpdatedAt;

        project.UpdateMotivation("My updated motivation");

        Assert.True(updatedAt < project.UpdatedAt);
    }

    [Fact]
    public void UpdateMotivation_WithNull_ShouldRemoveMotivation()
    {
        var project = new Project("My Project", "My Objective", "My Motivation");

        project.UpdateMotivation(null);

        Assert.Null(project.Motivation);
    }
    #endregion

    #region UpdatePlannedDates
    [Fact]
    public void UpdatePlannedDates_WithValidDates_ShouldUpdateDates()
    {
        var project = new Project("My Project", "My Objective");
        var startDate = new DateOnly(2026, 10, 20);
        var endDate = new DateOnly(2026, 10, 30);

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

    [Fact]
    public void UpdatePlannedDates_ShouldUpdateUpdatedAt()
    {
        var project = new Project("My Project", "My Objective");
        var updatedAt = project.UpdatedAt;

        project.UpdatePlannedDates(new DateOnly(2026, 10, 20), new DateOnly(2026, 10, 30));

        Assert.True(updatedAt < project.UpdatedAt);
    }

    [Fact]
    public void UpdatePlannedDates_WithInvalidDates_ShouldNotUpdateUpdatedAt()
    {
        var project = new Project("My Project", "My Objective");
        var updatedAt = project.UpdatedAt;
        var invalidStart = new DateOnly(2026, 10, 20);
        var invalidEnd = new DateOnly(2026, 10, 15);

        var action = () => project.UpdatePlannedDates(invalidStart, invalidEnd);

        Assert.Throws<ArgumentException>(action);
        Assert.Equal(updatedAt, project.UpdatedAt);
    }

    [Fact]
    public void UpdatePlannedDates_WithOnlyStartDate_ShouldUpdateProject()
    {
        var project = new Project("My Project", "My Objective");
        var startDate = new DateOnly(2026, 10, 20);

        project.UpdatePlannedDates(startDate, null);

        Assert.Equal(startDate, project.PlannedStartDate);
        Assert.Null(project.PlannedEndDate);
    }

    [Fact]
    public void UpdatePlannedDates_WithOnlyEndDate_ShouldUpdateProject()
    {
        var project = new Project("My Project", "My Objective");
        var endDate = new DateOnly(2026, 10, 20);

        project.UpdatePlannedDates(null, endDate);

        Assert.Equal(endDate, project.PlannedEndDate);
        Assert.Null(project.PlannedStartDate);
    }
    #endregion

    #endregion
}

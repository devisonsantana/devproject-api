using DevProject.Application.DTOs.Projects;
using DevProject.Application.Interfaces.Mappers;
using DevProject.Application.Mappers;
using DevProject.Domain.Entities;
using DevProject.Domain.Enums;

namespace DevProject.Application.Tests.Mappers;

public sealed class ProjectMapperTests
{
    private readonly IProjectMapper _mapper;

    public ProjectMapperTests()
    {
        _mapper = new ProjectMapper();
    }

    [Fact]
    public void ToEntity_ShouldMapCreateRequestToProject()
    {
        // Arrange
        var request = new CreateProjectRequest(
            "DevProject",
            "Project management API"
        );

        // Act
        var project = _mapper.ToEntity(request);

        // Assert
        Assert.NotNull(project);
        Assert.Equal(request.Name, project.Name);
        Assert.Equal(request.Description, project.Description);
        Assert.Equal(ProjectStatus.Planned, project.Status);
    }

    [Fact]
    public void ToEntity_ShouldAllowNullDescription()
    {
        // Arrange
        var request = new CreateProjectRequest(
            "DevProject",
            null
        );

        // Act
        var project = _mapper.ToEntity(request);

        // Assert
        Assert.Equal("DevProject", project.Name);
        Assert.Null(project.Description);
    }

    [Fact]
    public void ToEntity_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => _mapper.ToEntity(null!)
        );
    }

    [Fact]
    public void ToResponse_ShouldMapProjectToResponse()
    {
        // Arrange
        var project = new Project(
            "DevProject",
            "Project management API"
        );

        // Act
        var response = _mapper.ToResponse(project);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(project.Id, response.Id);
        Assert.Equal(project.Name, response.Name);
        Assert.Equal(project.Description, response.Description);
        Assert.Equal(project.Status, response.Status);
        Assert.Equal(project.CreatedAt, response.CreatedAt);
        Assert.Equal(project.UpdatedAt, response.UpdatedAt);
    }

    [Fact]
    public void ToResponse_ShouldThrowArgumentNullException_WhenProjectIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => _mapper.ToResponse(null!)
        );
    }

    [Fact]
    public void UpdateEntity_ShouldUpdateProjectDetails()
    {
        // Arrange
        var project = new Project(
            "Old Project",
            "Old description"
        );

        var request = new UpdateProjectRequest(
            "Updated Project",
            "Updated description"
        );

        // Act
        _mapper.UpdateEntity(project, request);

        // Assert
        Assert.Equal("Updated Project", project.Name);
        Assert.Equal("Updated description", project.Description);
    }

    [Fact]
    public void UpdateEntity_ShouldAllowNullDescription()
    {
        // Arrange
        var project = new Project(
            "Old Project",
            "Old description"
        );

        var request = new UpdateProjectRequest(
            "Updated Project",
            null
        );

        // Act
        _mapper.UpdateEntity(project, request);

        // Assert
        Assert.Equal("Updated Project", project.Name);
        Assert.Null(project.Description);
    }

    [Fact]
    public void UpdateEntity_ShouldThrowArgumentNullException_WhenProjectIsNull()
    {
        // Arrange
        var request = new UpdateProjectRequest(
            "Updated Project",
            "Updated description"
        );

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => _mapper.UpdateEntity(null!, request)
        );
    }

    [Fact]
    public void UpdateEntity_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        var project = new Project("DevProject");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => _mapper.UpdateEntity(project, null!)
        );
    }

    [Fact]
    public void UpdateEntity_ShouldPreserveExistingStatus()
    {
        // Arrange
        var project = new Project("DevProject");

        var request = new UpdateProjectRequest(
            "Updated Project",
            "Updated description"
        );

        var originalStatus = project.Status;

        // Act
        _mapper.UpdateEntity(project, request);

        // Assert
        Assert.Equal(originalStatus, project.Status);
    }

    [Fact]
    public void UpdateEntity_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var project = new Project("DevProject");

        var request = new UpdateProjectRequest(
            "   ",
            "Updated description"
        );

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _mapper.UpdateEntity(project, request));
        Assert.Equal("DevProject", project.Name);
    }
}
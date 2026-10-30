using DevProject.Application.DTOs.Projects;
using DevProject.Domain.Enums;

namespace DevProject.Application.Tests.DTOs.Projects;

public sealed class ProjectResponseTests
{
    [Fact]
    public void Constructor_ShouldAssignProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        const string name = "DevProject";
        const string description = "Project management API";
        var status = ProjectStatus.Planned;
        var createdAt = DateTimeOffset.UtcNow;
        var updatedAt = DateTimeOffset.UtcNow;

        // Act
        var response = new ProjectResponse(
            id,
            name,
            description,
            status,
            createdAt,
            updatedAt
        );

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal(name, response.Name);
        Assert.Equal(description, response.Description);
        Assert.Equal(status, response.Status);
        Assert.Equal(createdAt, response.CreatedAt);
        Assert.Equal(updatedAt, response.UpdatedAt);
    }

    [Fact]
    public void Constructor_ShouldAllowNullOptionalProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;

        // Act
        var response = new ProjectResponse(
            id,
            "DevProject",
            null,
            ProjectStatus.Planned,
            createdAt,
            null
        );

        // Assert
        Assert.Null(response.Description);
        Assert.Null(response.UpdatedAt);
    }
}
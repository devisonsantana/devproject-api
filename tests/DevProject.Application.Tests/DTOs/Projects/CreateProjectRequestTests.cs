using DevProject.Application.DTOs.Projects;

namespace DevProject.Application.Tests.DTOs.Projects;

public sealed class CreateProjectRequestTests
{
    [Fact]
    public void Constructor_ShouldAssignProperties()
    {
        // Arrange
        const string name = "DevProject";
        const string description = "Project management API";

        // Act
        var request = new CreateProjectRequest(name, description);

        // Assert
        Assert.Equal(name, request.Name);
        Assert.Equal(description, request.Description);
    }

    [Fact]
    public void Constructor_ShouldAllowNullDescription()
    {
        // Act
        var request = new CreateProjectRequest("DevProject", null);

        // Assert
        Assert.Equal("DevProject", request.Name);
        Assert.Null(request.Description);
    }
}
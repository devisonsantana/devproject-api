using DevProject.Application.DTOs.Projects;

namespace DevProject.Application.Tests.DTOs.Projects;

public sealed class UpdateProjectRequestTests
{
    [Fact]
    public void Constructor_ShouldAssignProperties()
    {
        // Arrange
        const string name = "Updated Project";
        const string description = "Updated description";

        // Act
        var request = new UpdateProjectRequest(name, description);

        // Assert
        Assert.Equal(name, request.Name);
        Assert.Equal(description, request.Description);
    }

    [Fact]
    public void Constructor_ShouldAllowNullDescription()
    {
        // Act
        var request = new UpdateProjectRequest("Updated Project", null);

        // Assert
        Assert.Equal("Updated Project", request.Name);
        Assert.Null(request.Description);
    }
}
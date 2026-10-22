using DevProject.Domain.Entities;

namespace DevProject.Domain.Tests;

public sealed class EntityTests
{
    [Fact]
    public void New_entity_has_an_id_and_utc_creation_time()
    {
        var beforeCreation = DateTimeOffset.UtcNow;
        var project = new Project("Project");

        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal(TimeSpan.Zero, project.CreatedAt.Offset);
        Assert.InRange(project.CreatedAt, beforeCreation, DateTimeOffset.UtcNow);
        Assert.Null(project.UpdatedAt);
    }
}

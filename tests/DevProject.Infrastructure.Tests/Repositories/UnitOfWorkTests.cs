using DevProject.Domain.Entities;
using DevProject.Infrastructure.Context;
using DevProject.Infrastructure.Persistence.Repositories;
using DevProject.Infrastructure.Tests.Fixtures;

namespace DevProject.Infrastructure.Tests.Repositories;

public sealed class UnitOfWorkTests(DatabaseFixture database) : RepositoryTestBase(database)
{
    [Fact]
    public async Task SaveChangesAsync_PersistsChangesFromMultipleRepositories()
    {
        var project = new Project("Project");
        var feature = new Feature(project.Id, "Feature");
        var task = new ProjectTask(feature.Id, "Task");
        var user = new User("User", "user@example.com", "password-hash");

        await using (var context = CreateContext())
        {
            context.Projects.Add(project);
            new FeatureRepository(context).Add(feature);
            new ProjectTaskRepository(context).Add(task);
            new UserRepository(context).Add(user);

            await new UnitOfWork(context).SaveChangesAsync();
        }

        await using var assertContext = CreateContext();
        Assert.Equal(1, await new ProjectRepository(assertContext).GetCountAsync());
        Assert.Equal(1, await new FeatureRepository(assertContext).GetCountAsync());
        Assert.Equal(1, await new ProjectTaskRepository(assertContext).GetCountAsync());
        Assert.Equal(1, await new UserRepository(assertContext).GetCountAsync());
    }

    [Fact]
    public async Task SaveChangesAsync_WhenThereAreNoTrackedChanges_Completes()
    {
        await using var context = CreateContext();

        await new UnitOfWork(context).SaveChangesAsync();
    }
}

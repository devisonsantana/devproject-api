using DevProject.Domain.Entities;
using DevProject.Domain.Enums;
using DevProject.Infrastructure.Context;
using DevProject.Infrastructure.Persistence.Repositories;
using DevProject.Infrastructure.Tests.Fixtures;

namespace DevProject.Infrastructure.Tests.Repositories;

public sealed class ProjectTaskRepositoryTests(DatabaseFixture database) : RepositoryTestBase(database)
{
    private static ProjectTaskRepository CreateTaskRepository(AppDbContext context) => new(context);

    private static UnitOfWork CreateUnitOfWork(AppDbContext context) => new(context);

    private async Task SeedAsync(Project project, Feature feature, params ProjectTask[] tasks)
    {
        await using var context = CreateContext();
        context.Projects.Add(project);
        context.Features.Add(feature);

        var repository = CreateTaskRepository(context);
        foreach (var task in tasks)
        {
            repository.Add(task);
        }

        await CreateUnitOfWork(context).SaveChangesAsync();
    }

    [Fact]
    public async Task Add_AfterSaveChanges_PersistsTask()
    {
        var project = new Project("Project");
        var feature = new Feature(project.Id, "Feature");
        var dueDate = new DateTimeOffset(2026, 12, 31, 12, 0, 0, TimeSpan.Zero);
        var task = new ProjectTask(
            feature.Id,
            "  Write integration tests  ",
            "  Cover persistence  ",
            TaskPriority.High,
            dueDate,
            4);

        await SeedAsync(project, feature, task);

        await using var context = CreateContext();
        var saved = await CreateTaskRepository(context).GetByIdAsync(task.Id);

        Assert.NotNull(saved);
        Assert.Equal(feature.Id, saved.FeatureId);
        Assert.Equal("Write integration tests", saved.Title);
        Assert.Equal("Cover persistence", saved.Description);
        Assert.Equal(ProjectTaskStatus.Todo, saved.Status);
        Assert.Equal(TaskPriority.High, saved.Priority);
        Assert.Equal(dueDate, saved.DueDate);
        Assert.Equal(4, saved.Order);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        await using var context = CreateContext();

        var result = await CreateTaskRepository(context).GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_AndGetCountAsync_ReturnPersistedTasks()
    {
        var project = new Project("Project");
        var feature = new Feature(project.Id, "Feature");
        await SeedAsync(
            project,
            feature,
            new ProjectTask(feature.Id, "One"),
            new ProjectTask(feature.Id, "Two"));

        await using var context = CreateContext();
        var repository = CreateTaskRepository(context);
        var tasks = (await repository.GetAllAsync()).ToList();
        var count = await repository.GetCountAsync();

        Assert.Equal(2, tasks.Count);
        Assert.Contains(tasks, task => task.Title == "One");
        Assert.Contains(tasks, task => task.Title == "Two");
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task Update_PersistsChangesMadeToTask()
    {
        var project = new Project("Project");
        var feature = new Feature(project.Id, "Feature");
        var task = new ProjectTask(feature.Id, "Old title");
        await SeedAsync(project, feature, task);

        await using (var context = CreateContext())
        {
            var repository = CreateTaskRepository(context);
            var loaded = await repository.GetByIdAsync(task.Id);

            loaded!.UpdateDetails(
                "New title",
                "Updated description",
                TaskPriority.Critical,
                new DateTimeOffset(2027, 1, 15, 9, 30, 0, TimeSpan.Zero));
            loaded.ChangeStatus(ProjectTaskStatus.Done);
            loaded.ChangeOrder(5);
            repository.Update(loaded);
            await CreateUnitOfWork(context).SaveChangesAsync();
        }

        await using var assertContext = CreateContext();
        var saved = await CreateTaskRepository(assertContext).GetByIdAsync(task.Id);

        Assert.NotNull(saved);
        Assert.Equal("New title", saved.Title);
        Assert.Equal("Updated description", saved.Description);
        Assert.Equal(TaskPriority.Critical, saved.Priority);
        Assert.Equal(ProjectTaskStatus.Done, saved.Status);
        Assert.NotNull(saved.CompletedAt);
        Assert.Equal(5, saved.Order);
        Assert.NotNull(saved.UpdatedAt);
    }

    [Fact]
    public async Task Remove_DeletesTaskFromDatabase()
    {
        var project = new Project("Project");
        var feature = new Feature(project.Id, "Feature");
        var task = new ProjectTask(feature.Id, "To delete");
        await SeedAsync(project, feature, task);

        await using (var context = CreateContext())
        {
            var repository = CreateTaskRepository(context);
            var loaded = await repository.GetByIdAsync(task.Id);

            repository.Remove(loaded!);
            await CreateUnitOfWork(context).SaveChangesAsync();
        }

        await using var assertContext = CreateContext();
        var result = await CreateTaskRepository(assertContext).GetByIdAsync(task.Id);

        Assert.Null(result);
    }
}

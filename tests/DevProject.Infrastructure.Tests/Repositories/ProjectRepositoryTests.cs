using DevProject.Domain.Entities;
using DevProject.Domain.Enums;
using DevProject.Infrastructure.Context;
using DevProject.Infrastructure.Persistence.Repositories;
using DevProject.Infrastructure.Tests.Fixtures;

namespace DevProject.Infrastructure.Tests.Repositories;

public sealed class ProjectRepositoryTests(DatabaseFixture database) : RepositoryTestBase(database)
{
    private static ProjectRepository CreateProjectRepository(AppDbContext context) => new(context);

    private static UnitOfWork CreateUnitOfWork(AppDbContext context) => new(context);

    private async Task SeedAsync(params Project[] projects)
    {
        await using var context = CreateContext();
        var repository = CreateProjectRepository(context);

        foreach (var project in projects)
        {
            repository.Add(project);
        }

        await CreateUnitOfWork(context).SaveChangesAsync();
    }

    [Fact]
    public async Task Add_AfterSaveChanges_PersistsProject()
    {
        // Arrange
        var project = new Project("  My project  ", "A description");

        // Act
        await using (var context = CreateContext())
        {
            CreateProjectRepository(context).Add(project);
            await CreateUnitOfWork(context).SaveChangesAsync();
        }

        // Assert (contexto novo = leitura real do banco)
        await using var assertContext = CreateContext();
        var saved = await CreateProjectRepository(assertContext).GetByIdAsync(project.Id);

        Assert.NotNull(saved);
        Assert.Equal("My project", saved.Name);
        Assert.Equal("A description", saved.Description);
        Assert.Equal(ProjectStatus.Planned, saved.Status);
        Assert.Equal(project.CreatedAt, saved.CreatedAt, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task Add_WithoutSaveChanges_DoesNotPersist()
    {
        // Arrange
        var project = new Project("Not saved");

        // Act
        await using (var context = CreateContext())
        {
            CreateProjectRepository(context).Add(project);
            // sem SaveChangesAsync de propósito
        }

        // Assert
        await using var assertContext = CreateContext();
        var count = await CreateProjectRepository(assertContext).GetCountAsync();

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        await using var context = CreateContext();

        var result = await CreateProjectRepository(context).GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }
    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsProject()
    {
        var project = new Project("Existing");
        await SeedAsync(project);

        await using var context = CreateContext();
        var result = await CreateProjectRepository(context).GetByIdAsync(project.Id);

        Assert.NotNull(result);
        Assert.Equal(project.Id, result.Id);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPersistedProjects()
    {
        await SeedAsync(new Project("One"), new Project("Two"));

        await using var context = CreateContext();
        var all = (await CreateProjectRepository(context).GetAllAsync()).ToList();

        Assert.Equal(2, all.Count);
        Assert.Contains(all, p => p.Name == "One");
        Assert.Contains(all, p => p.Name == "Two");
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyCollection()
    {
        await using var context = CreateContext();

        var all = await CreateProjectRepository(context).GetAllAsync();

        Assert.Empty(all);
    }

    [Fact]
    public async Task GetFilteredAsync_FiltersNameCaseInsensitivelyAndByStatus()
    {
        var matchingProject = new Project("Alpha Platform");
        matchingProject.ChangeStatus(ProjectStatus.Active);
        var wrongStatus = new Project("Alpha archive");
        var wrongName = new Project("Beta");
        wrongName.ChangeStatus(ProjectStatus.Active);
        await SeedAsync(matchingProject, wrongStatus, wrongName);

        await using var context = CreateContext();
        var result = (await CreateProjectRepository(context).GetFilteredAsync(
            "PLATFORM",
            ProjectStatus.Active)).ToArray();

        var project = Assert.Single(result);
        Assert.Equal(matchingProject.Id, project.Id);
    }

    [Fact]
    public async Task GetCountAsync_ReturnsNumberOfRows()
    {
        await SeedAsync(new Project("One"), new Project("Two"), new Project("Three"));

        await using var context = CreateContext();
        var count = await CreateProjectRepository(context).GetCountAsync();

        Assert.Equal(3, count);
    }

    [Fact]
    public async Task Update_PersistsChangesMadeToProject()
    {
        // Arrange
        var project = new Project("Old name");
        await SeedAsync(project);

        // Act: carrega, altera, atualiza e salva em um contexto
        await using (var context = CreateContext())
        {
            var repository = CreateProjectRepository(context);
            var loaded = await repository.GetByIdAsync(project.Id);

            loaded!.UpdateDetails("New name", "New description");
            loaded.ChangeStatus(ProjectStatus.OnHold);
            repository.Update(loaded);
            await CreateUnitOfWork(context).SaveChangesAsync();
        }

        // Assert
        await using var assertContext = CreateContext();
        var saved = await CreateProjectRepository(assertContext).GetByIdAsync(project.Id);

        Assert.NotNull(saved);
        Assert.Equal("New name", saved.Name);
        Assert.Equal("New description", saved.Description);
        Assert.Equal(ProjectStatus.OnHold, saved.Status);
        Assert.NotNull(saved.UpdatedAt);
    }

    [Fact]
    public async Task Remove_DeletesProjectFromDatabase()
    {
        // Arrange
        var project = new Project("To delete");
        await SeedAsync(project);

        // Act
        await using (var context = CreateContext())
        {
            var repository = CreateProjectRepository(context);
            var loaded = await repository.GetByIdAsync(project.Id);

            repository.Remove(loaded!);
            await CreateUnitOfWork(context).SaveChangesAsync();
        }

        // Assert
        await using var assertContext = CreateContext();
        var result = await CreateProjectRepository(assertContext).GetByIdAsync(project.Id);

        Assert.Null(result);
    }
}

using DevProject.Domain.Entities;
using DevProject.Domain.Enums;
using DevProject.Infrastructure.Context;
using DevProject.Infrastructure.Persistence.Repositories;
using DevProject.Infrastructure.Tests.Fixtures;

namespace DevProject.Infrastructure.Tests.Repositories;

public sealed class FeatureRepositoryTests(DatabaseFixture database) : RepositoryTestBase(database)
{
    private static FeatureRepository CreateFeatureRepository(AppDbContext context) => new(context);

    private static UnitOfWork CreateUnitOfWork(AppDbContext context) => new(context);

    private async Task SeedAsync(Project project, params Feature[] features)
    {
        await using var context = CreateContext();
        context.Projects.Add(project);

        var repository = CreateFeatureRepository(context);
        foreach (var feature in features)
        {
            repository.Add(feature);
        }

        await CreateUnitOfWork(context).SaveChangesAsync();
    }

    [Fact]
    public async Task Add_AfterSaveChanges_PersistsFeature()
    {
        var project = new Project("Project");
        var feature = new Feature(project.Id, "  Authentication  ", "  User sign-in  ", 2);

        await SeedAsync(project, feature);

        await using var context = CreateContext();
        var saved = await CreateFeatureRepository(context).GetByIdAsync(feature.Id);

        Assert.NotNull(saved);
        Assert.Equal(project.Id, saved.ProjectId);
        Assert.Equal("Authentication", saved.Name);
        Assert.Equal("User sign-in", saved.Description);
        Assert.Equal(FeatureStatus.Planned, saved.Status);
        Assert.Equal(2, saved.Order);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        await using var context = CreateContext();

        var result = await CreateFeatureRepository(context).GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_AndGetCountAsync_ReturnPersistedFeatures()
    {
        var project = new Project("Project");
        await SeedAsync(
            project,
            new Feature(project.Id, "One"),
            new Feature(project.Id, "Two"));

        await using var context = CreateContext();
        var repository = CreateFeatureRepository(context);
        var features = (await repository.GetAllAsync()).ToList();
        var count = await repository.GetCountAsync();

        Assert.Equal(2, features.Count);
        Assert.Contains(features, feature => feature.Name == "One");
        Assert.Contains(features, feature => feature.Name == "Two");
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetFilteredAsync_FiltersByProjectNameAndStatus()
    {
        var project = new Project("Project");
        var otherProject = new Project("Other project");
        var matchingFeature = new Feature(project.Id, "Authentication");
        matchingFeature.ChangeStatus(FeatureStatus.InProgress);
        var wrongStatus = new Feature(project.Id, "Auth archive");
        var wrongProject = new Feature(otherProject.Id, "Authentication");
        wrongProject.ChangeStatus(FeatureStatus.InProgress);
        await SeedAsync(project, matchingFeature, wrongStatus);
        await SeedAsync(otherProject, wrongProject);

        await using var context = CreateContext();
        var result = (await CreateFeatureRepository(context).GetFilteredAsync(
            project.Id,
            "AUTH",
            FeatureStatus.InProgress)).ToArray();

        var feature = Assert.Single(result);
        Assert.Equal(matchingFeature.Id, feature.Id);
    }

    [Fact]
    public async Task Update_PersistsChangesMadeToFeature()
    {
        var project = new Project("Project");
        var feature = new Feature(project.Id, "Old name");
        await SeedAsync(project, feature);

        await using (var context = CreateContext())
        {
            var repository = CreateFeatureRepository(context);
            var loaded = await repository.GetByIdAsync(feature.Id);

            loaded!.UpdateDetails("New name", "Updated description");
            loaded.ChangeStatus(FeatureStatus.InProgress);
            loaded.ChangeOrder(3);
            repository.Update(loaded);
            await CreateUnitOfWork(context).SaveChangesAsync();
        }

        await using var assertContext = CreateContext();
        var saved = await CreateFeatureRepository(assertContext).GetByIdAsync(feature.Id);

        Assert.NotNull(saved);
        Assert.Equal("New name", saved.Name);
        Assert.Equal("Updated description", saved.Description);
        Assert.Equal(FeatureStatus.InProgress, saved.Status);
        Assert.Equal(3, saved.Order);
        Assert.NotNull(saved.UpdatedAt);
    }

    [Fact]
    public async Task Remove_DeletesFeatureFromDatabase()
    {
        var project = new Project("Project");
        var feature = new Feature(project.Id, "To delete");
        await SeedAsync(project, feature);

        await using (var context = CreateContext())
        {
            var repository = CreateFeatureRepository(context);
            var loaded = await repository.GetByIdAsync(feature.Id);

            repository.Remove(loaded!);
            await CreateUnitOfWork(context).SaveChangesAsync();
        }

        await using var assertContext = CreateContext();
        var result = await CreateFeatureRepository(assertContext).GetByIdAsync(feature.Id);

        Assert.Null(result);
    }
}

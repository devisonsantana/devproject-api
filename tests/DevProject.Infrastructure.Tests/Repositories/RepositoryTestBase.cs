using DevProject.Infrastructure.Context;
using DevProject.Infrastructure.Tests.Fixtures;

namespace DevProject.Infrastructure.Tests.Repositories;

[Collection("Database Collection")]
public abstract class RepositoryTestBase(DatabaseFixture database) : IAsyncLifetime
{
    protected AppDbContext CreateContext() => database.CreateContext();

    public async Task InitializeAsync() => await database.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}

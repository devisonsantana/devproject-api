using DevProject.Domain.Entities;
using DevProject.Infrastructure.Context;
using DevProject.Infrastructure.Persistence.Repositories;
using DevProject.Infrastructure.Tests.Fixtures;

namespace DevProject.Infrastructure.Tests.Repositories;

public sealed class UserRepositoryTests(DatabaseFixture database) : RepositoryTestBase(database)
{
    private static UserRepository CreateUserRepository(AppDbContext context) => new(context);

    private static UnitOfWork CreateUnitOfWork(AppDbContext context) => new(context);

    private async Task SeedAsync(params User[] users)
    {
        await using var context = CreateContext();
        var repository = CreateUserRepository(context);

        foreach (var user in users)
        {
            repository.Add(user);
        }

        await CreateUnitOfWork(context).SaveChangesAsync();
    }

    [Fact]
    public async Task Add_AfterSaveChanges_PersistsUser()
    {
        var user = new User("  Ada Lovelace  ", " ada@example.com ", " password-hash ");

        await using (var context = CreateContext())
        {
            CreateUserRepository(context).Add(user);
            await CreateUnitOfWork(context).SaveChangesAsync();
        }

        await using var assertContext = CreateContext();
        var saved = await CreateUserRepository(assertContext).GetByIdAsync(user.Id);

        Assert.NotNull(saved);
        Assert.Equal("Ada Lovelace", saved.Name);
        Assert.Equal("ada@example.com", saved.Email);
        Assert.Equal("ADA@EXAMPLE.COM", saved.NormalizedEmail);
        Assert.Equal("password-hash", saved.PasswordHash);
        Assert.True(saved.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        await using var context = CreateContext();

        var result = await CreateUserRepository(context).GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_AndGetCountAsync_ReturnPersistedUsers()
    {
        await SeedAsync(
            new User("Ada", "ada@example.com", "hash-1"),
            new User("Grace", "grace@example.com", "hash-2"));

        await using var context = CreateContext();
        var repository = CreateUserRepository(context);
        var users = (await repository.GetAllAsync()).ToList();
        var count = await repository.GetCountAsync();

        Assert.Equal(2, users.Count);
        Assert.Contains(users, user => user.Email == "ada@example.com");
        Assert.Contains(users, user => user.Email == "grace@example.com");
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task Update_PersistsChangesMadeToUser()
    {
        var user = new User("Old name", "old@example.com", "old-hash");
        await SeedAsync(user);

        await using (var context = CreateContext())
        {
            var repository = CreateUserRepository(context);
            var loaded = await repository.GetByIdAsync(user.Id);

            loaded!.UpdateProfile("New name", "new@example.com");
            loaded.UpdatePasswordHash("new-hash");
            repository.Update(loaded);
            await CreateUnitOfWork(context).SaveChangesAsync();
        }

        await using var assertContext = CreateContext();
        var saved = await CreateUserRepository(assertContext).GetByIdAsync(user.Id);

        Assert.NotNull(saved);
        Assert.Equal("New name", saved.Name);
        Assert.Equal("new@example.com", saved.Email);
        Assert.Equal("NEW@EXAMPLE.COM", saved.NormalizedEmail);
        Assert.Equal("new-hash", saved.PasswordHash);
        Assert.NotNull(saved.UpdatedAt);
    }

    [Fact]
    public async Task Remove_DeletesUserFromDatabase()
    {
        var user = new User("To delete", "delete@example.com", "hash");
        await SeedAsync(user);

        await using (var context = CreateContext())
        {
            var repository = CreateUserRepository(context);
            var loaded = await repository.GetByIdAsync(user.Id);

            repository.Remove(loaded!);
            await CreateUnitOfWork(context).SaveChangesAsync();
        }

        await using var assertContext = CreateContext();
        var result = await CreateUserRepository(assertContext).GetByIdAsync(user.Id);

        Assert.Null(result);
    }
}

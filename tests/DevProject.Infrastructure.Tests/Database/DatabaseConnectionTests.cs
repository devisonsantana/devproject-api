using DevProject.Infrastructure.Tests.Fixtures;

namespace DevProject.Infrastructure.Tests.Database;

[Collection("Database Collection")]
public sealed class DatabaseConnectionTests(DatabaseFixture database)
{
    [Fact]
    public async Task Should_connect_to_test_database()
    {
        await using var context = database.CreateContext();
        var canConnect = await context.Database.CanConnectAsync();

        Assert.True(canConnect);
    }
}

using DevProject.Domain.Entities;

namespace DevProject.Domain.Tests;

public sealed class UserTests
{
    [Fact]
    public void User_stores_profile_and_normalized_email_but_not_plain_password()
    {
        var user = new User("  Dev User ", " User@Example.com ", "secure-hash");

        Assert.Equal("Dev User", user.Name);
        Assert.Equal("User@Example.com", user.Email);
        Assert.Equal("USER@EXAMPLE.COM", user.NormalizedEmail);
        Assert.Equal("secure-hash", user.PasswordHash);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Invalid_profile_does_not_mutate_user()
    {
        var user = new User("Dev User", "user@example.com", "hash");

        var action = () => user.UpdateProfile("New Name", " ");

        Assert.Throws<ArgumentException>(action);
        Assert.Equal("Dev User", user.Name);
        Assert.Equal("user@example.com", user.Email);
    }

    [Fact]
    public void Deactivating_user_changes_active_state()
    {
        var user = new User("Dev User", "user@example.com", "hash");

        user.Deactivate();

        Assert.False(user.IsActive);
        Assert.NotNull(user.UpdatedAt);
    }
}

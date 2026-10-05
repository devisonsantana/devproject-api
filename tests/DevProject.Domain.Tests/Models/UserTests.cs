using DevProject.Domain.Models;

namespace DevProject.Domain.Tests.Models;

public class UserTests
{
    #region Create Tests

    #region Id Tests
    [Fact]
    public void Create_ShouldGenerateUniqueId()
    {
        var user1 = new User("User name 1", "user1@email.com", "strongpassword123");
        var user2 = new User("User name 2", "user2@email.com", "strongpassword123");

        Assert.NotEqual(Guid.Empty, user1.Id);
        Assert.NotEqual(Guid.Empty, user2.Id);
        Assert.NotEqual(user1.Id, user2.Id);
    }
    #endregion

    #region Name Tests
    [Fact]
    public void Create_WithValidNameEmailPassword_ShouldSetNameEmailPassword()
    {
        var name = "User name";
        var email = "user@email.com";
        var passwordHash = "strongpassword123";

        var user = new User(name, email, passwordHash);

        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
        Assert.Equal(passwordHash, user.PasswordHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ShouldThrowException(string? name)
    {
        var action = () => new User(name!, "user@email.com", "strongpassword");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Create_WithNameContainingWhitespace_ShouldTrimName()
    {
        var name = "    User name    ";

        var user = new User(name, "user@email.com", "strongpassword");

        Assert.Equal("User name", user.Name);
    }
    #endregion

    #region Email Tests
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidEmail_ShouldThrowException(string? email)
    {
        var action = () => new User("User name", email!, "strongpassword");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Create_WithEmailContainingWhitespace_ShouldTrimEmail()
    {
        var email = "    user@email.com    ";

        var user = new User("User name", email, "strongpassword");

        Assert.Equal("user@email.com", user.Email);
    }
    #endregion

    #region Password Tests
    [Fact]
    public void Create_WithPasswordContainingWhitespace_ShouldTrimPassword()
    {
        var password = "    strongpassword    ";

        var user = new User("User name", "user@email.com", password);

        Assert.Equal("strongpassword", user.PasswordHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidPassword_ShouldThrowException(string? password)
    {
        var action = () => new User("User name", "user@email.com", password!);

        Assert.Throws<ArgumentException>(action);
    }
    #endregion

    #region CreatedAt & UpdatedAt Tests
    [Fact]
    public void Create_ShouldSetCreationAndUpdateDates()
    {
        var user = new User("User name", "user@email.com", "strongpassword");

        Assert.NotEqual(default, user.CreatedAt);
        Assert.NotEqual(default, user.UpdatedAt);
        Assert.Equal(user.CreatedAt, user.UpdatedAt);
    }
    #endregion

    #endregion

    #region Update Tests

    #region UpdateName 
    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        var user = new User("User name", "user@email.com", "strongpassword");
        var updatedName = "User Updated";

        user.UpdateName(updatedName);

        Assert.Equal(updatedName, user.Name);
    }

    [Fact]
    public void UpdateName_WithContainingWhitespace_ShouldTrim()
    {
        var user = new User("User name", "user@email.com", "strongpassword");

        user.UpdateName("   User Updated    ");

        Assert.Equal("User Updated", user.Name);
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateUpdatedAt()
    {
        var user = new User("User name", "user@email.com", "strongpassword");
        var updatedAt = user.UpdatedAt;

        user.UpdateName("User Updated");

        Assert.NotEqual(updatedAt, user.UpdatedAt);
        Assert.True(updatedAt < user.UpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateName_WithInvalidName_ShouldThrowException(string? name)
    {
        var user = new User("User name", "user@email.com", "strongpassword");

        var action = () => user.UpdateName(name!);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void UpdateName_WithInvalidName_ShouldNotUpdateUpdatedAt()
    {
        var user = new User("User name", "user@email.com", "strongpassword");
        var updatedAt = user.UpdatedAt;

        var action = () => user.UpdateName("");

        Assert.Throws<ArgumentException>(action);
        Assert.Equal(updatedAt, user.UpdatedAt);
    }

    [Fact]
    public void UpdateName_WithInvalidName_ShouldNotUpdateName()
    {
        var name = "User name";
        var user = new User(name, "user@email.com", "strongpassword");

        var action = () => user.UpdateName("");

        Assert.Throws<ArgumentException>(action);
        Assert.Equal(name, user.Name);
    }
    #endregion

    #region UpdateEmail
    [Fact]
    public void UpdateEmail_WithValidEmail_ShouldUpdateEmail()
    {
        var user = new User("User name", "user@email.com", "strongpassword");
        var updatedEmail = "updatedemail@email.com";

        user.UpdateEmail(updatedEmail);

        Assert.Equal(updatedEmail, user.Email);
    }

    [Fact]
    public void UpdateEmail_WithContainingWhitespace_ShouldTrim()
    {
        var user = new User("User name", "user@email.com", "strongpassword");

        user.UpdateEmail("   updatedemail@email.com    ");

        Assert.Equal("updatedemail@email.com", user.Email);
    }

    [Fact]
    public void UpdateEmail_WithValidEmail_ShouldUpdateUpdatedAt()
    {
        var user = new User("User name", "user@email.com", "strongpassword");
        var updatedAt = user.UpdatedAt;

        user.UpdateEmail("updatedemail@email.com");

        Assert.NotEqual(updatedAt, user.UpdatedAt);
        Assert.True(updatedAt < user.UpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateEmail_WithInvalidEmail_ShouldThrowException(string? email)
    {
        var user = new User("User name", "user@email.com", "strongpassword");

        var action = () => user.UpdateEmail(email!);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void UpdateEmail_WithInvalidEmail_ShouldNotUpdateUpdatedAt()
    {
        var user = new User("User name", "user@email.com", "strongpassword");
        var updatedAt = user.UpdatedAt;

        var action = () => user.UpdateEmail("");

        Assert.Throws<ArgumentException>(action);
        Assert.Equal(updatedAt, user.UpdatedAt);
    }

    [Fact]
    public void UpdateEmail_WithInvalidEmail_ShouldNotUpdateEmail()
    {
        var email = "user@email.com";
        var user = new User("User name", email, "strongpassword");

        var action = () => user.UpdateEmail("");

        Assert.Throws<ArgumentException>(action);
        Assert.Equal(email, user.Email);
    }
    #endregion

    #region UpdatePassword
    [Fact]
    public void UpdatePassword_WithValidPassword_ShouldUpdatePassword()
    {
        var user = new User("User name", "user@email.com", "strongpassword");
        var updatedPassword = "V3ryStR@ng_PAsSw0Rd!349";

        user.UpdatePasswordHash(updatedPassword);

        Assert.Equal(updatedPassword, user.PasswordHash);
    }

    [Fact]
    public void UpdatePassword_WithContainingWhitespace_ShouldTrim()
    {
        var user = new User("User name", "user@email.com", "strongpassword");

        user.UpdatePasswordHash("   V3ryStR@ng_PAsSw0Rd!349    ");

        Assert.Equal("V3ryStR@ng_PAsSw0Rd!349", user.PasswordHash);
    }

    [Fact]
    public void UpdatePassword_WithValidPassword_ShouldUpdateUpdatedAt()
    {
        var user = new User("User name", "user@email.com", "strongpassword");
        var updatedAt = user.UpdatedAt;

        user.UpdatePasswordHash("V3ryStR@ng_PAsSw0Rd!349");

        Assert.NotEqual(updatedAt, user.UpdatedAt);
        Assert.True(updatedAt < user.UpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdatePassword_WithInvalidPassword_ShouldThrowException(string? password)
    {
        var user = new User("User name", "user@email.com", "strongpassword");

        var action = () => user.UpdatePasswordHash(password!);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void UpdatePassword_WithInvalidPassword_ShouldNotUpdateUpdatedAt()
    {
        var user = new User("User name", "user@email.com", "strongpassword");
        var updatedAt = user.UpdatedAt;

        var action = () => user.UpdatePasswordHash("");

        Assert.Throws<ArgumentException>(action);
        Assert.Equal(updatedAt, user.UpdatedAt);
    }
    [Fact]
    public void UpdatePassword_WithInvalidPassword_ShouldNotUpdatePassword()
    {
        var passwordHash = "strongpassword";
        var user = new User("User name", "user@email.com", passwordHash);

        var action = () => user.UpdatePasswordHash("");

        Assert.Throws<ArgumentException>(action);
        Assert.Equal(passwordHash, user.PasswordHash);
    }
    #endregion

    #endregion
}

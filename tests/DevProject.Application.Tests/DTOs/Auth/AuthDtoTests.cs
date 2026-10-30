using DevProject.Application.DTOs.Auth;

namespace DevProject.Application.Tests.DTOs.Auth;

public sealed class AuthDtoTests
{
    [Fact]
    public void RegisterRequest_preserves_credentials_and_profile_values()
    {
        var request = new RegisterRequest("Dev User", "user@example.com", "password");

        Assert.Equal("Dev User", request.Name);
        Assert.Equal("user@example.com", request.Email);
        Assert.Equal("password", request.Password);
    }

    [Fact]
    public void LoginRequest_preserves_credentials()
    {
        var request = new LoginRequest("user@example.com", "password");

        Assert.Equal("user@example.com", request.Email);
        Assert.Equal("password", request.Password);
    }

    [Fact]
    public void RefreshTokenRequest_preserves_token()
    {
        var request = new RefreshTokenRequest("refresh-token");

        Assert.Equal("refresh-token", request.RefreshToken);
    }

    [Fact]
    public void RevokeTokenRequest_preserves_token()
    {
        var request = new RevokeTokenRequest("refresh-token");

        Assert.Equal("refresh-token", request.RefreshToken);
    }

    [Fact]
    public void AuthResponse_preserves_tokens_expiration_and_user()
    {
        var expiration = DateTimeOffset.UtcNow.AddMinutes(15);
        var user = new UserResponse(Guid.NewGuid(), "Dev User", "user@example.com", true);

        var response = new AuthResponse("access-token", "refresh-token", expiration, user);

        Assert.Equal("access-token", response.AccessToken);
        Assert.Equal("refresh-token", response.RefreshToken);
        Assert.Equal(expiration, response.AccessTokenExpiresAt);
        Assert.Same(user, response.User);
    }

    [Fact]
    public void UserResponse_preserves_public_profile_values()
    {
        var id = Guid.NewGuid();

        var response = new UserResponse(id, "Dev User", "user@example.com", false);

        Assert.Equal(id, response.Id);
        Assert.Equal("Dev User", response.Name);
        Assert.Equal("user@example.com", response.Email);
        Assert.False(response.IsActive);
        Assert.DoesNotContain(
            typeof(UserResponse).GetProperties(),
            property => property.Name.Contains("Hash", StringComparison.OrdinalIgnoreCase));
    }
}

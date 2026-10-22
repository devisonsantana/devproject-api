using DevProject.Domain.Entities;

namespace DevProject.Domain.Tests;

public sealed class RefreshTokenTests
{
    [Fact]
    public void Unexpired_token_is_active()
    {
        var token = new RefreshToken(
            Guid.NewGuid(),
            "token-hash",
            DateTimeOffset.UtcNow.AddMinutes(5));

        Assert.False(token.IsRevoked);
        Assert.False(token.IsExpired);
        Assert.True(token.IsActive);
    }

    [Fact]
    public void Revoked_token_is_no_longer_active()
    {
        var token = new RefreshToken(
            Guid.NewGuid(),
            "token-hash",
            DateTimeOffset.UtcNow.AddMinutes(5));

        token.Revoke("replacement-hash");

        Assert.True(token.IsRevoked);
        Assert.False(token.IsActive);
        Assert.Equal("replacement-hash", token.ReplacedByTokenHash);
    }

    [Fact]
    public void Expired_token_is_not_active()
    {
        var token = new RefreshToken(
            Guid.NewGuid(),
            "token-hash",
            DateTimeOffset.UtcNow.AddSeconds(-1));

        Assert.True(token.IsExpired);
        Assert.False(token.IsActive);
    }
}

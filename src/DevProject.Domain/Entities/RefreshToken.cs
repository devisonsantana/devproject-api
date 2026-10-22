namespace DevProject.Domain.Entities;

public sealed class RefreshToken : Entity
{
    private RefreshToken()
    {
    }

    public RefreshToken(Guid userId, string tokenHash, DateTimeOffset expiresAt)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        UserId = userId;
        TokenHash = NormalizeRequired(tokenHash, nameof(tokenHash));
        ExpiresAt = expiresAt;
    }

    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public string? ReplacedByTokenHash { get; private set; }

    public bool IsRevoked => RevokedAt.HasValue;

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke(string? replacedByTokenHash = null)
    {
        if (IsRevoked)
        {
            throw new ArgumentException("Refresh token is already revoked.");
        }

        if (!string.IsNullOrWhiteSpace(replacedByTokenHash))
        {
            ReplacedByTokenHash = replacedByTokenHash.Trim();
        }

        RevokedAt = DateTimeOffset.UtcNow;
        UpdateTimestamp();
    }

    private static string NormalizeRequired(string value, string parameterName)
    {
        var normalizedValue = value?.Trim();
        return !string.IsNullOrEmpty(normalizedValue)
            ? normalizedValue
            : throw new ArgumentException("Value is required.", parameterName);
    }
}

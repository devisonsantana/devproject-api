namespace DevProject.Domain.Entities;

public sealed class User : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string NormalizedEmail { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private User()
    {
    }

    public User(string name, string email, string passwordHash)
    {
        Name = NormalizeRequired(name, nameof(name));
        Email = NormalizeEmail(email);
        NormalizedEmail = Email.ToUpperInvariant();
        PasswordHash = NormalizeRequired(passwordHash, nameof(passwordHash));
        IsActive = true;
    }

    public void UpdateProfile(string name, string email)
    {
        if (!TryNormalizeRequired(name, out var normalizedName)
            || !TryNormalizeEmail(email, out var normalizedEmail))
        {
            throw new ArgumentException("user.profile.invalid", "Name and email are required.");
        }

        Name = normalizedName;
        Email = normalizedEmail;
        NormalizedEmail = normalizedEmail.ToUpperInvariant();
        UpdateTimestamp();
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        if (!TryNormalizeRequired(passwordHash, out var normalizedHash))
        {
            throw new ArgumentException("Password hash is required.");
        }

        PasswordHash = normalizedHash;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        UpdateTimestamp();
    }

    private static string NormalizeRequired(string value, string parameterName) =>
        TryNormalizeRequired(value, out var normalizedValue)
            ? normalizedValue
            : throw new ArgumentException("Value is required.", parameterName);

    private static bool TryNormalizeRequired(string? value, out string normalizedValue)
    {
        normalizedValue = value?.Trim() ?? string.Empty;
        return normalizedValue.Length > 0;
    }

    private static string NormalizeEmail(string email) =>
        TryNormalizeEmail(email, out var normalizedEmail)
            ? normalizedEmail
            : throw new ArgumentException("Email is required.", nameof(email));

    private static bool TryNormalizeEmail(string? email, out string normalizedEmail)
    {
        normalizedEmail = email?.Trim() ?? string.Empty;
        return normalizedEmail.Length > 0;
    }
}

namespace DevProject.Domain.Models;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public User(string name, string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Name = NormalizeRequiredText(name, nameof(name));
        Email = NormalizeRequiredText(email, nameof(email));
        PasswordHash = NormalizeRequiredText(passwordHash, nameof(passwordHash));

        var now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public void UpdateName(string name)
    {
        Name = NormalizeRequiredText(name, nameof(name));

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        Email = NormalizeRequiredText(email, nameof(email));

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        PasswordHash = NormalizeRequiredText(passwordHash, nameof(passwordHash));

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static string NormalizeRequiredText(string value, string param)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"User {param} cannot be empty.", param);

        return value.Trim();
    }

}

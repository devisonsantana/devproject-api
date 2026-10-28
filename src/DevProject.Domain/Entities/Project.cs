using DevProject.Domain.Enums;

namespace DevProject.Domain.Entities;

public sealed class Project : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ProjectStatus Status { get; private set; }
    public ICollection<Feature> Features { get; private set; } = [];

    private Project()
    {
    }

    public Project(string name, string? description = null)
    {
        Name = NormalizeRequired(name, nameof(name));
        Description = NormalizeOptional(description);
        Status = ProjectStatus.Planned;
    }

    public void UpdateDetails(string name, string? description)
    {
        if (!TryNormalizeRequired(name, out var normalizedName))
        {
            throw new ArgumentException("Project name is required.");
        }

        Name = normalizedName;
        Description = NormalizeOptional(description);
        UpdateTimestamp();
    }

    public void ChangeStatus(ProjectStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException("Project status is invalid.");
        }

        if (Status == status)
        {
            return;
        }

        Status = status;
        UpdateTimestamp();
    }

    private static string NormalizeRequired(string value, string parameterName)
    {
        if (!TryNormalizeRequired(value, out var normalizedValue))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return normalizedValue;
    }

    private static bool TryNormalizeRequired(string? value, out string normalizedValue)
    {
        normalizedValue = value?.Trim() ?? string.Empty;
        return normalizedValue.Length > 0;
    }

    private static string? NormalizeOptional(string? value)
    {
        var normalizedValue = value?.Trim();
        return string.IsNullOrEmpty(normalizedValue) ? null : normalizedValue;
    }
}

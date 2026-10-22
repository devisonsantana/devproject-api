using DevProject.Domain.Enums;

namespace DevProject.Domain.Entities;

public sealed class Feature : Entity
{
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public FeatureStatus Status { get; private set; }
    public int Order { get; private set; }
    public ICollection<ProjectTask> Tasks = [];

    private Feature()
    {
    }

    public Feature(Guid projectId, string name, string? description = null, int order = 0)
    {
        if (projectId == Guid.Empty)
        {
            throw new ArgumentException("Project ID is required.", nameof(projectId));
        }

        ProjectId = projectId;
        Name = NormalizeRequired(name, nameof(name));
        Description = NormalizeOptional(description);
        Status = FeatureStatus.Planned;
        Order = order;
    }

    public void UpdateDetails(string name, string? description)
    {
        if (!TryNormalizeRequired(name, out var normalizedName))
        {
            throw new ArgumentException("Feature name is required.");
        }

        Name = normalizedName;
        Description = NormalizeOptional(description);
        UpdateTimestamp();
    }

    public void ChangeStatus(FeatureStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException("Feature status is invalid.");
        }

        if (Status == status)
        {
            return;
        }

        Status = status;
        UpdateTimestamp();
    }

    public void ChangeOrder(int order)
    {
        if (Order == order)
        {
            return;
        }

        Order = order;
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

    private static string? NormalizeOptional(string? value)
    {
        var normalizedValue = value?.Trim();
        return string.IsNullOrEmpty(normalizedValue) ? null : normalizedValue;
    }
}

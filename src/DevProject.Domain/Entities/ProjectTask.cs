using DevProject.Domain.Enums;

namespace DevProject.Domain.Entities;

public sealed class ProjectTask : Entity
{
    public Guid FeatureId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ProjectTaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public DateTimeOffset? DueDate { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public int Order { get; private set; }

    private ProjectTask()
    {
    }

    public ProjectTask(
        Guid featureId,
        string title,
        string? description = null,
        TaskPriority priority = TaskPriority.Medium,
        DateTimeOffset? dueDate = null,
        int order = 0)
    {
        if (featureId == Guid.Empty)
        {
            throw new ArgumentException("Feature ID is required.", nameof(featureId));
        }

        FeatureId = featureId;
        Title = NormalizeRequired(title, nameof(title));
        Description = NormalizeOptional(description);
        Status = ProjectTaskStatus.Todo;
        Priority = Enum.IsDefined(priority)
            ? priority
            : throw new ArgumentException(nameof(priority));
        DueDate = dueDate;
        Order = order;
    }

    public void UpdateDetails(
        string title,
        string? description,
        TaskPriority priority,
        DateTimeOffset? dueDate)
    {
        if (!TryNormalizeRequired(title, out var normalizedTitle))
        {
            throw new ArgumentException("Task title is required.");
        }

        if (!Enum.IsDefined(priority))
        {
            throw new ArgumentException("Task priority is invalid.");
        }

        Title = normalizedTitle;
        Description = NormalizeOptional(description);
        Priority = priority;
        DueDate = dueDate;
        UpdateTimestamp();
    }

    public void ChangeStatus(ProjectTaskStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException("Task status is invalid.");
        }

        if (Status == status)
        {
            return;
        }

        Status = status;
        CompletedAt = status == ProjectTaskStatus.Done ? DateTimeOffset.UtcNow : null;
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

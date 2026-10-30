using DevProject.Domain.Enums;

namespace DevProject.Application.DTOs.Tasks;

/// <summary>Task data returned by the API.</summary>
public sealed record ProjectTaskResponse(
    Guid Id,
    Guid FeatureId,
    string Title,
    string? Description,
    ProjectTaskStatus Status,
    TaskPriority Priority,
    DateTimeOffset? DueDate,
    DateTimeOffset? CompletedAt,
    int Order,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

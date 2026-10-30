using DevProject.Domain.Enums;

namespace DevProject.Application.DTOs.Tasks;

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

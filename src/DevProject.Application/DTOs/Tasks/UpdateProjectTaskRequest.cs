using DevProject.Domain.Enums;

namespace DevProject.Application.DTOs.Tasks;

public sealed record UpdateProjectTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTimeOffset? DueDate,
    int Order);

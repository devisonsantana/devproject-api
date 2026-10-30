using DevProject.Domain.Enums;

namespace DevProject.Application.DTOs.Tasks;

public sealed record CreateProjectTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority = TaskPriority.Medium,
    DateTimeOffset? DueDate = null,
    int Order = 0);

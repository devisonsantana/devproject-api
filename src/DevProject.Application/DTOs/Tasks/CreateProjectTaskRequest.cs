using DevProject.Domain.Enums;

namespace DevProject.Application.DTOs.Tasks;

/// <summary>Data required to create a task within a feature.</summary>
public sealed record CreateProjectTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority = TaskPriority.Medium,
    DateTimeOffset? DueDate = null,
    int Order = 0);

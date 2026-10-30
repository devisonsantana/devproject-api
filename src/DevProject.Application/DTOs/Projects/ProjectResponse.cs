using DevProject.Domain.Enums;

namespace DevProject.Application.DTOs.Projects;

/// <summary>Project data returned by the API.</summary>
public sealed record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    ProjectStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

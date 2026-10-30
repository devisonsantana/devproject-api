using DevProject.Domain.Enums;

namespace DevProject.Application.DTOs.Features;

public sealed record FeatureResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    string? Description,
    FeatureStatus Status,
    int Order,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

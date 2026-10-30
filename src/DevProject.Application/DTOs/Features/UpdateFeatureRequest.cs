using DevProject.Domain.Enums;

namespace DevProject.Application.DTOs.Features;

public sealed record UpdateFeatureRequest(
    string Name,
    string? Description,
    FeatureStatus Status,
    int Order);

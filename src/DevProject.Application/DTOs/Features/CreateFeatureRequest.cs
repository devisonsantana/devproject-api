namespace DevProject.Application.DTOs.Features;

public sealed record CreateFeatureRequest(string Name, string? Description, int Order = 0);

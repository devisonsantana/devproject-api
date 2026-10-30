namespace DevProject.Application.DTOs.Features;

/// <summary>Data required to create a feature within a project.</summary>
public sealed record CreateFeatureRequest(string Name, string? Description, int Order = 0);

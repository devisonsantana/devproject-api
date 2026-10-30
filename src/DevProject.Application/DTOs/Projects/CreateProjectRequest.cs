namespace DevProject.Application.DTOs.Projects;

/// <summary>Data required to create a project.</summary>
public sealed record CreateProjectRequest(string Name, string? Description);
